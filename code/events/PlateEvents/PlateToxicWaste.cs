using System.Linq;
using Sandbox;
using System;

 
public class PlateToxicWasteEvent : PlatesEventAttribute
{

    public PlateToxicWasteEvent(){
        name = "Plate Toxic Waste";
        command = "plate_toxic_waste";
        text = " plate(s) will receive a barrel of toxic waste in ";
        type = EventType.Plate;
        rarity = EventRarity.Rare;
    }

    public override void OnEvent(Plate plate){
        var coil = new ToxicWasteEnt();
        coil.Position = plate.Position + (Vector3.Up * (plate.toScale.z/2f));
        var size = plate.GetSize();
        Random Rand = new();
        coil.Position += Vector3.Left * Rand.Int(-50,50) * size;
        coil.Position += Vector3.Forward * Rand.Int(-50,50) * size;
        plate.AddEntity(coil, true);
    }
}

public class ToxicWasteEnt : ModelEntity
{

    public Player nearest;
    private DamageInfo damage;

    public ToxicWasteEnt() {}

    public override void Spawn()
    {
        SetModel("models/toxic_waste.vmdl");
		SetupPhysicsFromModel(PhysicsMotionType.Dynamic);
        RenderColor = Color.Yellow;
        Name = "Toxic Waste";
        damage = new DamageInfo().WithAttacker(this);
        damage.Damage = 0.1f;
    }

    [Event.Tick.Server]
    public void Tick(){
        var part = Particles.Create("particles/stinky.vpcf");
        part.SetPosition(0, Position + Vector3.Up*20);
        nearest = Entity.All.OfType<Player>().OrderBy( x => Vector3.DistanceBetween( x.Position + x.Rotation.Up * 40, Position + Rotation.Up * 70 ) ).ToArray()[0];
        var distance = Vector3.DistanceBetween( nearest.Position, Position );
        if(distance <= 80){
            nearest.TakeDamage(damage);
        }
    }
}