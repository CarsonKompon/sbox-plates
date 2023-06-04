
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sandbox.UI
{
	public partial class PlatesScoreboard<T> : Panel where T : PlatesScoreboardEntry, new()
	{
		public Panel Canvas { get; protected set; }
		Dictionary<IClient, T> Rows = new ();

		public Panel Header { get; protected set; }

		public PlatesScoreboard()
		{
			StyleSheet.Load( "/ui/screen/scoreboard/platesscoreboard.scss" );
			AddClass( "scoreboard" );

			AddHeader();

			Canvas = Add.Panel( "canvas" );
		}

		public override void Tick()
		{
			base.Tick();

			SetClass( "open", ShouldBeOpen() );

			if ( !IsVisible )
				return;

			//
			// Clients that were added
			//
			foreach ( var client in Game.Clients.Except( Rows.Keys ) )
			{
				var entry = AddClient( client );
				Rows[client] = entry;
			}

			foreach ( var client in Rows.Keys.Except( Game.Clients ) )
			{
				if ( Rows.TryGetValue( client, out var row ))
				{
					row?.Delete();
					Rows.Remove( client );
				}
			}
		}

		public virtual bool ShouldBeOpen()
		{
			return Input.Down( "score" );
		}


		protected virtual void AddHeader() 
		{
			Header = Add.Panel( "header" );
			Header.Add.Label( "Name", "name" );
			Header.Add.Label( "Money", "rank" );
			Header.Add.Label( "Wins", "wins" );
			Header.Add.Label( "Ping", "ping" );
		}

		protected virtual T AddClient( IClient entry )
		{
			var p = Canvas.AddChild<T>();
			p.Client = entry;
			return p;
		}
	}
}
