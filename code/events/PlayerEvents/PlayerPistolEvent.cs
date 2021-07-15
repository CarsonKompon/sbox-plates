using Sandbox;


[EventBase]
public class PlayerPistolEvent : EventBase
{
    public PlayerPistolEvent(){
        name = "player_pistol";
        text = " player(s) will get a pistol in ";
        type = EventType.Player;
    }

    public override void OnEvent(Entity ent){
        var ply = (ent as PlatesPlayer);
        ply.Inventory.Add(new Pistol());
    }
}