using Sandbox;

 
public class PlayerGrowEvent : PlatesEventAttribute
{
    public PlayerGrowEvent(){
        name = "Player Grows";
        command = "player_grow";
        text = " player(s) will grow in ";
        type = EventType.Player;
        rarity = EventRarity.Uncommon;
    }

    public override void OnEvent(Entity ent){
        ent.Scale += 0.1f;
    }
}

 
public class PlayerShrinkEvent : PlatesEventAttribute
{
    public PlayerShrinkEvent(){
        name = "Player Shrinks";
        command = "player_shrink";
        text = " player(s) will shrink in ";
        type = EventType.Player;
        rarity = EventRarity.Uncommon;
    }
    
    public override void OnEvent(Entity ent){
        ent.Scale -= 0.1f;
        if(ent.Scale <= 0.2f) ent.Scale = 0.2f;
    }
}

