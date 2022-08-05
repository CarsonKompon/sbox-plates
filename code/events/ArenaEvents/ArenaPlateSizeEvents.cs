using Sandbox;
using System.Linq;

[PlatesEvent]
public class ArenaPlateGrow10Event : PlatesEventAttribute
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
        foreach(var plate in Entity.All.OfType<Plate>()){
            plate.Grow(0.1f);
			plate.SetGlow( true, Color.Blue );
        }
    }
}

[PlatesEvent]
public class ArenaPlateShrink10Event : PlatesEventAttribute
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
        foreach(var plate in Entity.All.OfType<Plate>()){
            plate.Shrink(0.1f);
			plate.SetGlow( true, Color.Blue );
		}
    }
}

[PlatesEvent]
public class ArenaPlateShrinkHalfEvent : PlatesEventAttribute
{
    public ArenaPlateShrinkHalfEvent(){
        name = "arena_shrink_half";
        text = "All plates will shrink in half in ";
        subtext = "";
        type = EventType.Arena;
        minAffected = 1;
        maxAffected = 1;
    }

    public override void OnEvent(){
        foreach(var plate in Entity.All.OfType<Plate>()){
            plate.SetSize(plate.GetSize()/2);
			plate.SetGlow( true, Color.Blue );
		}
    }
}


[PlatesEvent]
public class ArenaPlateGrowDoubleEvent : PlatesEventAttribute
{
    public ArenaPlateGrowDoubleEvent(){
        name = "arena_grow_double";
        text = "All plates will double in size in ";
        subtext = "";
        type = EventType.Arena;
        minAffected = 1;
        maxAffected = 1;
    }

    public override void OnEvent(){
        foreach(var plate in Entity.All.OfType<Plate>()){
            plate.SetSize(plate.GetSize()*2);
			plate.SetGlow( true, Color.Blue );
		}
    }
}
