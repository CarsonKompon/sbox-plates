
using System.Numerics;
using System;
namespace Sandbox
{
	public class PlatesCamera : Camera
	{
		[ConVar.Replicated]
		public static bool thirdperson_orbit { get; set; } = false;

		[ConVar.Replicated]
		public static bool thirdperson_collision { get; set; } = true;

		private Angles orbitAngles;
		private float orbitDistance = 150;
		private float thirdDistance = 130.0f;
		private float fov = 70;

		public override void Update()
		{
			var pawn = Local.Pawn as AnimEntity;
			var client = Local.Client;

			if ( pawn == null )
				return;

			Pos = pawn.Position;
			Vector3 targetPos;

			var center = pawn.Position + (Vector3.Up * 64 * pawn.Scale);

			if ( thirdperson_orbit )
			{
				Pos += Vector3.Up * (pawn.CollisionBounds.Center.z * pawn.Scale);
				Rot = Rotation.From( orbitAngles );

				targetPos = Pos + Rot.Backward * orbitDistance;
			}
			else
			{
				Pos = center;
				Rot = Rotation.FromAxis( Vector3.Up, 2 ) * Input.Rotation;

				if ( Input.MouseWheel != 0 ){
					thirdDistance -= Input.MouseWheel*3;
					thirdDistance = Math.Clamp(thirdDistance, 60, 220*2);
				}

				float distance = thirdDistance * pawn.Scale;

				targetPos = Pos + Input.Rotation.Right * ((pawn.CollisionBounds.Maxs.x + 15) * pawn.Scale);
				targetPos += Input.Rotation.Forward * -distance;
			}

			if ( thirdperson_collision )
			{
				var tr = Trace.Ray( Pos, targetPos )
					.Ignore( pawn )
					.Radius( 8 )
					.Run();

				Pos = tr.EndPos;
			}
			else
			{
				Pos = targetPos;
			}

			if(Input.Down(InputButton.Attack2)){
				fov = MathC.Lerp(fov,40,0.125f);
			}else{
				fov = MathC.Lerp(fov,70,0.125f);
			}

			FieldOfView = fov;

			Viewer = null;
		}

		public override void BuildInput( InputBuilder input )
		{
			if ( thirdperson_orbit && input.Down( InputButton.Walk ) )
			{
				if ( input.Down( InputButton.Attack1 ) )
				{
					orbitDistance += input.AnalogLook.pitch;
					orbitDistance = orbitDistance.Clamp( 0, 1000 );
				}
				else
				{
					orbitAngles.yaw += input.AnalogLook.yaw;
					orbitAngles.pitch += input.AnalogLook.pitch;
					orbitAngles = orbitAngles.Normal;
					orbitAngles.pitch = orbitAngles.pitch.Clamp( -89, 89 );
				}

				input.AnalogLook = Angles.Zero;

				input.Clear();
				input.StopProcessing = true;
			}

			base.BuildInput( input );
		}
	}
}
