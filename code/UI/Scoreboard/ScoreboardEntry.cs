
using Sandbox;
using Sandbox.Hooks;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;

public partial class ScoreboardEntry : Panel
{

	public Client Client;

	public Label PlayerName;
	public Label Kills;
	public Label Deaths;
	public Label Ping;

	public ScoreboardEntry()
	{
		AddClass( "entry" );

		PlayerName = Add.Label( "PlayerName", "name" );
		Kills = Add.Label( "", "kills" );
		Deaths = Add.Label( "", "deaths" );
		Ping = Add.Label( "", "ping" );
	}

	public override void Tick()
	{
		base.Tick();
	}

	public virtual void UpdateData()
	{
		PlayerName.Text = Client.Name;
		Kills.Text = Client.GetInt( "kills" ).ToString();
		Deaths.Text = Client.GetInt( "deaths" ).ToString();
		Ping.Text = Client.Ping.ToString();
		SetClass( "me", Client == Local.Client );
	}
	
}
