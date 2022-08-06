using Sandbox;

 
public class PlayerInvisibleEvent : PlatesEventAttribute
{
    public PlayerInvisibleEvent(){
        name = "player_invisible";
        text = " player(s) will become invisible in ";
        type = EventType.Player;
    }

    public override void OnEvent(Entity ent){
        if(ent is PlatesPlayer ply)
        {
            ply.RenderColor = ply.RenderColor.WithAlpha(0f);
        }
    }
}

public class PlayerGhostEvent : PlatesEventAttribute
{
    public PlayerGhostEvent(){
        name = "player_ghost";
        text = " player(s) will become ghost-like in ";
        type = EventType.Player;
    }

	public override void OnEvent(Entity ent)
	{
		if(ent is PlatesPlayer ply)
        {
            ply.RenderColor = ply.RenderColor.WithAlpha(0.5f);
        }
	}
}