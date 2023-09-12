using Sandbox;
using System;

namespace Plates;
 
public class PlayerSpeedUpEvent : PlatesEvent
{
    public PlayerSpeedUpEvent(){
        name = "Player Speeds Up";
        command = "player_speed_up";
        text = " player(s) will speed up in ";
        type = EventType.Player;
        rarity = EventRarity.Uncommon;
    }

    public override void OnEvent(Entity ent){
        if(ent is Player ply && ply.Controller is WalkController wc)
        {
            wc.Speed += 0.25f;
        }
    }
}

 
public class PlayerSpeedDownEvent : PlatesEvent
{
    public PlayerSpeedDownEvent(){
        name = "Player Speeds Down";
        command = "player_speed_down";
        text = " player(s) will slow down in ";
        type = EventType.Player;
        rarity = EventRarity.Uncommon;
    }
    
    public override void OnEvent(Entity ent){
        WalkController ply = (ent as Player).Controller as WalkController;
        ply.Speed -= 0.25f;
        if(ply.Speed < 0.1f) ply.Speed = 0.1f;
    }
}

 
public class PlayerJumpUpEvent : PlatesEvent
{
    public PlayerJumpUpEvent(){
        name = "Player Jumps Higher";
        command = "player_jump_up";
        text = " player(s) will jump 25% higher in ";
        type = EventType.Player;
        rarity = EventRarity.Uncommon;
    }

    public override void OnEvent(Entity ent){
        Player ply = ent as Player;
        (ply.Controller as WalkController).JumpPower += 0.25f;
    }
}

 
public class PlayerJumpDownEvent : PlatesEvent
{
    public PlayerJumpDownEvent(){
        name = "Player Jumps Lower";
        command = "player_jump_down";
        text = " player(s) will jump 25% lower in ";
        type = EventType.Player;
        rarity = EventRarity.Uncommon;
    }

    public override void OnEvent(Entity ent){
        Player ply = ent as Player;
        var wc = (ply.Controller as WalkController);
        wc.JumpPower -= 0.25f;
        if(wc.JumpPower < 0) wc.JumpPower = 0;
    }
}

 
public class PlayerWalkBackwardsEvent : PlatesEvent
{
    public PlayerWalkBackwardsEvent(){
        name = "Player Reversed Controls";
        command = "player_walk_backwards";
        text = " player(s) will have their controls reversed in ";
        type = EventType.Player;
        rarity = EventRarity.Rare;
    }

    public override void OnEvent(Entity ent){
        Player ply = ent as Player;
        var wc = (ply.Controller as WalkController);
        wc.InputMultiplier *= -1;
    }
}

 
public class PlayerWalkRandomlyEvent : PlatesEvent
{
    public PlayerWalkRandomlyEvent(){
        name = "Player Walks Randomly";
        command = "player_walk_randomly";
        text = " player(s) will walk randomly in ";
        type = EventType.Player;
        rarity = EventRarity.Uncommon;
    }

    public override void OnEvent(Entity ent){
        Player ply = ent as Player;
        var wc = (ply.Controller as WalkController);
        new WalkRandomlyEnt(wc);
    }
}

public class WalkRandomlyEnt : Entity
{
    public WalkController walkController;

    public WalkRandomlyEnt(){}
    public WalkRandomlyEnt(WalkController wc){
        walkController = wc;
        PlatesGame.Current.AddEntity(this);
    }

    [GameEvent.Tick.Server]
    public void Tick(){
        Random Rand = new();
        if(Rand.Int(1,100) == 1){
            walkController.ForwardInput = (Rand.Float() * 2.0f) - 1.0f;
            walkController.SidewaysInput = (Rand.Float() * 2.0f) - 1.0f;
        }
    }

}