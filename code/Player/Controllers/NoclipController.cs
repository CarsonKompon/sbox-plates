using Sandbox;

namespace Plates
{
	[Library]
    class NoclipController : PawnController
	{
		public override void Simulate()
		{
			var fwd = Entity.InputDirection.x.Clamp( -1f, 1f );
			var left = Entity.InputDirection.y.Clamp( -1f, 1f );
			var rotation = Entity.ViewAngles.ToRotation();

			var vel = (rotation.Forward * fwd) + (rotation.Left * left);

			if ( Input.Down( "jump" ) )
			{
				vel += Vector3.Up * 1;
			}

			vel = vel.Normal * 2000;

			if ( Input.Down( "run" ) )
				vel *= 5.0f;

			if ( Input.Down( "crouch" ) )
				vel *= 0.2f;

			Velocity += vel * Time.Delta;

			if ( Velocity.LengthSquared > 0.01f )
			{
				Position += Velocity * Time.Delta;
			}

			Velocity = Velocity.Approach( 0, Velocity.Length * Time.Delta * 5.0f );

			EyeRotation = rotation;
			WishVelocity = Velocity;
			GroundEntity = null;
			BaseVelocity = Vector3.Zero;

			SetTag( "noclip" );
		}

	}
}
