using Sandbox;
using System.Linq;
using System.Collections.Generic;

[EventBase]
public class PlayerVirusEvent : EventBase
{
    public PlayerVirusEvent(){
        name = "player_virus";
        text = " player(s) will get the virus in ";
        type = EventType.Player;
    }

    public override void OnEvent(Entity ent){
        //(ent as PlatesPlayer).RenderColor = Color.Green;
        new PlyVirusEnt(ent);
        var plate = Entity.All.OfType<Plate>().OrderBy(x => Rand.Double()).ToArray()[0];
        new VirusCureEnt(plate as Entity);
    }
}

public class PlyVirusEnt : Entity
{
    public Entity ent;

    public PlyVirusEnt(Entity e){
        ent = e;
        PlatesGame.GameEnts.Add(this);
    }

    [Event.Tick]
    public void Tick(){
        if(ent.IsValid()){
            var part = Particles.Create("particles/virus.vpcf");
            part.SetPosition(0,ent.Position + Vector3.Up*40);
            if(IsServer){
                ent.TakeDamage(DamageInfo.Generic( 0.001f ));
            }
        }else Delete();
    }

}

public class VirusCureEnt : Prop
{   

    public VirusCureEnt(){}
    public VirusCureEnt(Entity e){
        SetModel("models/teslacoil.vmdl");
		SetupPhysicsFromModel(PhysicsMotionType.Dynamic);
        RenderColor = Color.Magenta;
        Position = e.Position + Vector3.Up*5;
        Scale = 0.1f;
        //SetParent(e);
        PlatesGame.GameEnts.Add(this);
    }

    protected override void OnPhysicsCollision( CollisionEventData eventData )
    {
        foreach(var virus in Entity.All.OfType<PlyVirusEnt>()){
            if(virus.ent == eventData.Entity){
                virus.Delete();
                Delete();
            }
        }
        base.OnPhysicsCollision( eventData );
    }

}