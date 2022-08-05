using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;

public class NameTag : WorldPanel
{
    public Label NameLabel;
    public Image Avatar;

    private PlatesPlayer player = null;

    public NameTag(PlatesPlayer pl)
    {
        StyleSheet.Load("/ui/world/nametag.scss");
        AddClass("nametag");

        player = pl;
        NameLabel = Add.Label( player.Client.Name, "name" );
        Avatar = Add.Image( $"avatar:{player.Client.PlayerId}", "nametag-avatar" );


        var width = 700;
        var height = 200;
        PanelBounds = new Rect(-width * .5f, -height * .5f, width, height);
    }

    [Event.Frame]
    public void OnFrame()
    {
        Position = player.Position + Vector3.Up * 80;
        Rotation = Rotation.LookAt(-Screen.GetDirection(new Vector2(Screen.Width * 0.5f, Screen.Height * 0.5f)));

        // Only draw if looking at the plate
        var cPos = CurrentView.Position;
        var dist = Position.Distance(cPos);
        var tr = Trace.Ray( cPos, cPos + CurrentView.Rotation.Forward * 10000 )
                        .Size( 1.0f )
                        .Ignore( CurrentView.Viewer )
                        .UseHitboxes()
                        .Run();
        
        // 8 - 32 Characters
        // 36px - 18px;
        var _characters = NameLabel.TextLength;
        var _fontSize = MathC.Map(_characters, 8, 32, 36, 18);
        NameLabel.Style.FontSize = Length.Pixels(_fontSize);
        Style.Opacity = Math.Clamp((1000f-dist)/1000f, 0f, 1000f);
    }
}

// 	public class PlateNameTags : Panel
// 	{
// 		Dictionary<Plate, PlateTag> ActiveTags = new Dictionary<Plate, PlateTag>();

// 		//public int MaxTagsToShow = 5;

// 		public PlateNameTags()
// 		{
// 			StyleSheet.Load( "./platetags.scss" );
// 		}

// 		public override void Tick()
// 		{
// 			base.Tick();


// 			var deleteList = new List<Plate>();
// 			deleteList.AddRange( ActiveTags.Keys );

// 			int count = 0;
// 			foreach ( var plate in Entity.All.OfType<Plate>().OrderBy( x => Vector3.DistanceBetween( x.Position, CurrentView.Position ) ) )
// 			{
// 				if ( UpdateNameTag( plate ) )
// 				{
// 					deleteList.Remove( plate );
// 					count++;
// 				}

// 				// if ( count >= MaxTagsToShow )
// 				// 	break;
// 			}

// 			foreach( var plate in deleteList )
// 			{
// 				ActiveTags[plate].Delete();
// 				ActiveTags.Remove( plate );
// 			}

// 		}

// 		public virtual PlateTag CreateNameTag( Plate plate )
// 		{
// 			if ( !plate.IsValid() )
// 				return null;

// 			var tag = new PlateTag( plate );
// 			tag.Parent = this;
// 			return tag;
// 		}

// 		public bool UpdateNameTag( Plate plate )
// 		{
// 			// Where we putting the label, in world coords
// 			var labelPos = plate.Position + plate.Rotation.Up * 72;

// 			// Are we too far away?
// 			var cPos = CurrentView.Position;
// 			float dist = labelPos.Distance( cPos );

// 			// Only draw if looking at the plate
// 			var tr = Trace.Ray( cPos, cPos + CurrentView.Rotation.Forward * 10000 )
// 							.Size( 1.0f )
// 							.Ignore( CurrentView.Viewer )
// 							.UseHitboxes()
// 							.Run();
// 			var alpha = 0;
// 			if(tr.Hit && tr.Entity == plate) alpha = 1;

// 			//var alpha = dist.LerpInverse( MaxDrawDistance, MaxDrawDistance * 0.1f, true );

// 			// If I understood this I'd make it proper function
// 			var objectSize = 0.05f / dist / (2.0f * MathF.Tan( (CurrentView.FieldOfView / 2.0f).DegreeToRadian() )) * 1500.0f;

// 			objectSize = objectSize.Clamp( 0.4f, 1.0f );

// 			if ( !ActiveTags.TryGetValue( plate, out var tag ) )
// 			{
// 				tag = CreateNameTag( plate );
// 				if ( tag != null )
// 				{
// 					ActiveTags[plate] = tag;
// 				}
// 			}

// 			tag.UpdateFromPlayer( plate );

// 			var screenPos = labelPos.ToScreen();

// 			tag.Style.Left = Length.Fraction( screenPos.x );
// 			tag.Style.Top = Length.Fraction( screenPos.y );
// 			tag.Style.Opacity = alpha;

// 			var transform = new PanelTransform();
// 			transform.AddTranslateY( Length.Fraction( -1.0f ) );
// 			transform.AddScale( objectSize );
// 			transform.AddTranslateX( Length.Fraction( -0.5f ) );

// 			tag.Style.Transform = transform;
// 			tag.Style.Dirty();

// 			return true;
// 		}
// 	}
// }
