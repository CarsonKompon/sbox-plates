
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
}