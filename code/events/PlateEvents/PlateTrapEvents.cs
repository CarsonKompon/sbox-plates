using Sandbox;
using System;

[PlatesEvent]
public class PlateBarrelTrapEvent : PlatesEventAttribute
{

    public PlateBarrelTrapEvent(){
        name = "plate_barrel_trap";
        text = " plate(s) will receive an explosive barrel in ";
        type = EventType.Plate;
    }

    public override void OnEvent(Plate plate){
        Prop barrel = new Prop();
        barrel.SetModel("models/rust_props/barrels/fuel_barrel.vmdl");
        barrel.Position = plate.Position + Vector3.Up * 10;
        barrel.Position += Vector3.Left * Rand.Int(-50,50) * plate.Scale;
        barrel.Position += Vector3.Forward * Rand.Int(-50,50) * plate.Scale;
        barrel.SetupPhysicsFromModel(PhysicsMotionType.Dynamic);
        PlatesGame.AddEntity(barrel);
        plate.PlateEnts.Add(barrel);
    }
}