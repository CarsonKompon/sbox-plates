using Sandbox;

[EventBase]
public class PlayerInvisibleEvent : EventBase
{
    public PlayerInvisibleEvent(){
        name = "player_invisible";
        text = " player(s) will become invisible in ";
        type = EventType.Player;
    }

    public override void OnEvent(Entity ent){
        (ent as PlatesPlayer).RenderAlpha = 0;
    }
}