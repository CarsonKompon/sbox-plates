using Sandbox;

public partial class PlatesWalkController : WalkController
{
	[Net] public float Speed { get; set; } = 2;
	[Net] public float JumpPower { get; set; } = 1;
	[Net] public float InputMultiplier { get; set; } = 1;
	[Net] public float ForwardInput { get; set; } = 0;
	[Net] public float SidewaysInput { get; set; } = 0;

	public override float GetWishSpeed()
	{
		var ws = Duck.GetWishSpeed();
		if ( ws >= 0 ) return ws * Speed;

		if ( Input.Down( InputButton.Run ) ) return SprintSpeed * Speed;
		if ( Input.Down( InputButton.Walk ) ) return WalkSpeed * Speed;

		return DefaultSpeed * Speed;
	}

	public override void CheckJumpButton()
	{
		if ( Swimming )
		{
			ClearGroundEntity();
			Velocity = Velocity.WithZ( 100 );
			return;
		}

		if ( GroundEntity == null ) return;

		ClearGroundEntity();

		float flGroundFactor = 1.0f;
		float flMul = 268.3281572999747f * 1.2f * JumpPower;
		if ( Duck.IsActive ) flMul *= 0.8f;

		Velocity = Velocity.WithZ( Velocity.z + flMul * flGroundFactor );
		Velocity -= new Vector3( 0, 0, Gravity * 0.5f ) * Time.Delta;

		AddEvent( "jump" );
	}

	public override void Simulate()
	{
		EyeLocalPosition = Vector3.Up * (EyeHeight * Pawn.Scale);
		UpdateBBox();

		EyeLocalPosition += TraceOffset;
		EyeRotation = Input.Rotation;

		if ( Unstuck.TestAndFix() ) return;

		CheckLadder();
		Swimming = Pawn.WaterLevel > 0.6f;

		if ( !Swimming )
		{
			Velocity -= new Vector3( 0, 0, Gravity * 0.5f ) * Time.Delta;
			Velocity += new Vector3( 0, 0, BaseVelocity.z ) * Time.Delta;
			BaseVelocity = BaseVelocity.WithZ( 0 );
		}

		if ( AutoJump ? Input.Down( InputButton.Jump ) : Input.Pressed( InputButton.Jump ) )
		{
			CheckJumpButton();
		}

		bool bStartOnGround = GroundEntity != null;
		if ( bStartOnGround )
		{
			Velocity = Velocity.WithZ( 0 );

			if ( GroundEntity != null )
			{
				ApplyFriction( GroundFriction * SurfaceFriction );
			}
		}

		WishVelocity = new Vector3( ForwardInput + InputMultiplier * Input.Forward, SidewaysInput + InputMultiplier * Input.Left, 0 );

		ForwardInput = MathX.Lerp( ForwardInput, 0, 0.125f );
		SidewaysInput = MathX.Lerp( SidewaysInput, 0, 0.125f );

		var inSpeed = WishVelocity.Length.Clamp( 0, 1 );
		WishVelocity *= Input.Rotation;

		if ( !Swimming ) WishVelocity = WishVelocity.WithZ( 0 );

		WishVelocity = WishVelocity.Normal * inSpeed;
		WishVelocity *= GetWishSpeed();

		Duck.PreTick();

		bool bStayOnGround = false;
		if ( Swimming )
		{
			ApplyFriction( 1 );
			WaterMove();
		}
		else if ( GroundEntity != null )
		{
			bStayOnGround = true;
			WalkMove();
		}
		else
		{
			AirMove();
		}

		CategorizePosition( bStayOnGround );

		if ( !Swimming )
		{
			Velocity -= new Vector3( 0, 0, Gravity * 0.5f ) * Time.Delta;
		}

		if ( GroundEntity != null )
		{
			Velocity = Velocity.WithZ( 0 );
		}
	}

}