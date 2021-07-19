using Sandbox;
using System;
using System.Linq;

[EventBase]
public class PlayerSwapEvent : EventBase
{
    Random random = new Random();

    public PlayerSwapEvent(){
        name = "arena_player_swap";
        text = "2 players will swap places in ";
        minAffected = 1;
        maxAffected = 1;
        type = EventType.Arena;
    }

    public override void OnEvent(){
        var ar = Client.All.OrderBy(x => random.NextDouble()).ToArray();
        if(ar.Length < 2) return;
        var ply1 = ar[0];
        var ply2 = ar[1];
        var ply1Pos = ply1.Pawn.Position;
        ply1.Pawn.Position = ply2.Pawn.Position;
        ply2.Pawn.Position = ply1Pos;
        subtext =  ply1.Name + " and " + ply2.Name + " have swapped";
    }
}

[EventBase]
public class PlateSwapEvent : EventBase
{
    Random random = new Random();

    public PlateSwapEvent(){
        name = "arena_plate_swap";
        text = "2 plates will swap places in ";
        minAffected = 1;
        maxAffected = 1;
        type = EventType.Arena;
    }

    public override void OnEvent(){
        var ar = Entity.All.OfType<Plate>().OrderBy(x => random.NextDouble()).ToArray();
        if(ar.Length < 2) return;
        var ply1 = ar[0];
        var ply2 = ar[1];
        var ply1Pos = ply1.Position;
        ply1.Position = ply2.Position;
        ply2.Position = ply1Pos;
        subtext =  ply1.ownerName + " and " + ply2.ownerName + "s plates have swapped";
    }
}