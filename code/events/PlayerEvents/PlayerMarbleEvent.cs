using Sandbox;


//[EventBase]
public class PlayerMarbleEvent : EventBase
{
    public PlayerMarbleEvent(){
        name = "player_marble";
        text = " player(s) will become a marble in ";
        type = EventType.Player;
    }

    public override void OnEvent(Entity ent){
        var ply = (ent as PlatesPlayer);
        ply.SetModel("models/marble.vmdl");
        ply.Controller = new MarbleController();
    }
}

public partial class MarbleController : BasePlayerController
{
    public override void Simulate()
    {
        // get the velocity
        Vector3 targetForward = Input.Rotation.Forward;
        targetForward.z = 0;

        Vector3 vel = (targetForward * Input.Forward) + (Input.Rotation.Left * Input.Left);
        vel = vel.Normal * 400;
        Velocity += vel * Time.Delta;

        // slow the player down
        Velocity = Vector3.Lerp( Velocity, new Vector3( 0f, 0f, Velocity.z ), Time.Delta * 0.6f );
    }
}