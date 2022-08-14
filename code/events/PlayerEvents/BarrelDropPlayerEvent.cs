using Sandbox;
using System;

 
public class BarrelDropPlayerEvent : PlatesEventAttribute
{

    public BarrelDropPlayerEvent(){
        name = "Explosive Barrel Drops On Player";
        command = "player_barrel_drop";
        text = " player(s) will have an Explosive Barrel dropped on them in ";
        type = EventType.Player;
    }

    public override void OnEvent(Entity ent){
        var barrel = new Prop();
        barrel.Position = new Vector3(ent.Position.x+Rand.Int(-60,60), ent.Position.y+Rand.Int(-60,60), 10000);
        barrel.Rotation = Rotation.From(new Angles(Rand.Float()*360,Rand.Float()*360,Rand.Float()*360));
        barrel.AngularVelocity = new Angles(Rand.Float()*5,Rand.Float()*5,Rand.Float()*5) * Rand.FromArray(new int[] {1,-1});
        barrel.SetModel("models/rust_props/barrels/fuel_barrel.vmdl");
        PlatesGame.AddEntity(barrel);
    }
}


