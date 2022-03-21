﻿using Sandbox;
using Sandbox.Component;

public partial class PlatesPlayer : Player
{
	private TimeSince timeSinceDropped;
	private TimeSince timeSinceJumpReleased;

	private DamageInfo lastDamage;

	/// <summary>
	/// The clothing container is what dresses the citizen
	/// </summary>
	public Clothing.Container Clothing = new();

	[Net] public Plate CurrentPlate { get; set; }

	[Net] public bool BlurFX { get; set; } = false;

	private Glow glow;

	/// <summary>
	/// Init Player
	/// </summary>
	public PlatesPlayer()
	{
		Inventory = new Inventory( this );
		// Initialize glow
		glow = Components.GetOrCreate<Glow>();
		glow.Active = false;
		glow.RangeMin = 0;
		glow.RangeMax = 2000;
		glow.Color = Color.Blue;
}

	/// <summary>
	/// Init Player using this client
	/// </summary>
	public PlatesPlayer( Client cl ) : this()
	{
		Clothing.LoadFromClient( cl );
	}

	public void ResetValues(bool changeProperties = true) {
		SetGlow(false);
		BlurFX = false;
		if(changeProperties)
		{
			Scale = 1.0f;
			RenderColor = RenderColor.WithAlpha( 1 );
			Velocity = Vector3.Zero;
		}
	}

	public override void Respawn()
	{
		SetModel( "models/citizen/citizen.vmdl" );

		Controller = new PlatesWalkController();
		(Controller as PlatesWalkController).AutoJump = true;
		Animator = new StandardPlayerAnimator();

		if ( DevController is NoclipController )
			DevController = null;

		EnableAllCollisions = true;
		EnableDrawing = true;
		EnableHideInFirstPerson = true;
		EnableShadowInFirstPerson = true;

		Clothing.DressEntity( this );

			
		ResetValues();

		Inventory.DeleteContents();

		Inventory.Add( new Fists() );
		//Inventory.Add( new SWB_CSS.Knife() );
		//Inventory.Add( new GravGun() );
		//Inventory.Add( new Tool() );
		//Inventory.Add( new Pistol() );
		//Inventory.Add( new Flashlight() );

		CameraMode = new FirstPersonCamera();

		base.Respawn();
	}

	public override void OnKilled()
	{
		base.OnKilled();

		// Vehicle Death
		if ( lastDamage.Flags.HasFlag( DamageFlags.Vehicle ) )
		{
			Particles.Create( "particles/impact.flesh.bloodpuff-big.vpcf", lastDamage.Position );
			Particles.Create( "particles/impact.flesh-big.vpcf", lastDamage.Position );
			PlaySound( "kersplat" );
		}

		BecomeRagdollOnClient( Velocity, lastDamage.Flags, lastDamage.Position, lastDamage.Force * 2, GetHitboxBone( lastDamage.HitboxIndex ) );
			
		Controller = null;

		EnableAllCollisions = false;
		EnableDrawing = false;

		CameraMode = new SpectateRagdollCamera();

		ResetValues(false);

		foreach ( var child in Children )
		{
			child.EnableDrawing = false;
		}

		Inventory.DropActive();
		Inventory.DeleteContents();
	}

	public override void TakeDamage( DamageInfo info )
	{
		if ( GetHitboxGroup( info.HitboxIndex ) == 1 )
		{
			info.Damage *= 10.0f;
		}

		lastDamage = info;

		TookDamage( lastDamage.Flags, lastDamage.Position, lastDamage.Force );

		base.TakeDamage( info );
	}

	[ClientRpc]
	public void TookDamage( DamageFlags damageFlags, Vector3 forcePos, Vector3 force )
	{
	}

	public override PawnController GetActiveController()
	{
		if ( DevController != null ) return DevController;

		return base.GetActiveController();
	}
	
	public override void Simulate( Client cl )
	{
		base.Simulate( cl );

		if ( Input.ActiveChild != null )
		{
			ActiveChild = Input.ActiveChild;
		}

		if ( LifeState != LifeState.Alive )
			return;

		var controller = GetActiveController();
		if ( controller != null )
			EnableSolidCollisions = !controller.HasTag( "noclip" );

		TickPlayerUse();
		SimulateActiveChild( cl, ActiveChild );

		if ( Input.Pressed( InputButton.View ) )
		{
			if ( CameraMode is ThirdPersonCamera )
			{
				CameraMode = new FirstPersonCamera();
			}
			else
			{
				CameraMode = new ThirdPersonCamera();
			}
		}

		if ( Input.Pressed( InputButton.Drop ) )
		{
			var dropped = Inventory.DropActive();
			if ( dropped != null )
			{
				dropped.PhysicsGroup.ApplyImpulse( Velocity + EyeRotation.Forward * 500.0f + Vector3.Up * 100.0f, true );
				dropped.PhysicsGroup.ApplyAngularImpulse( Vector3.Random * 100.0f, true );

				timeSinceDropped = 0;
			}
		}

		if ( Input.Released( InputButton.Jump ) )
		{
			if ( timeSinceJumpReleased < 0.3f )
			{
				Game.Current?.DoPlayerNoclip( cl );
			}

			timeSinceJumpReleased = 0;
		}

		if ( Input.Left != 0 || Input.Forward != 0 )
		{
			timeSinceJumpReleased = 1;
		}
	}

	public override void StartTouch( Entity other )
	{
		if ( timeSinceDropped < 1 ) return;

		base.StartTouch( other );
	}

	[ServerCmd( "inventory_current" )]
	public static void SetInventoryCurrent( string entName )
	{
		var target = ConsoleSystem.Caller.Pawn as Player;
		if ( target == null ) return;

		var inventory = target.Inventory;
		if ( inventory == null )
			return;

		for ( int i = 0; i < inventory.Count(); ++i )
		{
			var slot = inventory.GetSlot( i );
			if ( !slot.IsValid() )
				continue;

			if ( !slot.ClassInfo.IsNamed( entName ) )
				continue;

			inventory.SetActiveSlot( i, false );

			break;
		}
	}

	public void SetGlow( bool visible, Color color = default )
	{
		if ( color == default )
			color = glow.Color;

		glow.Active = visible;
	}
}
