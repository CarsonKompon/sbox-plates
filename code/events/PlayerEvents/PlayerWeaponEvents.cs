using Sandbox;


[EventBase]
public class PlayerPistolEvent : EventBase
{
    public PlayerPistolEvent(){
        name = "player_pistol";
        text = " player(s) will get a Deagle with 6 shots in ";
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

[EventBase]
public class PlayerKnifeEvent : EventBase
{
    public PlayerKnifeEvent(){
        name = "player_knife";
        text = " player(s) will get a Knife in ";
        type = EventType.Player;
        minAffected = 1;
        maxAffected = 6;
    }

    public override void OnEvent(Entity ent){
        var ply = (ent as PlatesPlayer);
        ply.Inventory.Add(new SWB_CSS.Knife());
    }
}