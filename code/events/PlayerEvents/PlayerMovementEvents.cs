using Sandbox;

[EventBase]
public class PlayerSpeedUpEvent : EventBase
{
    public PlayerSpeedUpEvent(){
        name = "player_speed_up";
        text = " player(s) will speed up in ";
        type = EventType.Player;
    }

    public override void OnEvent(Entity ent){
        PlatesPlayer ply = ent as PlatesPlayer;
        (ply.Controller as PlatesWalkController).Speed += 0.25f;
    }
}

[EventBase]
public class PlayerSpeedDownEvent : EventBase
{
    public PlayerSpeedDownEvent(){
        name = "player_speed_down";
        text = " player(s) will slow down in ";
        type = EventType.Player;
    }
    
    public override void OnEvent(Entity ent){
        PlatesWalkController ply = (ent as PlatesPlayer).Controller as PlatesWalkController;
        ply.Speed -= 0.25f;
        if(ply.Speed < 0.1f) ply.Speed = 0.1f;
    }
}

[EventBase]
public class PlayerJumpUpEvent : EventBase
{
    public PlayerJumpUpEvent(){
        name = "player_jump_up";
        text = " player(s) will jump 25% higher in ";
        type = EventType.Player;
    }

    public override void OnEvent(Entity ent){
        PlatesPlayer ply = ent as PlatesPlayer;
        (ply.Controller as PlatesWalkController).JumpPower += 0.25f;
    }
}

[EventBase]
public class PlayerJumpDownEvent : EventBase
{
    public PlayerJumpDownEvent(){
        name = "player_jump_down";
        text = " player(s) will jump 25% lower in ";
        type = EventType.Player;
    }

    public override void OnEvent(Entity ent){
        PlatesPlayer ply = ent as PlatesPlayer;
        var wc = (ply.Controller as PlatesWalkController);
        wc.JumpPower -= 0.25f;
        if(wc.JumpPower < 0) wc.JumpPower = 0;
    }
}