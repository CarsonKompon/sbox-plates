using Sandbox;
using System.Linq;
using System.Collections.Generic;

 
public class PlayerStinkyEvent : PlatesEventAttribute
{
    public PlayerStinkyEvent(){
        name = "player_smell_bad";
        text = " player(s) will smell bad in ";
        type = EventType.Player;
    }

    public override void OnEvent(Entity ent){
        (ent as PlatesPlayer).RenderColor = Color.Green;
        PlatesGame.AddEntity(new SmellBadEnt(ent));
    }
}

public partial class SmellBadEnt : Entity
{
    [Net] public Entity ent {get;set;}
    private DamageInfo damage;

    public SmellBadEnt(){}
    public SmellBadEnt(Entity e){
        ent = e;
        Name = e.Name + "'s Stench";
        damage = new DamageInfo().WithAttacker(this);
        damage.Damage = 0.05f;
    }

    [Event.Tick.Server]
    public void Tick(){
        if(ent is PlatesPlayer ply)
        {
            var part = Particles.Create("particles/stinky.vpcf");
            part.SetPosition(0,ent.Position + Vector3.Up*40);
            if(IsServer && Client.All.Count > 1){
                var nearest = Entity.All.OfType<Player>().OrderBy( x => Vector3.DistanceBetween( x.Position + x.Rotation.Up * 40, ent.Position + Rotation.Up * 40 ) ).ToArray()[1];
                var distance = Vector3.DistanceBetween( nearest.Position, ent.Position );
                if(distance <= 100)
                {
                    nearest.TakeDamage(damage);
                }
            }
        }
    }

}