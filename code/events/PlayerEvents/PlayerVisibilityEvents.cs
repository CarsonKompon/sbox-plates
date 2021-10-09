using Sandbox;

[EventBase]
public class PlayerInvisibleEvent : EventBase
{
    public PlayerInvisibleEvent(){
        name = "player_invisible";
        text = " player(s) will become invisible in ";
        type = EventType.Player;
    }

    public override void OnEvent(Entity ent){
        var ply = (ent as PlatesPlayer);
        ply.RenderColor = ply.RenderColor.WithAlpha(0f);
    }
}

[EventBase]
public class PlayerSkeletonEvent : EventBase
{
    public PlayerSkeletonEvent(){
        name = "player_skeleton";
        text = " player(s) will become a skeleton in ";
        type = EventType.Player;
    }

    public override void OnEvent(Entity ent){
        var ply = (ent as PlatesPlayer);
        ply.RenderColor = ply.RenderColor.WithAlpha(0f);
        new SkeletonDrawingEnt(ent);
    }
}

public class SkeletonDrawingEnt : Entity
{
    public Entity ent;

    public SkeletonDrawingEnt(Entity e){
        ent = e;
        PlatesGame.GameEnts.Add(this);
    }

    [Event.Tick]
    public void Tick(){
        if(IsServer){
            if(!ent.IsValid()) Delete();
            else DebugOverlay.Skeleton(ent, Color.White);
        }
        if(IsClient && ent.IsValid()){
            DebugOverlay.Skeleton(ent, Color.White);
        }
    }

}