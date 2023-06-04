using Sandbox;
using Sandbox.UI;
using Sandbox.Component;

public partial class PlatesPlayer : Player
{
	private TimeSince timeSinceDropped;
	private TimeSince timeSinceJumpReleased;

	private DamageInfo lastDamage;
	private Entity lastWeapon;

	[Net] private Glow glow {get; set;}

	public ClothingContainer Clothing = new();
	[Net] public string ClothingString { get; set; } = "";

	[Net] public Plate CurrentPlate {get; set;}
	[Net] public int EventCount {get;set;} = 0;
	[Net] public bool InGame {get;set;} = false;

	public float FieldOfView { get; set; } = 90;
	public float ThirdPersonZoom { get; set; } = 0;
	public Angles ThirdPersonRotation { get; set; }


	public PlatesPlayer()
	{
		Inventory = new Inventory(this);
	}

	public PlatesPlayer( IClient cl ) : this()
	{
		Clothing.LoadFromClient( cl );
		ClothingString = Clothing.Serialize();
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

	public override void Spawn()
	{
		// Initialize glow
		glow = Components.GetOrCreate<Glow>();
		glow.Enabled = false;
		glow.Color = Color.Blue;
	}

	public override void Respawn()
	{
		SetModel("models/citizen/citizen.vmdl");
		
		Controller = new PlatesWalkController()
		{
			WalkSpeed = 60.0f,
			DefaultSpeed = 180.0f
		};
		(Controller as PlatesWalkController).AutoJump = true;
		if(DevController is NoclipController) DevController = null;

		InGame = false;
		EventCount = 0;
		ResetValues();

		this.ClearWaterLevel();
		EnableAllCollisions = true;
		EnableDrawing = true;
		EnableHideInFirstPerson = true;
		EnableShadowInFirstPerson = true;

		Clothing.DressEntity(this);

		base.Respawn();
	}

	public override void OnKilled()
	{
		base.OnKilled();

		BecomeRagdollOnClient( Velocity, lastDamage.Position, lastDamage.Force, lastDamage.BoneIndex, lastDamage.HasTag( "bullet" ), lastDamage.HasTag( "blast" ) );

		Controller = null;

		EnableAllCollisions = false;
		EnableDrawing = false;

		foreach(var child in Children){
			child.EnableDrawing = false;
		}

		ResetValues(false);

		Inventory.DropActive();
		Inventory.DeleteContents();
	}

	RealTimeSince timeSinceDied = 0;
	public override void Simulate( IClient cl )
	{
		base.Simulate( cl );

		if(LifeState != LifeState.Alive) return;

		if(GroundEntity is ConveyorBelt belt)
		{
			BaseVelocity = belt.Direction * belt.Speed;
		}

		var controller = GetActiveController();
		if(controller != null)
		{
			EnableSolidCollisions = !controller.HasTag("noclip");
			SimulateAnimation(controller);
		}

		TickPlayerUse();
		SimulateActiveChild(cl, ActiveChild);

		if(Input.Pressed("view"))
		{
			if(ThirdPersonZoom > 0f) ThirdPersonZoom = 0f;
			else ThirdPersonZoom = 130f;
		}

		if(Input.Pressed("drop")){
			var dropped = Inventory.DropActive();
			if(dropped != null){
				dropped.PhysicsGroup.ApplyImpulse(Velocity + EyeRotation.Forward * 500.0f + Vector3.Up * 100.0f, true);
				dropped.PhysicsGroup.ApplyAngularImpulse(Vector3.Random * 100.0f, true);

				timeSinceDropped = 0;
			}
		}

		if ( InputDirection.y != 0 || InputDirection.x != 0 )
		{
			timeSinceJumpReleased = 1;
		}
		
	}

	public override void FrameSimulate( IClient cl )
	{
		if(ThirdPersonZoom > 0f) // THIRD PERSON CAM
		{
			Camera.Rotation = ViewAngles.ToRotation() * ThirdPersonRotation.ToRotation();
		}
		else // FIRST PERSON CAM
		{
			Camera.Rotation = ViewAngles.ToRotation();
			ThirdPersonRotation = default;
		}
		var targetFOV = Game.Preferences.FieldOfView - (Input.Down("Zoom") ? 40 : 0);
		FieldOfView = MathX.LerpTo( FieldOfView, targetFOV, Time.Delta * 10f );
		Camera.FieldOfView = Screen.CreateVerticalFieldOfView( FieldOfView );

		if(Input.MouseWheel != 0)
		{

			// ZOOM CAMERA IN/OUT
			float previousZoom = ThirdPersonZoom;
			ThirdPersonZoom = MathX.Clamp(ThirdPersonZoom - Input.MouseWheel * 10, 10, 400);
			if(Input.MouseWheel > 0f && ThirdPersonZoom <= 10)
			{
				ThirdPersonZoom = 0f;
			}
			else if(Input.MouseWheel < 0f && ThirdPersonZoom < 10)
			{
				ThirdPersonZoom = 10;
			}
		}

		if ( LifeState != LifeState.Alive && Corpse.IsValid() ) // RAGDOLL CAM
		{
			Corpse.EnableDrawing = true;

			var pos = Corpse.GetBoneTransform( 0 ).Position + Vector3.Up * 10;
			var targetPos = pos + Camera.Rotation.Backward * 100;

			var tr = Trace.Ray( pos, targetPos )
				.WithAnyTags( "solid" )
				.Ignore( this )
				.Radius( 8 )
				.Run();

			Camera.Position = tr.EndPosition;
			Camera.FirstPersonViewer = null;
		}
		else if ( ThirdPersonZoom > 0f ) // THIRD PERSON CAM
		{
			Camera.FirstPersonViewer = null;

			// Vector3 targetPos;
			// var center = Position + Vector3.Up * 64;

			// var pos = center;
			// var rot = Camera.Rotation * Rotation.FromAxis( Vector3.Up, -16 );

			// float distance = ThirdPersonZoom * Scale;
			// targetPos = pos + rot.Right * ((CollisionBounds.Mins.x + 16) * Scale);
			// targetPos += rot.Forward * -distance;

			var pos = Position + Vector3.Up * 64;
			var targetPos = pos + (Camera.Rotation).Backward * ThirdPersonZoom;

			var tr = Trace.Ray( pos, targetPos )
				.WithAnyTags( "solid" )
				.Ignore( this )
				.Radius( 8 )
				.Run();

			Camera.Position = tr.EndPosition;
		}
		else // FIRST PERSON CAM
		{
			Camera.Position = EyePosition;
			Camera.FirstPersonViewer = this;
			Camera.Main.SetViewModelCamera( 90f );
		}
	}

	void SimulateAnimation(PawnController controller)
	{
		if(controller == null) return;

		// Where should we be rotated to
		var turnSpeed = 0.02f;

		Rotation rotation;

		// If we're a bot, spin us around 180 degrees
		if ( Client.IsBot )
			rotation = ViewAngles.WithYaw( ViewAngles.yaw + 180f ).ToRotation();
		else
			rotation = ViewAngles.ToRotation();

		var idealRotation = Rotation.LookAt( rotation.Forward.WithZ( 0 ), Vector3.Up );
		Rotation = Rotation.Slerp( Rotation, idealRotation, controller.WishVelocity.Length * Time.Delta * turnSpeed );
		Rotation = Rotation.Clamp( idealRotation, 45.0f, out var shuffle ); // lock facing to within 45 degrees of look direction

		CitizenAnimationHelper animHelper = new CitizenAnimationHelper( this );

		animHelper.WithWishVelocity( controller.WishVelocity );
		animHelper.WithVelocity( controller.Velocity );
		animHelper.WithLookAt( EyePosition + EyeRotation.Forward * 100.0f, 1.0f, 1.0f, 0.5f );
		animHelper.AimAngle = rotation;
		animHelper.FootShuffle = shuffle;
		animHelper.DuckLevel = MathX.Lerp( animHelper.DuckLevel, controller.HasTag( "ducked" ) ? 1 : 0, Time.Delta * 10.0f );
		animHelper.VoiceLevel = ( Game.IsClient && Client.IsValid() ) ? Client.Voice.LastHeard < 0.5f ? Client.Voice.CurrentLevel : 0.0f : 0.0f;
		animHelper.IsGrounded = GroundEntity != null;
		animHelper.IsSitting = controller.HasTag( "sitting" );
		animHelper.IsNoclipping = controller.HasTag( "noclip" );
		animHelper.IsClimbing = controller.HasTag( "climbing" );
		animHelper.IsSwimming = this.GetWaterLevel() >= 0.5f;
		animHelper.IsWeaponLowered = false;
		animHelper.MoveStyle = Input.Down( "run" ) ? CitizenAnimationHelper.MoveStyles.Run : CitizenAnimationHelper.MoveStyles.Walk;

		if ( controller.HasEvent( "jump" ) ) animHelper.TriggerJump();
		if ( ActiveChild != lastWeapon ) animHelper.TriggerDeploy();

		if ( ActiveChild is BaseCarriable carry )
		{
			carry.SimulateAnimator( animHelper );
		}
		else
		{
			animHelper.HoldType = CitizenAnimationHelper.HoldTypes.None;
			animHelper.AimBodyWeight = 0.5f;
		}

		lastWeapon = ActiveChild;
	}

	WorldInput WorldInput = new();

	[ConCmd.Admin("noclip")]
	static void DoPlayerNoclip()
	{
		if(ConsoleSystem.Caller.Pawn is PlatesPlayer player)
		{
			if(player.DevController is NoclipController)
			{
				player.DevController = null;
			}
			else
			{
				player.DevController = new NoclipController();
			}
		}
	}

	// "kill" command that kills the player who called it
	[ConCmd.Server("kill")]
	static void DoPlayerSuicide()
	{
		if(ConsoleSystem.Caller.Pawn is PlatesPlayer player)
		{
			player.TakeDamage(new DamageInfo {Damage = player.Health * 99 });
		}
	}

	/// <summary>
	/// Called from the gamemode, clientside only.
	/// </summary>
	public override void BuildInput()
	{

		if(Input.Down("attack2") && ThirdPersonZoom > 0f)
		{
			ThirdPersonRotation += Input.AnalogLook.WithYaw(Input.AnalogLook.yaw * 1f);
			Input.AnalogLook = default;
		}
		else
		{
			ThirdPersonRotation = ThirdPersonRotation.LerpTo(default, Time.Delta * 10f);
		}

		base.BuildInput();
		
		WorldInput.Ray = new Ray( EyePosition, EyeRotation.Forward );
		WorldInput.MouseLeftPressed = Input.Down( "attack1" );
	}

	public void SetGlow(bool visible, Color color = default)
	{
		if(color != default)
		{
			glow.Color = color;
		}
		glow.Enabled = visible;
	}
}
