using Sandbox;
using System;

 
public class PlayerSpeedUpEvent : PlatesEventAttribute
{
    public PlayerSpeedUpEvent(){
        name = "player_speed_up";
        text = " player(s) will speed up in ";
        type = EventType.Player;
    }

    public override void OnEvent(Entity ent){
        if(ent is PlatesPlayer ply && ply.Controller is PlatesWalkController wc)
        {
            wc.Speed += 0.25f;
        }
    }
}

 
public class PlayerSpeedDownEvent : PlatesEventAttribute
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

 
public class PlayerJumpUpEvent : PlatesEventAttribute
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

 
public class PlayerJumpDownEvent : PlatesEventAttribute
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

 
public class PlayerWalkBackwardsEvent : PlatesEventAttribute
{
    public PlayerWalkBackwardsEvent(){
        name = "player_walk_backwards";
        text = " player(s) will have their controls reversed in ";
        type = EventType.Player;
    }

    public override void OnEvent(Entity ent){
        PlatesPlayer ply = ent as PlatesPlayer;
        var wc = (ply.Controller as PlatesWalkController);
        wc.InputMultiplier *= -1;
    }
}

 
public class PlayerWalkRandomlyEvent : PlatesEventAttribute
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
    public PlatesWalkController walkController;

    public WalkRandomlyEnt(){}
    public WalkRandomlyEnt(PlatesWalkController wc){
        walkController = wc;
        PlatesGame.AddEntity(this);
    }

    [Event.Tick.Server]
    public void Tick(){
        if(Rand.Int(1,100) == 1){
            walkController.ForwardInput = (Rand.Float() * 2.0f) - 1.0f;
            walkController.SidewaysInput = (Rand.Float() * 2.0f) - 1.0f;
        }
    }

}