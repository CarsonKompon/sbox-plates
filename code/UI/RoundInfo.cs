using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System.Collections.Generic;
using System.Linq;

public partial class RoundInfo : Panel
{

	public Panel Header { get; protected set; }
	public Panel SubHeader { get; protected set; }
	public Panel Canvas { get; protected set; }
	Dictionary<Client, PlatesScoreboardEntry> Rows = new();
	public static float Vis = 0f;
	public static Label label;

	public RoundInfo(){
		StyleSheet.Load( "/ui/roundinfo.scss" );
		AddClass( "roundinfo" );

		Header = Add.Panel( "header" );
		label = Header.Add.Label( "Normal Round", "name" );

		Canvas = Header.Add.Panel("canvas");
		SubHeader = Canvas.Add.Panel( "subheader" );
		SubHeader.Add.Label( "Name", "name" );
		SubHeader.Add.Label( "Wins", "wins" );
		SubHeader.Add.Label( "Ping", "ping" );
	}

	public override void Tick()
	{
		//base.Tick();
		if(Vis > 0f) Vis -= 1.0f/Global.TickRate;
		SetClass("open", Input.Down(InputButton.Score) || (Vis > 0.0f));
		SetClass("expand", Input.Down(InputButton.Score));
		Canvas.SetClass( "open", Input.Down(InputButton.Score) );

		if ( !IsVisible )
			return;

		// Clients that were added
		foreach ( var client in Client.All.Except( Rows.Keys ) )
		{
			var entry = AddClient( client );
			Rows[client] = entry;
		}

		foreach ( var client in Rows.Keys.Except( Client.All ) )
		{
			if ( Rows.TryGetValue( client, out var row ))
			{
				row?.Delete();
				Rows.Remove( client );
			}
		}
	}

	protected virtual PlatesScoreboardEntry AddClient( Client entry )
	{
		var p = Canvas.AddChild<PlatesScoreboardEntry>();
		p.client = entry;
		return p;
	}

	[ClientRpc]
	public static void NewRound(string text){
		label.Text = text;
		Vis = 30f;
	}
}

public partial class PlatesScoreboardEntry : Panel
{
	public Client client;

	public Label playerName;
	public Label wins;
	public Label ping;

	public PlatesScoreboardEntry()
	{
		AddClass("entry");

		playerName = Add.Label("Missingname.", "name");
		wins = Add.Label("0", "wins");
		ping = Add.Label("999", "ping");
	}

	RealTimeSince TimeSinceUpdate = 0;

	public override void Tick()
	{
		base.Tick();

		if ( !IsVisible )
			return;

		if ( !client.IsValid() )
			return;

		if ( TimeSinceUpdate < 0.1f )
			return;

		TimeSinceUpdate = 0;
		UpdateData();
	}

	public virtual void UpdateData()
	{
		playerName.Text = client.Name;
		wins.Text = client.GetInt( "wins" ).ToString();
		ping.Text = client.Ping.ToString();
		SetClass( "me", client == Local.Client );
	}

	public virtual void UpdateFrom( Client _client )
	{
		client = _client;
		UpdateData();
	}
}