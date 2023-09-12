using Sandbox;
using System.Linq;
using System.Collections.Generic;

 namespace Plates;

public class PlayerStinkyEvent : PlatesEvent
{
    public PlayerStinkyEvent(){
        name = "Player Smell Bad";
        command = "player_smell_bad";
        text = " player(s) will smell bad in ";
        type = EventType.Player;
        rarity = EventRarity.Rare;
    }

    public override void OnEvent(Entity ent){
        (ent as Player).RenderColor = Color.Green;
        PlatesGame.Current.AddEntity(new SmellBadEnt(ent));
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

    [GameEvent.Tick.Server]
    public void Tick(){
        if(ent is Player ply)
        {
            var part = Particles.Create("particles/stinky.vpcf");
            part.SetPosition(0,ent.Position + Vector3.Up*40);
            if(Game.Clients.Count > 1){
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