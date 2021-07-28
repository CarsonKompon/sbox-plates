using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

public partial class RoundInfo : Panel
{

	public Panel Header { get; protected set; }
	public static float Vis = 0f;
	public static Label label;

	public RoundInfo(){
		StyleSheet.Load( "/ui/roundinfo.scss" );
		AddClass( "roundinfo" );

		Header = Add.Panel( "header" );
		label = Header.Add.Label( "Normal Round", "name" );

	}

	public override void Tick()
	{
		//base.Tick();
		if(Vis > 0f) Vis -= 1.0f/Global.TickRate;
		SetClass( "open", Input.Down(InputButton.Score) || (Vis > 0.0f) );
	}

	[ClientRpc]
	public static void NewRound(string text){
		label.Text = text;
		Vis = 30f;
	}
}