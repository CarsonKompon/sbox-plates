using System.Xml.Schema;
using Sandbox;
using System;

[EventBase]
public partial class LetterRainEvent : EventBase
{
    public LetterRainEvent(){
        name = "arena_letter_rain";
        text = "Letters will rain from the sky in ";
        subtext = "Letters will rain from the sky for 2 minutes.";
        type = EventType.Arena;
        minAffected = 1;
        maxAffected = 1;
    }

    public override void OnEvent(){
        new LetterRainEnt(2*60);
    }
}

public class LetterRainEnt : Entity
{
    Random random = new Random();
    public float timer = 2*60;

    public LetterRainEnt(float time = 2*60){
        PlatesGame.GameEnts.Add(this);
        timer = time;
    }

    [Event.Tick]
    public void Tick(){
        if(timer > 0){
            timer -= 1.0f/60.0f;
            if(random.Next(0,40) == 1){
                var ent = new Prop();
                ent.Scale = 2;
                ent.Position = new Vector3(random.Next(-1500,1500), random.Next(-1500,1500), 10000);
                ent.Rotation = Rotation.From(new Angles((float)random.NextDouble()*360,(float)random.NextDouble()*360,(float)random.NextDouble()*360));
                ent.Velocity = new Vector3(0,0,-1000000);
                ent.SetModel("models/letters/g_low.vmdl");
                ent.RenderColor = Color.FromBytes(random.Next(0,255),random.Next(0,255),random.Next(0,255));
                PlatesGame.GameEnts.Add(ent);
            }
            if(timer <= 0) this.Delete();
        }
    }
}