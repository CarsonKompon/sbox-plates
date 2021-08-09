using Sandbox;


[EventBase]
public class PlayerScreenBlurEvent : EventBase
{
    public PlayerScreenBlurEvent(){
        name = "player_blur";
        text = " player(s) will have their vision blurred in ";
        type = EventType.Player;
    }

    public override void OnEvent(Entity ent){
        var ply = (ent as PlatesPlayer);
        ply.BlurFX = true;
    }
}