using Sandbox;
using System;
using System.Linq;

 
public class PlayerSwapEvent : PlatesEventAttribute
{

    public PlayerSwapEvent(){
        name = "2 Players Swap Places";
        command = "arena_player_swap";
        text = "2 players will swap places in ";
        minAffected = 1;
        maxAffected = 1;
        type = EventType.Arena;
        rarity = EventRarity.Uncommon;
    }

    public override void OnEvent(){
        Random Rand = new();
        var ar = PlatesGame.GameClients.OrderBy(x => Rand.Double(0f,1f)).ToArray();
        if(ar.Length < 2) return;
        var ply1 = ar[0];
        var ply2 = ar[1];
        var ply1Pos = ply1.Pawn.Position;
        ply1.Pawn.Position = ply2.Pawn.Position;
        ply2.Pawn.Position = ply1Pos;
        subtext =  ply1.Name + " and " + ply2.Name + " have swapped";
        (ply1.Pawn as PlatesPlayer).SetGlow( true, Color.Blue );
		(ply2.Pawn as PlatesPlayer).SetGlow( true, Color.Blue );
	}
}

 
public class PlateSwapEvent : PlatesEventAttribute
{
    public PlateSwapEvent(){
        name = "2 Plates Swap Places";
        command = "arena_plate_swap";
        text = "2 plates will swap places in ";
        minAffected = 1;
        maxAffected = 1;
        type = EventType.Arena;
        rarity = EventRarity.Uncommon;
    }

    public override void OnEvent(){
        Random Rand = new();
        var ar = Entity.All.OfType<Plate>().OrderBy(x => Rand.Double(0f,1f)).ToArray();
        if(ar.Length < 2) return;
        var ply1 = ar[0];
        var ply2 = ar[1];
        var ply1Pos = ply1.Position;
        ply1.Position = ply2.Position;
        ply2.Position = ply1Pos;
        subtext =  ply1.ownerName + " and " + ply2.ownerName + "s plates have swapped";
		ply1.SetGlow( true, Color.Blue );
        ply2.SetGlow( true, Color.Blue );
	}
}
