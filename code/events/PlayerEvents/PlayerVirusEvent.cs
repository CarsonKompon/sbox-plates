using Sandbox;
using Sandbox.UI;
using System;
using System.Linq;
using System.Collections.Generic;
using Sandbox.UI.Construct;

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
        new VirusCureEnt(plate as Entity, ent as PlatesPlayer);
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

public partial class VirusCureEnt : Prop
{   

    [Net] public long owner {get;set;}
	[Net] public string ownerName {get;set;}

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


//UI STUFF
public class VirusTag : Panel
{
    public Label NameLabel;
    //public Image Avatar;

    VirusCureEnt virus;

    public VirusTag( VirusCureEnt virus )
    {
        this.virus = virus;

        NameLabel = Add.Label( $"{virus.ownerName}'s Cure" );
        //Avatar = Add.Image( $"avatar:{plate.owner}" );
    }

    public virtual void UpdateFromPlayer( VirusCureEnt virus )
    {
        // Nothing to do unless we're showing health and shit
    }
}

[LoadUI]
public class VirusNameTags : Panel
{
    Dictionary<VirusCureEnt, VirusTag> ActiveTags = new Dictionary<VirusCureEnt, VirusTag>();

    //public int MaxTagsToShow = 5;

    public VirusNameTags()
    {
        StyleSheet.Load( "/events/playerevents/ui/virustags.scss" );
    }

    public override void Tick()
    {
        base.Tick();


        var deleteList = new List<VirusCureEnt>();
        deleteList.AddRange( ActiveTags.Keys );

        int count = 0;
        foreach ( var virus in Entity.All.OfType<VirusCureEnt>().OrderBy( x => Vector3.DistanceBetween( x.Position, CurrentView.Position ) ) )
        {
            if ( UpdateNameTag( virus ) )
            {
                deleteList.Remove( virus );
                count++;
            }

            // if ( count >= MaxTagsToShow )
            // 	break;
        }

        foreach( var virus in deleteList )
        {
            ActiveTags[virus].Delete();
            ActiveTags.Remove( virus );
        }

    }

    public virtual VirusTag CreateNameTag( VirusCureEnt virus )
    {
        if ( !virus.IsValid() )
            return null;

        var tag = new VirusTag( virus );
        tag.Parent = this;
        return tag;
    }

    public bool UpdateNameTag( VirusCureEnt virus )
    {
        // Where we putting the label, in world coords
        var labelPos = virus.Position + Vector3.Up * 32;

        // Are we too far away?
        var cPos = CurrentView.Position;
        float dist = labelPos.Distance( cPos );

        // Only draw if looking at the plate
        /*
        var tr = Trace.Ray( cPos, cPos + CurrentView.Rotation.Forward * 10000 )
                        .Size( 1.0f )
                        .Ignore( CurrentView.Viewer )
                        .UseHitboxes()
                        .Run();
        var alpha = 0;
        if(tr.Hit && tr.Entity == virus) alpha = 1;
        */
        var alpha = 1;

        //var alpha = dist.LerpInverse( MaxDrawDistance, MaxDrawDistance * 0.1f, true );

        // If I understood this I'd make it proper function
        var objectSize = 0.05f / dist / (2.0f * MathF.Tan( (CurrentView.FieldOfView / 2.0f).DegreeToRadian() )) * 1500.0f;

        objectSize = objectSize.Clamp( 0.25f, 1.0f );

        if ( !ActiveTags.TryGetValue( virus, out var tag ) )
        {
            tag = CreateNameTag( virus );
            if ( tag != null )
            {
                ActiveTags[virus] = tag;
            }
        }

        tag.UpdateFromPlayer( virus );

        var screenPos = labelPos.ToScreen();

        tag.Style.Left = Length.Fraction( screenPos.x );
        tag.Style.Top = Length.Fraction( screenPos.y );
        tag.Style.Opacity = alpha;

        var transform = new PanelTransform();
        transform.AddTranslateY( Length.Fraction( -1.0f ) );
        transform.AddScale( objectSize );
        transform.AddTranslateX( Length.Fraction( -0.5f ) );

        tag.Style.Transform = transform;
        tag.Style.Dirty();

        return true;
    }
}
