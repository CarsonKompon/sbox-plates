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
        (ent as PlatesPlayer).RenderAlpha = 0;
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
        (ent as PlatesPlayer).RenderAlpha = 0;
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
        Log.Info("fuck");
        if(IsServer){
            if(!ent.IsValid()) Delete();
            else DebugOverlay.Skeleton(ent, Color.White);
        }
        if(IsClient && ent.IsValid()){
            DebugOverlay.Skeleton(ent, Color.White);
        }
    }

}