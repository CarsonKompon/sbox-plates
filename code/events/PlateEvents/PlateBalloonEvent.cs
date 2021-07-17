using Sandbox;
using System;

//[EventBase]
public class PlateBalloonEvent : EventBase
{

    Random random = new Random();

    public PlateBalloonEvent(){
        name = "plate_balloons";
        text = " plate(s) will be attached to balloons in ";
        type = EventType.Plate;
    }

    public override void OnEvent(Plate plate){
        
        var startPos = plate.Position + Vector3.Up * 120;
        var dir = Vector3.Down;

        var tr = Trace.Ray( startPos, startPos + dir * 200 )
            .WithTag("plate")
            .Run();

        for(var i=0;i<30;i++){

            if (tr.Entity.IsValid() && !(tr.Entity is BalloonEntity)){

                var ent = new BalloonEntity
                {
                    Position = tr.EndPos,
                };

                ent.SetModel( "models/citizen_props/balloonregular01.vmdl" );
                ent.PhysicsBody.GravityScale = -0.2f;
                ent.RenderColor = Color.Random;

                var rope = Particles.Create( "particles/rope.vpcf" );
                rope.SetEntity( 0, ent );

                var attachEnt = tr.Body.IsValid() ? tr.Body.Entity : tr.Entity;
                var attachLocalPos = tr.Body.Transform.PointToLocal( tr.EndPos );

                if ( attachEnt.IsWorld )
                {
                    rope.SetPosition( 1, attachLocalPos );
                }
                else
                {
                    rope.SetEntityBone( 1, attachEnt, tr.Bone, new Transform( attachLocalPos ) );
                }

                ent.AttachRope = rope;

                ent.AttachJoint = PhysicsJoint.Spring
                    .From( ent.PhysicsBody )
                    .To( tr.Body )
                    .WithPivot( tr.EndPos )
                    .WithFrequency( 5.0f )
                    .WithDampingRatio( 0.7f )
                    .WithReferenceMass( 0 )
                    .WithMinRestLength( 0 )
                    .WithMaxRestLength( 100 )
                    .WithCollisionsEnabled()
                    .Create();

            }

        }

        plate.SetupPhysicsFromModel(PhysicsMotionType.Dynamic);
    }
}


