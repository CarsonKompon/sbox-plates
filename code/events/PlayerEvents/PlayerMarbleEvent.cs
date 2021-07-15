using Sandbox;


//[EventBase]
public class PlayerMarbleEvent : EventBase
{
    public PlayerMarbleEvent(){
        name = "player_marble";
        text = " player(s) will become a marble in ";
        type = EventType.Player;
    }

    public override void OnEvent(Entity ent){
        //var ply = (ent as PlatesPlayer);
        //.SetModel("models/marble.vmdl");
    }
}