using Sandbox;
using System.Linq;
using System.Collections.Generic;

[EventBase]
public class PlayerStinkyEvent : EventBase
{
    public PlayerStinkyEvent(){
        name = "player_smell_bad";
        text = " player(s) will smell bad in ";
        type = EventType.Player;
    }

    public override void OnEvent(Entity ent){
        (ent as PlatesPlayer).RenderColor = Color.Green;
        new SmellBadEnt(ent);
    }
}

public class SmellBadEnt : Entity
{
    public Entity ent;

    public SmellBadEnt(Entity e){
        ent = e;
        PlatesGame.GameEnts.Add(this);
    }

    [Event.Tick]
    public void Tick(){
        if(ent.IsValid()){
            var part = Particles.Create("particles/stinky.vpcf");
            part.SetPosition(0,ent.Position + Vector3.Up*40);
            if(IsServer && Client.All.Count > 1){
                var nearest = Entity.All.OfType<Player>().OrderBy( x => Vector3.DistanceBetween( x.Position + x.Rotation.Up * 40, ent.Position + Rotation.Up * 40 ) ).ToArray()[1];
                var distance = Vector3.DistanceBetween( nearest.Position, ent.Position );
                if(distance <= 100){
                    nearest.TakeDamage(DamageInfo.Generic( 0.05f ));
                }
            }
        }else Delete();
    }

}