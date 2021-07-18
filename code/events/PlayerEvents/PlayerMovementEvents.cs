using Sandbox;
using System;

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

[EventBase]
public class PlayerWalkBackwardsEvent : EventBase
{
    public PlayerWalkBackwardsEvent(){
        name = "player_walk_backwards";
        text = " player(s) will walk backwards in ";
        type = EventType.Player;
    }

    public override void OnEvent(Entity ent){
        PlatesPlayer ply = ent as PlatesPlayer;
        var wc = (ply.Controller as PlatesWalkController);
        wc.InputMultiplier *= -1;
    }
}

[EventBase]
public class PlayerWalkRandomlyEvent : EventBase
{
    public PlayerWalkRandomlyEvent(){
        name = "player_walk_randomly";
        text = " player(s) will walk randomly in ";
        type = EventType.Player;
    }

    public override void OnEvent(Entity ent){
        PlatesPlayer ply = ent as PlatesPlayer;
        var wc = (ply.Controller as PlatesWalkController);
        new WalkRandomlyEnt(wc);
    }
}

public class WalkRandomlyEnt : Entity
{
    Random random = new Random();
    public PlatesWalkController walkController;

    public WalkRandomlyEnt(PlatesWalkController wc){
        walkController = wc;
        PlatesGame.GameEnts.Add(this);
    }

    [Event.Tick]
    public void Tick(){
        if(IsServer){
            if(random.Next(0,100) == 1){
                walkController.ForwardInput = ((float)random.NextDouble() * 2.0f) - 1.0f;
                walkController.SidewaysInput = ((float)random.NextDouble() * 2.0f) - 1.0f;
            }
        }
    }

}