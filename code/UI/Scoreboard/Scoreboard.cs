
using Sandbox;
using Sandbox.Hooks;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;


public partial class Scoreboard<T> : Panel where T : ScoreboardEntry, new()
{
	public Panel Canvas { get; protected set; }
	Dictionary<int, T> Entries = new ();

	public Panel Header { get; protected set; }

	public Scoreboard()
	{
		StyleSheet.Load( "/ui/scoreboard/platesscoreboard.scss" );
		AddClass( "scoreboard" );


		AddHeader();

		Canvas = Add.Panel( "canvas" );

		PlayerScore.OnPlayerAdded += AddPlayer;
		PlayerScore.OnPlayerUpdated += UpdatePlayer;
		PlayerScore.OnPlayerRemoved += RemovePlayer;

		foreach ( var player in PlayerScore.All )
		{
			AddPlayer( player );
		}
	}

	public override void Tick()
	{
		base.Tick();

		SetClass( "open", Input.Down(InputButton.Score) );
	}


	protected virtual void AddHeader() 
	{
		Header = Add.Panel( "header" );
		Header.Add.Label( "Name", "name" );
		Header.Add.Label( "Games", "kills" );
		Header.Add.Label( "Wins", "deaths" );
		Header.Add.Label( "Ping", "ping" );
	}

	protected virtual void AddPlayer( PlayerScore.Entry entry )
	{
		var p = Canvas.AddChild<T>();
		p.UpdateFrom( entry );

		Entries[entry.Id] = p;
	}

	protected virtual void UpdatePlayer( PlayerScore.Entry entry )
	{
		if ( Entries.TryGetValue( entry.Id, out var panel ) )
		{
			panel.UpdateFrom( entry );
		}
	}

	protected virtual void RemovePlayer( PlayerScore.Entry entry )
	{
		if ( Entries.TryGetValue( entry.Id, out var panel ) )
		{
			panel.Delete();
			Entries.Remove( entry.Id );
		}
	}
}