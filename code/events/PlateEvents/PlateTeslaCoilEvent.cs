using System.Linq;
using Sandbox;
using System;

[EventBase]
public class PlateTeslaCoilEvent : EventBase
{

    public PlateTeslaCoilEvent(){
        name = "plate_tesla_coil";
        text = " plate(s) will receive a tesla coil in ";
        type = EventType.Plate;
    }

    public override void OnEvent(Plate plate){
        var coil = new TeslaCoilEnt();
        coil.SetModel("models/teslacoil.vmdl");
        coil.Position = plate.Position + Vector3.Up * 10;
        coil.Position += Vector3.Left * Rand.Int(-50,50) * plate.Scale;
        coil.Position += Vector3.Forward * Rand.Int(-50,50) * plate.Scale;
        plate.PlateEnts.Add(coil);
    }
}

public class TeslaCoilEnt : ModelEntity
{

    public Player nearest;

    public TeslaCoilEnt()
    {
        PlatesGame.GameEnts.Add(this);

		SetupPhysicsFromModel(PhysicsMotionType.Dynamic, false);
    }

    [Event.Tick]
    public void Tick(){
        if(IsServer){
            nearest = Entity.All.OfType<Player>().OrderBy( x => Vector3.DistanceBetween( x.Position + x.Rotation.Up * 40, Position + Rotation.Up * 70 ) ).ToArray()[0];
            var distance = Vector3.DistanceBetween( nearest.Position, Position );
            if(distance <= 150){
                DebugOverlay.Line(Position + Rotation.Up * 70, nearest.Position + nearest.Rotation.Up * 40);
                nearest.TakeDamage(DamageInfo.Generic( 0.2f ));
            }
        }else if(IsClient){
            nearest = Entity.All.OfType<Player>().OrderBy( x => Vector3.DistanceBetween( x.Position + x.Rotation.Up * 40, Position + Rotation.Up * 70 ) ).ToArray()[0];
            var distance = Vector3.DistanceBetween( nearest.Position, Position );
            if(distance <= 150){
                DebugOverlay.Line(Position + Rotation.Up * 70, nearest.Position + nearest.Rotation.Up * 40);
                /*
                var part = Particles.Create("particles/tesla.vpcf");
                part.SetEntity(0,nearest);
                part.SetEntity(1,this);
                part.Destroy
                */
            }
        }
    }
}