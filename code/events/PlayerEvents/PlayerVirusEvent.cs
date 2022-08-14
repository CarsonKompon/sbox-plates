using Sandbox;
using Sandbox.UI;
using System;
using System.Linq;
using System.Collections.Generic;
using Sandbox.UI.Construct;

// TODO: Implement

 
public class PlayerVirusEvent : PlatesEventAttribute
{
    public PlayerVirusEvent(){
        name = "Player Gets A Curable Virus";
        command = "player_virus";
        text = " player(s) will get a curable virus in ";
        type = EventType.Player;
        rarity = EventRarity.Rare;
    }

    public override void OnEvent(Entity ent){
        //(ent as PlatesPlayer).RenderColor = Color.Green;
        var plate = Rand.FromList(Entity.All.OfType<Plate>().ToList());
        var cure = new VirusCureEnt(plate as Entity, ent as PlatesPlayer);
        var virus = new PlyVirusEnt(ent, cure);
        PlatesGame.AddEntity(virus);
    }
}

public partial class PlyVirusEnt : Entity
{
    [Net] public Entity ent {get;set;}
    [Net] public VirusCureEnt cure {get;set;}

    public PlyVirusEnt() {}
    public PlyVirusEnt(Entity e, VirusCureEnt c){
        ent = e;
        cure = c;
    }

    [Event.Tick]
    public void Tick(){
        if(ent is PlatesPlayer ply){
            if(Rand.Int(4)==0)
            {
                var part = Particles.Create("particles/virus.vpcf");
                part.SetPosition(0,ent.Position + Vector3.Up*40);
            }
            if(IsServer){
                if(!ply.InGame) Delete();
                else ent.TakeDamage(DamageInfo.Generic( 0.002f ));
            }
        }
    }

    protected override void OnDestroy()
    {
        if(IsServer) cure?.Delete();
        base.OnDestroy();
    }

}

public partial class VirusCureEnt : Prop
{   

    [Net] public long owner {get;set;}
	[Net] public string ownerName {get;set;}

    VirusNameTag nameTag = null;

    public VirusCureEnt(){}
    public VirusCureEnt(Entity e, PlatesPlayer p){
        SetModel("models/teslacoil.vmdl");
		SetupPhysicsFromModel(PhysicsMotionType.Dynamic);
        RenderColor = Color.Magenta;
        Position = e.Position + Vector3.Up*5;
        Scale = 0.1f;
        var client = p.Client;
        owner = client.PlayerId;
        ownerName = client.Name;
        //SetParent(e);
        PlatesGame.AddEntity(this);
    }

    [Event.Tick]
    private void Tick()
    {
        if(IsClient && nameTag != null)
        {
            nameTag = new VirusNameTag(this);
        }
    }

    public override void StartTouch( Entity other )
    {
        if(IsServer)
        {
            foreach(var virus in Entity.All.OfType<PlyVirusEnt>())
            {
                if(virus.ent == other)
                {
                    virus.Delete();
                    Delete();
                }
            }
        }
        base.StartTouch(other);
    }

}


//UI STUFF
public class VirusNameTag : WorldPanel
{
    public Label NameLabel;
    //public Image Avatar;

    VirusCureEnt virus;

    public VirusNameTag( VirusCureEnt virus )
    {
        StyleSheet.Load("/events/playerevents/ui/virustags.scss");
        this.virus = virus;

        NameLabel = Add.Label( $"{virus.ownerName}'s Cure" );
        //Avatar = Add.Image( $"avatar:{plate.owner}" );
    }

    [Event.Tick]
    public override void Tick()
    {
        base.Tick();

        Position = virus.Position + Vector3.Up * 50f;
    }
}