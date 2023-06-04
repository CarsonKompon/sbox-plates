using Sandbox;
using System;

 
public class PlateBarrelTrapEvent : PlatesEventAttribute
{

    public PlateBarrelTrapEvent(){
        name = "Plate Explosive Barrel";
        command = "plate_barrel_trap";
        text = " plate(s) will receive an explosive barrel in ";
        type = EventType.Plate;
        rarity = EventRarity.Uncommon;
    }

    public override void OnEvent(Plate plate){
        Prop barrel = new Prop();
        Random Rand = new();
        barrel.SetModel("models/rust_props/barrels/fuel_barrel.vmdl");
        barrel.Position = plate.Position + Vector3.Up * 10;
        barrel.Position += Vector3.Left * Rand.Int(-50,50) * plate.Scale;
        barrel.Position += Vector3.Forward * Rand.Int(-50,50) * plate.Scale;
        barrel.SetupPhysicsFromModel(PhysicsMotionType.Dynamic);
        barrel.Name = "Explosive Barrel";
        PlatesGame.AddEntity(barrel);
        plate.PlateEnts.Add(barrel);
    }
}