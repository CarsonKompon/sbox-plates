using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;
using System.Linq;

public class NameTag : WorldPanel
{
    public Label NameLabel;
    public Image Avatar;

    public NameTag(string name, long? steamid)
    {
        StyleSheet.Load("/ui/world/nametag.scss");
        AddClass("nametag");

        NameLabel = Add.Label( name, "name" );
        if(steamid != null) Avatar = Add.Image( $"avatar:{steamid}", "nametag-avatar" );

        var width = 700;
        var height = 200;
        PanelBounds = new Rect(-width * .5f, -height * .5f, width, height);
    }

    // [Event.Frame]
    // public void OnFrame()
    // {
    //     if(!player.IsValid()) return;
    //     if(!player.Client.IsValid()) return;
        
    //     Position = player.Position + Vector3.Up * 80;
    //     Rotation = Rotation.LookAt(-Screen.GetDirection(new Vector2(Screen.Width * 0.5f, Screen.Height * 0.5f)));

    //     // Only draw if looking at the plate
    //     var cPos = CurrentView.Position;
    //     var dist = Position.Distance(cPos);
    //     var tr = Trace.Ray( cPos, cPos + CurrentView.Rotation.Forward * 10000 )
    //                     .Size( 1.0f )
    //                     .Ignore( CurrentView.Viewer )
    //                     .UseHitboxes()
    //                     .Run();
        
    //     // 8 - 32 Characters
    //     // 36px - 18px;
    //     var _characters = NameLabel.TextLength;
    //     var _fontSize = MathC.Map(_characters, 8, 32, 36, 18);
    //     NameLabel.Style.FontSize = Length.Pixels(_fontSize);
    //     Style.Opacity = Math.Clamp((1000f-dist)/1000f, 0f, 1000f);
    // }
}

internal class NameTagComponent : EntityComponent<PlatesPlayer>
{
    NameTag NameTag;

	protected override void OnActivate()
	{
		NameTag = new NameTag(Entity.Client?.Name ?? Entity.Name, Entity.Client?.SteamId);
	}

	protected override void OnDeactivate()
	{
		NameTag?.Delete();
        NameTag = null;
	}

    [GameEvent.Client.Frame]
    public void FrameUpdate()
    {
        var tx = Entity.GetAttachment("hat") ?? Entity.Transform;
        tx.Position += Vector3.Up * 12f;
        tx.Rotation = Rotation.LookAt(-Camera.Rotation.Forward);

        NameTag.Transform = tx;
    }

    [GameEvent.Client.Frame]
    public static void SystemUpdate()
    {
        foreach(var player in Sandbox.Entity.All.OfType<PlatesPlayer>())
        {
            if(player.IsLocalPawn && player.IsFirstPersonMode)
            {
                var c = player.Components.Get<NameTagComponent>();
                c?.Remove();
                continue;
            }

            var shouldRemove = player.Position.Distance(Camera.Position) > 800;
            shouldRemove = shouldRemove || player.LifeState != LifeState.Alive;
            shouldRemove = shouldRemove || player.IsDormant;

            if(shouldRemove)
            {
                var c = player.Components.Get<NameTagComponent>();
                c?.Remove();
                continue;
            }

            player.Components.GetOrCreate<NameTagComponent>();
        }
    }
}