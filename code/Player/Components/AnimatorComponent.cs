using System;
using Sandbox;

namespace Plates;

public partial class AnimatorComponent : EntityComponent<Player>
{
	CitizenAnimationHelper AnimHelper;

	protected override void OnActivate()
	{
		base.OnActivate();
		AnimHelper = new CitizenAnimationHelper(Entity);
	}

    public void Simulate()
    {
        if(Entity.Controller == null) return;

		var turnSpeed = 0.02f;
        var controller = Entity.Controller;
		Rotation rotation;

		// If we're a bot, spin us around 180 degrees
		if ( Entity.Client.IsBot )
			rotation = Entity.ViewAngles.WithYaw( Entity.ViewAngles.yaw + 180f ).ToRotation();
		else
			rotation = Entity.ViewAngles.ToRotation();

		if(Entity.Controller.HasRotation)
		{
			var idealRotation = Rotation.LookAt( rotation.Forward.WithZ( 0 ), Vector3.Up );
			Entity.Rotation = Rotation.Slerp( Entity.Rotation, idealRotation, controller.WishVelocity.Length * Time.Delta * turnSpeed );
			Entity.Rotation = Entity.Rotation.Clamp( idealRotation, 45.0f, out var shuffle ); // lock facing to within 45 degrees of look direction
			AnimHelper.FootShuffle = shuffle;
		}

		if(!Entity.Controller.HasAnimations) return;

		AnimHelper.WithWishVelocity(controller.WishVelocity);
		AnimHelper.WithVelocity(controller.Velocity);
		AnimHelper.WithLookAt( Entity.EyePosition + Entity.EyeRotation.Forward * 100.0f, 1.0f, 1.0f, 0.5f );
		AnimHelper.AimAngle = rotation;
		AnimHelper.DuckLevel = MathX.Lerp( AnimHelper.DuckLevel, controller.HasTag( "ducked" ) ? 1 : 0, Time.Delta * 10.0f );
		AnimHelper.VoiceLevel = (Game.LocalPawn == Entity) ? Voice.Level : Entity.Client.Voice.CurrentLevel;
		AnimHelper.IsGrounded = Entity.GroundEntity != null;
		AnimHelper.IsSitting = controller.HasTag( "sitting" );
		AnimHelper.IsNoclipping = controller.HasTag( "noclip" );
		AnimHelper.IsClimbing = controller.HasTag( "climbing" );
		AnimHelper.IsSwimming = Entity.GetWaterLevel() >= 0.5f;
		AnimHelper.IsWeaponLowered = false;
		AnimHelper.MoveStyle = Input.Down( "walk" ) ? CitizenAnimationHelper.MoveStyles.Walk : CitizenAnimationHelper.MoveStyles.Run;
	

		if ( controller.HasEvent( "jump" ) ) AnimHelper.TriggerJump();
		// if ( ActiveChild != lastWeapon ) animHelper.TriggerDeploy();

		if ( Entity.ActiveChild is BaseCarriable carry )
		{
			carry.SimulateAnimator( AnimHelper );
		}
		else
		{
			AnimHelper.HoldType = CitizenAnimationHelper.HoldTypes.None;
			AnimHelper.AimBodyWeight = 0.5f;
		}
    }

}