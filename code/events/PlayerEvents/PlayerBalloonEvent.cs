using Sandbox;
using System;

//[EventBase]
public class PlayerBalloonEvent : EventBase
{

    public PlayerBalloonEvent(){
        name = "player_balloons";
        text = " player(s) will be attached to balloons in ";
        type = EventType.Player;
    }

    public override void OnEvent(Entity ply){
        
        var startPos = ply.Position + Vector3.Up * 120;
        var dir = Vector3.Down;

        var tr = Trace.Ray( startPos, startPos + dir * 200 )
            .Run();

        for(var i=0;i<30;i++){

            if (tr.Entity.IsValid() && !(tr.Entity is BalloonEntity)){

                var ent = new BalloonEntity
                {
                    Position = tr.EndPosition,
                };

                ent.SetModel( "models/citizen_props/balloonregular01.vmdl" );
                ent.PhysicsBody.GravityScale = -0.2f;
                ent.RenderColor = Color.Random;

                var rope = Particles.Create( "particles/rope.vpcf" );
                rope.SetEntity( 0, ent );

                var attachEnt = tr.Body.IsValid() ? tr.Body.GetEntity() : tr.Entity;
                var attachLocalPos = tr.Body.Transform.PointToLocal( tr.EndPosition );

                if ( attachEnt.IsWorld )
                {
                    rope.SetPosition( 1, attachLocalPos );
                }
                else
                {
                    rope.SetEntityBone( 1, attachEnt, tr.Bone, new Transform( attachLocalPos ) );
                }

                ent.AttachRope = rope;

				var spring = PhysicsJoint.CreateLength( ent.PhysicsBody, PhysicsPoint.World( tr.Body, tr.EndPosition ), 100 );
				spring.SpringLinear = new( 5, 0.7f );
				spring.Collisions = true;
				spring.EnableAngularConstraint = false;
				spring.OnBreak += () =>
				{
					rope?.Destroy( true );
					spring.Remove();
				};
            }

        }
    }
}


