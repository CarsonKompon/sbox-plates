using System.Linq;
using Sandbox;
using System;

 
public class PlateTeslaCoilEvent : PlatesEventAttribute
{

    public PlateTeslaCoilEvent(){
        name = "plate_tesla_coil";
        text = " plate(s) will receive a tesla coil in ";
        type = EventType.Plate;
    }

    public override void OnEvent(Plate plate){
        var coil = new TeslaCoilEnt();
        coil.Position = plate.Position + (Vector3.Up * (plate.toScale.z/2f));
        var size = plate.GetSize();
        coil.Position += Vector3.Left * Rand.Int(-50,50) * size;
        coil.Position += Vector3.Forward * Rand.Int(-50,50) * size;
        plate.AddEntity(coil, true);
    }
}

public class TeslaCoilEnt : ModelEntity
{

    public Player nearest;

    public TeslaCoilEnt() {}

    public override void Spawn()
    {
        SetModel("models/teslacoil.vmdl");
		SetupPhysicsFromModel(PhysicsMotionType.Dynamic);
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