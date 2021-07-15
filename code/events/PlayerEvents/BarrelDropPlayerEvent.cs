using Sandbox;
using System;

[EventBase]
public class BarrelDropPlayerEvent : EventBase
{

    Random random = new Random();

    public BarrelDropPlayerEvent(){
        name = "player_barrel_drop";
        text = " player(s) will have an Explosive Barrel dropped on them in ";
        type = EventType.Player;
    }

    public override void OnEvent(Entity ent){
        var barrel = new Prop();
        barrel.Position = new Vector3(ent.Position.x+random.Next(-60,60), ent.Position.y+random.Next(-60,60), 10000);
        barrel.Rotation = Rotation.From(new Angles((float)random.NextDouble()*360,(float)random.NextDouble()*360,(float)random.NextDouble()*360));
        barrel.SetModel("models/rust_props/barrels/fuel_barrel.vmdl");
    }
}


