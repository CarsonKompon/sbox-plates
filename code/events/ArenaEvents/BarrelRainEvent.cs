using System.Xml.Schema;
using Sandbox;
using System;

[EventBase]
public partial class BarrelRainEvent : EventBase
{
    public BarrelRainEvent(){
        name = "arena_barrel_rain";
        text = "Explosive Barrels will rain from the sky in ";
        subtext = "Explosive Barrels will rain from the sky for 2 minutes.";
        type = EventType.Arena;
        minAffected = 1;
        maxAffected = 1;
    }

    public override void OnEvent(){
        new BarrelRainEnt(2*60);
    }
}

public class BarrelRainEnt : Entity
{
    Random random = new Random();
    public float timer = 2*60;

    public BarrelRainEnt(float time = 2*60){
        PlatesGame.GameEnts.Add(this);
        timer = time;
    }

    [Event.Tick]
    public void Tick(){
        if(timer > 0){
            timer -= 1.0f/60.0f;
            if(random.Next(0,500) == 1){
                var ent = new Prop();
                ent.Scale = 2;
                ent.Position = new Vector3(random.Next(-1500,1500), random.Next(-1500,1500), 10000);
                ent.Rotation = Rotation.From(new Angles((float)random.NextDouble()*360,(float)random.NextDouble()*360,(float)random.NextDouble()*360));
                ent.SetModel("models/rust_props/barrels/fuel_barrel.vmdl");
            }
            if(timer <= 0) this.Delete();
        }
    }
}