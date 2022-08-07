using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System.Collections.Generic;
using System.Linq;

public partial class RoundInfo : Panel
{
    public Panel Header { get; protected set; }
    public Panel SubHeader {get; protected set; }
    public Panel Canvas {get; protected set; }

    public static float Vis = 0f;
	private static Label roundLabel;
	//private static Label infoLabel;

	public RoundInfo(){
        StyleSheet.Load( "/ui/screen/roundinfo.scss" );
        AddClass( "roundinfo" );

        Header = Add.Panel( "header" );
        roundLabel = Header.Add.Label( "Normal Round", "round" );

        Canvas = Header.Add.Panel("canvas");
        //SubHeader = Canvas.Add.Panel( "subheader" );
        //infoLabel = SubHeader.Add.Label( "", "info" );
    }

    public override void Tick()
    {
        //base.Tick();
        var _score = Input.Down(InputButton.Score);
        if(Vis > 0f) Vis -= 1.0f/Global.TickRate;
        SetClass("open", _score || (Vis > 0.0f));
        //SetClass("expand", Input.Down(InputButton.Score));
        Canvas.SetClass( "open", _score );

        // if ( !IsVisible )
        //     return;

        // foreach ( var client in Rows.Keys.Except( Client.All ) )
        // {
        //     if ( Rows.TryGetValue( client, out var row ))
        //     {
        //         row?.Delete();
        //         Rows.Remove( client );
        //     }
        // }
    }

	[ClientRpc]
	public static void SetRoundText(string text, string info = ""){
		roundLabel.Text = text;
		//infoLabel.Text = info;
		Vis = 30f;
	}
}