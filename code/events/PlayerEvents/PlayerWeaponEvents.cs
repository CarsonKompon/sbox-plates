using Sandbox;


[EventBase]
public class PlayerPistolEvent : EventBase
{
    public PlayerPistolEvent(){
        name = "player_pistol";
        text = " player(s) will get a Deagle with 5 shots in ";
        type = EventType.Player;
    }

    public override void OnEvent(Entity ent){
        var ply = (ent as PlatesPlayer);
        ply.Inventory.Add(new SWB_CSS.Deagle());
    }
}

[EventBase]
public class PlayerRocketLauncherEvent : EventBase
{
    public PlayerRocketLauncherEvent(){
        name = "player_rpg";
        text = " player(s) will get a Rocket Launcher in ";
        type = EventType.Player;
    }

    public override void OnEvent(Entity ent){
        var ply = (ent as PlatesPlayer);
        ply.Inventory.Add(new SWB_EXPLOSIVES.RPG7());
    }
}