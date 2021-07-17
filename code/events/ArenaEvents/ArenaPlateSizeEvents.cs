using Sandbox;
using System.Linq;

[EventBase]
public class ArenaPlateGrow10Event : EventBase
{
    public ArenaPlateGrow10Event(){
        name = "arena_grow_10";
        text = "All plates will grow 10% in ";
        subtext = "";
        type = EventType.Arena;
        minAffected = 1;
        maxAffected = 1;
    }

    public override void OnEvent(){
        foreach(var plate in Entity.All.OfType<Plate>())
            plate.toScale += 0.10f;
    }
}

[EventBase]
public class ArenaPlateShrink10Event : EventBase
{
    public ArenaPlateShrink10Event(){
        name = "arena_shrink_10";
        text = "All plates will shrink 10% in ";
        subtext = "";
        type = EventType.Arena;
        minAffected = 1;
        maxAffected = 1;
    }

    public override void OnEvent(){
        foreach(var plate in Entity.All.OfType<Plate>())
            plate.toScale -= 0.10f;
    }
}