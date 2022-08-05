using Sandbox;
using Sandbox.Component;

public partial class PlatesPlayer : Player
{
	private TimeSince timeSinceDropped;
	private TimeSince timeSinceJumpReleased;

	private DamageInfo lastDamage;
	private Entity lastWeapon;

	[Net] private Glow glow {get; set;}

	public ClothingContainer Clothing = new();

	[Net] public Plate CurrentPlate {get; set;}
	[Net] public int EventCount {get;set;} = 0;
	[Net] public bool InGame {get;set;} = false;
	private NameTag nameTag = null;

	public PlatesPlayer()
	{
		Inventory = new BaseInventory( this );

		// Initialize glow
		glow = Components.GetOrCreate<Glow>();
		glow.Active = false;
		glow.RangeMin = 0;
		glow.RangeMax = 2000;
		glow.Color = Color.Blue;
	}

	public PlatesPlayer( Client cl ) : this()
	{
		Clothing.LoadFromClient( cl );
	}

	public void ResetValues(bool changeProperties = true)
	{
		SetGlow(false);
		if(changeProperties)
		{
			Scale = 1.0f;
			RenderColor = Color.White;
			Velocity = Vector3.Zero;
		}
	}

	public override void Respawn()
	{
		SetModel("models/citizen/citizen.vmdl");
		
		Controller = new PlatesWalkController();
		(Controller as PlatesWalkController).AutoJump = true;

		InGame = false;
		EventCount = 0;

		ResetValues();

		EnableAllCollisions = true;
		EnableDrawing = true;
		EnableHideInFirstPerson = true;
		EnableShadowInFirstPerson = true;

		Clothing.DressEntity(this);

		CameraMode = new FirstPersonCamera();

		base.Respawn();
	}

	public override void OnKilled()
	{
		base.OnKilled();

		BecomeRagdollOnClient(Velocity, lastDamage.Flags, lastDamage.Position, lastDamage.Force, GetHitboxBone(lastDamage.HitboxIndex));

		Controller = null;

		EnableAllCollisions = false;
		EnableDrawing = false;

		CameraMode = new SpectateRagdollCamera();

		ResetValues(false);

		foreach(var child in Children){
			child.EnableDrawing = false;
		}

		if(nameTag != null) nameTag.Delete();

		Inventory.DropActive();
		Inventory.DeleteContents();
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		if(nameTag != null) nameTag.Delete();
	}

	public override void TakeDamage( DamageInfo info )
	{
		if(GetHitboxGroup(info.HitboxIndex) == 1){
			info.Damage *= 10.0f;
		}

		lastDamage = info;

		base.TakeDamage( info );
	}

	public override PawnController GetActiveController()
	{
		// if ( DevController != null ) return DevController;

		return base.GetActiveController();
	}

	[Event.Tick]
	public void Tick()
	{
		if(IsClient && nameTag == null && Client != Local.Client)
        {
            nameTag = new NameTag(this);
        }
	}

	public override void Simulate( Client cl )
	{
		base.Simulate( cl );

		if(Input.ActiveChild != null)
		{
			ActiveChild = Input.ActiveChild;
		}

		if(LifeState != LifeState.Alive) return;

		var controller = GetActiveController();
		if(controller != null)
		{
			EnableSolidCollisions = !controller.HasTag("noclip");
			SimulateAnimation(controller);
		}

		TickPlayerUse();
		SimulateActiveChild(cl, ActiveChild);

		if(Input.Pressed(InputButton.View))
		{
			if(CameraMode is ThirdPersonCamera){
				CameraMode = new FirstPersonCamera();
			}else{
				CameraMode = new ThirdPersonCamera();
			}
		}

		if(Input.Pressed(InputButton.Drop)){
			var dropped = Inventory.DropActive();
			if(dropped != null){
				dropped.PhysicsGroup.ApplyImpulse(Velocity + EyeRotation.Forward * 500.0f + Vector3.Up * 100.0f, true);
				dropped.PhysicsGroup.ApplyAngularImpulse(Vector3.Random * 100.0f, true);

				timeSinceDropped = 0;
			}
		}

		if ( Input.Left != 0 || Input.Forward != 0 )
		{
			timeSinceJumpReleased = 1;
		}
		
	}

	void SimulateAnimation(PawnController controller)
	{
		if(controller == null) return;

		var turnSpeed = 0.02f;
		var idealRotation = Rotation.LookAt(Input.Rotation.Forward.WithZ(0), Vector3.Up);
		Rotation = Rotation.Slerp(Rotation, idealRotation, controller.WishVelocity.Length * Time.Delta * turnSpeed);
		Rotation = Rotation.Clamp(idealRotation, 45.0f, out var shuffle);

		CitizenAnimationHelper animHelper = new CitizenAnimationHelper(this);

		animHelper.WithWishVelocity(controller.WishVelocity);
		animHelper.WithVelocity(controller.Velocity);
		animHelper.WithLookAt(EyePosition + EyeRotation.Forward * 100.0f, 1.0f, 1.0f, 0.5f);
		animHelper.AimAngle = Input.Rotation;
		animHelper.FootShuffle = shuffle;
		animHelper.DuckLevel = MathX.Lerp(animHelper.DuckLevel, controller.HasTag("ducked") ? 1 : 0, Time.Delta * 10.0f);
		animHelper.VoiceLevel = ( Host.IsClient && Client.IsValid() ) ? Client.TimeSinceLastVoice < 0.5f ? Client.VoiceLevel : 0.0f : 0.0f;
		animHelper.IsGrounded = GroundEntity != null;
		animHelper.IsSitting = controller.HasTag("sitting");
		animHelper.IsNoclipping = controller.HasTag("noclip");
		animHelper.IsClimbing = controller.HasTag("climbing");
		animHelper.IsSwimming = WaterLevel >= 0.5f;
		animHelper.IsWeaponLowered = false;

		if(controller.HasEvent("jump")) animHelper.TriggerJump();
		if(ActiveChild != lastWeapon) animHelper.TriggerDeploy();

		if(ActiveChild is BaseCarriable carry){
			carry.SimulateAnimator(animHelper);
		}else{
			animHelper.HoldType = CitizenAnimationHelper.HoldTypes.None;
			animHelper.AimBodyWeight = 0.5f;
		}

		lastWeapon = ActiveChild;
	}

	public override void StartTouch( Entity other )
	{
		if(timeSinceDropped < 1) return;

		base.StartTouch( other );
	}

	public override float FootstepVolume()
	{
		return Velocity.WithZ(0).Length.LerpInverse(0.0f, 200.0f) * 5.0f;
	}

	public void SetGlow(bool visible, Color color = default)
	{
		if(color != default)
		{
			glow.Color = color;
		}
		glow.Active = visible;
	}

	[ClientRpc]
	public static void GiveMoney(int amount, bool force = false)
	{
		if(Local.Pawn is PlatesPlayer ply)
		{
			if(force || ply.InGame)
			{
				PlatesPlayerData.GiveMoney(amount);
			}
		}
	}
}
