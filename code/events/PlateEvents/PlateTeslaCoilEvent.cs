using System.Linq;
using Sandbox;
using System;

namespace Plates;
 
public class PlateTeslaCoilEvent : PlatesEvent
{

    public PlateTeslaCoilEvent(){
        name = "Plate Tesla Coil";
        command = "plate_tesla_coil";
        text = " plate(s) will receive a tesla coil in ";
        type = EventType.Plate;
        rarity = EventRarity.Rare;
    }

    public override void OnEvent(Plate plate){
        var coil = new TeslaCoilEnt();
        coil.Position = plate.Position + (Vector3.Up * (plate.toScale.z/2f));
        var size = plate.GetSize();
        Random Rand = new();
        coil.Position += Vector3.Left * Rand.Int(-50,50) * size;
        coil.Position += Vector3.Forward * Rand.Int(-50,50) * size;
        plate.AddEntity(coil, true);
    }
}

public class TeslaCoilEnt : ModelEntity
{

    public Player nearest;
    private DamageInfo damage;

    public TeslaCoilEnt() {}

    public override void Spawn()
    {
        SetModel("models/teslacoil.vmdl");
		SetupPhysicsFromModel(PhysicsMotionType.Dynamic);
        Name = "Tesla Coil";
        damage = new DamageInfo().WithAttacker(this);
        damage.Damage = 0.2f;
    }

    [GameEvent.Tick]
    public void Tick(){
        if(Game.IsServer){
            nearest = Entity.All.OfType<Player>().OrderBy( x => Vector3.DistanceBetween( x.Position + x.Rotation.Up * 40, Position + Rotation.Up * 70 ) ).ToArray()[0];
            var distance = Vector3.DistanceBetween( nearest.Position, Position );
            if(distance <= 150){
                DebugOverlay.Line(Position + Rotation.Up * 70, nearest.Position + nearest.Rotation.Up * 40);
                nearest.TakeDamage(damage);
            }
        }else if(Game.IsClient){
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