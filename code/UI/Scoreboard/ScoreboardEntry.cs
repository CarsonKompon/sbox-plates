
using Sandbox;
using Sandbox.Hooks;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;

public partial class ScoreboardEntry : Panel
{
	public PlayerScore.Entry Entry;

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

	public virtual void UpdateFrom( PlayerScore.Entry entry )
	{
		Entry = entry;

		PlayerName.Text = entry.GetString( "name" );
		Kills.Text = entry.Get<int>( "kills", 0 ).ToString();
		Deaths.Text = entry.Get<int>( "deaths", 0 ).ToString();
		Ping.Text = entry.Get<int>( "ping", 0 ).ToString();

		SetClass( "me", Local.Client != null && entry.Get<ulong>( "steamid", 0 ) == Local.Client.SteamId );
	}
}
