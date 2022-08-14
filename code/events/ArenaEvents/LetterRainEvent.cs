using System.Xml.Schema;
using Sandbox;
using System;

 
public partial class LetterRainEvent : PlatesEventAttribute
{
    public LetterRainEvent(){
        name = "Letter Rain";
        command = "arena_letter_rain";
        text = "Letters will rain from the sky in ";
        subtext = "Letters will rain from the sky for 2 minutes.";
        type = EventType.Arena;
        minAffected = 1;
        maxAffected = 1;
        rarity = EventRarity.Rare;
    }

    public override void OnEvent(){
        new LetterRainEnt(2*60);
    }
}

public partial class LetterRainEnt : Entity
{
    [Net] public RealTimeSince timer {get;set;} = -2*60f;
    //Names of all letter models
    private string[] letters = {
        "ampersand",
        "a_low", "a_up",
        "b_low", "b_up",
        "c_up",
        "g_low", "g_up",
        "l_low", "l_up",
        "m_low", "m_up",
        "p_low", "p_up",
        "t_low", "t_up",
        "w_low",
        "x_up",
        "y_up"
    };

    public LetterRainEnt(){}
    public LetterRainEnt(float time = 2*60){
        PlatesGame.AddEntity(this);
        timer = -time;
    }

    [Event.Tick.Server]
    public void Tick(){
        if(Rand.Int(0,200) == 1){
            var ent = new Prop();
            ent.Name = "Raining Letter";
            ent.Scale = 2;
            ent.Position = new Vector3(Rand.Int(-1500,1500), Rand.Int(-1500,1500), 10000);
            ent.Rotation = Rotation.From(new Angles(Rand.Float()*360,Rand.Float()*360,Rand.Float()*360));
            //ent.Velocity = new Vector3(0,0,-1000000);
            ent.SetModel("models/letters/" + Rand.FromArray(letters) + ".vmdl");
            ent.RenderColor = Color.FromBytes(Rand.Int(0,255),Rand.Int(0,255),Rand.Int(0,255));
            PlatesGame.AddEntity(ent);
        }
        if(timer >= 0) this.Delete();
    }
}