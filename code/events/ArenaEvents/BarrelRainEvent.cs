using System.Xml.Schema;
using Sandbox;
using System;

public partial class BarrelRainEvent : PlatesEventAttribute
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

public partial class BarrelRainEnt : Entity
{
    [Net] public RealTimeSince timer {get;set;} = -2*60f;

    public BarrelRainEnt() {}
    public BarrelRainEnt(float time = 2*60){
        PlatesGame.AddEntity(this);
        timer = -time;
    }

    [Event.Tick.Server]
    public void Tick(){
        if(Rand.Int(1,500) == 1){
            var ent = new Prop();
            ent.Scale = 2;
            ent.Position = new Vector3(Rand.Int(-1500,1500), Rand.Int(-1500,1500), 10000);
            ent.Rotation = Rotation.From(new Angles(Rand.Float()*360,Rand.Float()*360,Rand.Float()*360));
            ent.SetModel("models/rust_props/barrels/fuel_barrel.vmdl");
            ent.Name = "Explosive Barrel";
            PlatesGame.AddEntity(ent);
        }
        if(timer >= 0) this.Delete();
    }
}