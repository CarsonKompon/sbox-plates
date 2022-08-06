using Sandbox;
using System.Linq;

 
public class GravityDownEvent : PlatesEventAttribute
{
    public GravityDownEvent(){
        name = "arena_gravity_down";
        text = "Gravity will lower in ";
        subtext = "Gravity has lowered by 20%";
        minAffected = 1;
        maxAffected = 1;
        type = EventType.Arena;
    }

    public override void OnEvent(){
        foreach(var ply in Entity.All.OfType<PlatesPlayer>()){
            var wc = (ply.Controller as PlatesWalkController);
            wc.Gravity -= 800*0.2f;
            ply.SetGlow( true, Color.Blue );
		}
    }
}

 
public class GravityUpEvent : PlatesEventAttribute
{
    public GravityUpEvent(){
        name = "arena_gravity_up";
        text = "Gravity will increase in ";
        subtext = "Gravity has increased by 20%";
        minAffected = 1;
        maxAffected = 1;
        type = EventType.Arena;
    }

    public override void OnEvent(){
        foreach(var ply in Entity.All.OfType<PlatesPlayer>()){
            var wc = (ply.Controller as PlatesWalkController);
            wc.Gravity += 800*0.2f;
			ply.SetGlow( true, Color.Blue );
		}
    }
}
