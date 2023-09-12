using System.Globalization;
using System;
using Sandbox;
using Sandbox.UI;
using Sandbox.Component;
using System.ComponentModel;
using System.Text.Json;
using System.Collections.Generic;

namespace Plates;

public partial class Player : AnimatedEntity
{
	[Net, Predicted] public PawnController Controller { get; set; }

	public static Player Local => Game.LocalPawn as Player;

    /// <summary>
	/// This is used for noclip mode
	/// </summary>
	[Net, Predicted]
	public PawnController DevController { get; set; }

    [Net, Predicted] public Entity ActiveChild { get; set; }
	[ClientInput] public Vector3 InputDirection { get; protected set; }
	[ClientInput] public Entity ActiveChildInput { get; set; }
	[ClientInput] public Angles ViewAngles { get; set; }
	public Angles OriginalViewAngles { get; private set; }

    /// <summary>
	/// Player's inventory for entities that can be carried. See <see cref="BaseCarriable"/>.
	/// </summary>
	public Inventory Inventory { get; protected set; }

    private DamageInfo lastDamage;

	public float FieldOfView { get; set; } = 90;
	public float ThirdPersonZoom { get; set; } = 0;
	public Angles ThirdPersonRotation { get; set; }

	public RealTimeSince TimeSinceSpawned { get; set; }

	[Net] private Glow glow {get; set;}

    /// <summary>
    /// The clothing container is what dresses the citizen
    /// </summary>
    public ClothingContainer Clothing = new();
	[Net] public string ClothingString { get; set; } = "";

	[Net] public Plate CurrentPlate {get; set;}
	[Net] public int EventCount {get;set;} = 0;
	[Net] public bool InGame {get;set;} = false;

	[BindComponent] public AnimatorComponent Animator { get; }


    TimeSince timeSinceDied;

    public Player()
    {
        Inventory = new Inventory( this );
    }

    public Player(IClient client) : this()
    {
        // Load clothing from client data
        Clothing.LoadFromClient(client);
		ClothingString = Clothing.Serialize();
    }

    /// <summary>
	/// Return the controller to use. Remember any logic you use here needs to match
	/// on both client and server. This is called as an accessor every tick.. so maybe
	/// avoid creating new classes here or you're gonna be making a ton of garbage!
	/// </summary>
	public virtual PawnController GetActiveController()
	{
		if ( DevController != null ) return DevController;

		return Controller;
	}

	public override void Spawn()
	{
		EnableLagCompensation = true;

		Tags.Add( "player" );

		base.Spawn();

		Components.GetOrCreate<AnimatorComponent>();
	}

	public virtual void Respawn()
	{
        SetModel("models/citizen/citizen.vmdl");

		TimeSinceSpawned = 0;

        SetController(new WalkController());

        if(DevController is NoclipController)
        {
            DevController = null;
        }

        this.ClearWaterLevel();
        EnableAllCollisions = true;
        EnableDrawing = true;
        EnableHideInFirstPerson = true;
        EnableShadowInFirstPerson = true;

        Clothing.DressEntity(this);

        Game.AssertServer();

		LifeState = LifeState.Alive;
		Health = 100;
		Velocity = Vector3.Zero;

		CreateHull();

		GameManager.Current?.MoveToSpawnpoint( this );
		ResetInterpolation();
	}

	public void SetController(PawnController controller)
	{
		Game.AssertServer();
		Components.RemoveAny<PawnController>();
		Components.Add(controller);
		Controller = controller;
	}

	/// <summary>
	/// Called every tick to simulate the player. This is called on the
	/// client as well as the server (for prediction). So be careful!
	/// </summary>
	public override void Simulate( IClient cl )
	{
		if ( ActiveChildInput.IsValid() && ActiveChildInput.Owner == this )
		{
			ActiveChild = ActiveChildInput;
		}

		if ( LifeState == LifeState.Dead )
		{
			// Respawn
			if ( (timeSinceDied > 3 || Input.Pressed("attack1")) && Game.IsServer )
			{
				Respawn();
			}

			return;
		}
		
		var controller = GetActiveController();
		controller?.Simulate( cl, this );

		if(LifeState != LifeState.Alive) return;

		if(controller != null)
		{
			EnableSolidCollisions = !controller.HasTag( "noclip" );
			SimulateAnimation(controller);
		}

		TickPlayerUse();
		SimulateActiveChild(cl, ActiveChild);

		// Third Person Toggle
		if(Input.Pressed("view"))
		{
			if(ThirdPersonZoom > 0f) ThirdPersonZoom = 0f;
			else ThirdPersonZoom = 130f;
		}
	}

	[ConCmd.Admin("noclip")]
	static void DoPlayerNoclip()
	{
		if(ConsoleSystem.Caller.Pawn is Player player)
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
		if(ConsoleSystem.Caller.Pawn is Player player && player.TimeSinceSpawned > 2f)
		{
			player.TakeDamage(new DamageInfo {Damage = player.Health * 99 });
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

		animHelper.WithWishVelocity(controller.WishVelocity);
		animHelper.WithVelocity(controller.Velocity);
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
		animHelper.MoveStyle = Input.Down( "walk" ) ? CitizenAnimationHelper.MoveStyles.Walk : CitizenAnimationHelper.MoveStyles.Run;

		if ( controller.HasEvent( "jump" ) ) animHelper.TriggerJump();
		// if ( ActiveChild != lastWeapon ) animHelper.TriggerDeploy();

		if ( ActiveChild is BaseCarriable carry )
		{
			carry.SimulateAnimator( animHelper );
		}
		else
		{
			animHelper.HoldType = CitizenAnimationHelper.HoldTypes.None;
			animHelper.AimBodyWeight = 0.5f;
		}

		// lastWeapon = ActiveChild;
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

    /// <summary>
	/// Applies flashbang-like ear ringing effect to the player.
	/// </summary>
	/// <param name="strength">Can be approximately treated as duration in seconds.</param>
	[ClientRpc]
	public void Deafen( float strength )
	{
		Audio.SetEffect( "flashbang", strength, velocity: 20.0f, fadeOut: 4.0f * strength );
	}

	public override void OnKilled()
	{
		//PlatesGame.Current?.OnKilled(this);

		timeSinceDied = 0;
		LifeState = LifeState.Dead;
		StopUsing();

		Client?.AddInt("deaths", 1);

        BecomeRagdollOnClient( Velocity, lastDamage.Position, lastDamage.Force, lastDamage.BoneIndex, lastDamage.HasTag( "bullet" ), lastDamage.HasTag( "blast" ) );

		Controller = null;

		EnableAllCollisions = false;
		EnableDrawing = false;

		foreach ( var child in Children )
		{
			child.EnableDrawing = false;
		}
	}

    /// <summary>
	/// Create a physics hull for this player. The hull stops physics objects and players passing through
	/// the player. It's basically a big solid box. It also what hits triggers and stuff.
	/// The player doesn't use this hull for its movement size.
	/// </summary>
	public virtual void CreateHull()
	{
		SetupPhysicsFromAABB( PhysicsMotionType.Keyframed, new Vector3( -16, -16, 0 ), new Vector3( 16, 16, 72 ) );

		//var capsule = new Capsule( new Vector3( 0, 0, 16 ), new Vector3( 0, 0, 72 - 16 ), 32 );
		//var phys = SetupPhysicsFromCapsule( PhysicsMotionType.Keyframed, capsule );


		//	phys.GetBody(0).RemoveShadowController();

		// TODO - investigate this? if we don't set movetype then the lerp is too much. Can we control lerp amount?
		// if so we should expose that instead, that would be awesome.
		EnableHitboxes = true;
	}

    /// <summary>
	/// Called from the gamemode, clientside only.
	/// </summary>
	public override void BuildInput()
	{
		OriginalViewAngles = ViewAngles;
		InputDirection = Input.AnalogMove;

		if ( Input.StopProcessing )
			return;

		Angles look = Input.AnalogLook;

		if(Input.Down("attack2") && ThirdPersonZoom > 0f)
		{
			ThirdPersonRotation += look.WithYaw(look.yaw * 1f);
			look = default;
		}
		else
		{
			ThirdPersonRotation = ThirdPersonRotation.LerpTo(default, Time.Delta * 10f);
		}

		if ( ViewAngles.pitch > 90f || ViewAngles.pitch < -90f )
		{
			look = look.WithYaw( look.yaw * -1f );
		}

		var viewAngles = ViewAngles;
		viewAngles += look;
		viewAngles.pitch = viewAngles.pitch.Clamp( -89f, 89f );
		viewAngles.roll = 0f;
		ViewAngles = viewAngles.Normal;

		ActiveChild?.BuildInput();

		GetActiveController()?.BuildInput();
	}

    /// <summary>
	/// A generic corpse entity
	/// </summary>
	public ModelEntity Corpse { get; set; }


	TimeSince timeSinceLastFootstep = 0;

    /// <summary>
	/// A footstep has arrived!
	/// </summary>
	public override void OnAnimEventFootstep( Vector3 pos, int foot, float volume )
	{
		if ( LifeState != LifeState.Alive )
			return;

		if ( !Game.IsClient )
			return;

		if ( timeSinceLastFootstep < 0.2f )
			return;

		volume *= FootstepVolume();

		timeSinceLastFootstep = 0;

		//DebugOverlay.Box( 1, pos, -1, 1, Color.Red );
		//DebugOverlay.Text( pos, $"{volume}", Color.White, 5 );

		var tr = Trace.Ray( pos, pos + Vector3.Down * 20 )
			.Radius( 1 )
			.Ignore( this )
			.Run();

		if ( !tr.Hit ) return;

		tr.Surface.DoFootstep( this, tr, foot, volume );
	}

	/// <summary>
	/// Allows override of footstep sound volume.
	/// </summary>
	/// <returns>The new footstep volume, where 1 is full volume.</returns>
	public virtual float FootstepVolume()
	{
		return Velocity.WithZ( 0 ).Length.LerpInverse( 0.0f, 200.0f ) * 5.0f;
	}

    public override void StartTouch( Entity other )
	{
		if ( Game.IsClient ) return;

		if ( other is PickupTrigger )
		{
			StartTouch( other.Parent );
			return;
		}

        // TODO: Other pickups on touch?
		// Inventory?.Add( other, Inventory.Active == null );
	}

    /// <summary>
	/// This isn't networked, but it's predicted. If it wasn't then when the prediction system
	/// re-ran the commands LastActiveChild would be the value set in a future tick, so ActiveEnd
	/// and ActiveStart would get called multiple times and out of order, causing all kinds of pain.
	/// </summary>
	[Predicted]
	Entity LastActiveChild { get; set; }

	/// <summary>
	/// Simulated the active child. This is important because it calls ActiveEnd and ActiveStart.
	/// If you don't call these things, viewmodels and stuff won't work, because the entity won't
	/// know it's become the active entity.
	/// </summary>
	public virtual void SimulateActiveChild( IClient cl, Entity child )
	{
		if ( LastActiveChild != child )
		{
			OnActiveChildChanged( LastActiveChild, child );
			LastActiveChild = child;
		}

		if ( !LastActiveChild.IsValid() )
			return;

		if ( LastActiveChild.IsAuthority )
		{
			LastActiveChild.Simulate( cl );
		}
	}

	/// <summary>
	/// Called when the Active child is detected to have changed
	/// </summary>
	public virtual void OnActiveChildChanged( Entity previous, Entity next )
	{
		if ( previous is BaseCarriable previousBc )
		{
			previousBc?.ActiveEnd( this, previousBc.Owner != this );
		}

		if ( next is BaseCarriable nextBc )
		{
			nextBc?.ActiveStart( this );
		}
	}

	public override void TakeDamage( DamageInfo info )
	{
		if ( LifeState == LifeState.Dead )
			return;

		base.TakeDamage( info );

		this.ProceduralHitReaction( info );

		//
		// Add a score to the killer
		//
		if ( LifeState == LifeState.Dead && info.Attacker != null )
		{
			if ( info.Attacker.Client != null && info.Attacker != this )
			{
				info.Attacker.Client.AddInt( "kills" );
			}
		}

		if ( info.HasTag( "blast" ) )
		{
			Deafen( To.Single( Client ), info.Damage.LerpInverse( 0, 60 ) );
		}
	}

	// public override void OnChildAdded( Entity child )
	// {
	// 	Inventory?.OnChildAdded( child );
	// }

	// public override void OnChildRemoved( Entity child )
	// {
	// 	Inventory?.OnChildRemoved( child );
	// }

	/// <summary>
	/// Position a player should be looking from in world space.
	/// </summary>
	[Browsable( false )]
	public Vector3 EyePosition
	{
		get => Transform.PointToWorld( EyeLocalPosition );
		set => EyeLocalPosition = Transform.PointToLocal( value );
	}

	/// <summary>
	/// Position a player should be looking from in local to the entity coordinates.
	/// </summary>
	[Net, Predicted, Browsable( false )]
	public Vector3 EyeLocalPosition { get; set; }

	/// <summary>
	/// Rotation of the entity's "eyes", i.e. rotation for the camera when this entity is used as the view entity.
	/// </summary>
	[Browsable( false )]
	public Rotation EyeRotation
	{
		get => Transform.RotationToWorld( EyeLocalRotation );
		set => EyeLocalRotation = Transform.RotationToLocal( value );
	}

	/// <summary>
	/// Rotation of the entity's "eyes", i.e. rotation for the camera when this entity is used as the view entity. In local to the entity coordinates.
	/// </summary>
	[Net, Predicted, Browsable( false )]
	public Rotation EyeLocalRotation { get; set; }

	/// <summary>
	/// Override the aim ray to use the player's eye position and rotation.
	/// </summary>
	public override Ray AimRay => new Ray( EyePosition, EyeRotation.Forward );

	public void SetGlow(bool visible, Color color = default)
	{
		if(color != default)
		{
			glow.Color = color;
		}
		glow.Enabled = visible;
	}

}