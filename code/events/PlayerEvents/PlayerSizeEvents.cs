using Sandbox;

[EventBase]
public class PlayerGrowEvent : EventBase
{
    public PlayerGrowEvent(){
        name = "player_grow";
        text = " player(s) will grow in ";
        type = EventType.Player;
    }

    public override void OnEvent(Entity ent){
        ent.Scale += 0.1f;
    }
}

[EventBase]
public class PlayerShrinkEvent : EventBase
{
    public PlayerShrinkEvent(){
        name = "player_shrink";
        text = " player(s) will shrink in ";
        type = EventType.Player;
    }
    
    public override void OnEvent(Entity ent){
        ent.Scale -= 0.1f;
        if(ent.Scale <= 0.2f) ent.Scale = 0.2f;
    }
}

