
using Sandbox;
using Sandbox.Hooks;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;

namespace Sandbox.UI
{
	public partial class PlatesScoreboardEntry : Panel
	{
		public Client Client;

		public Label PlayerName;
		public Label Wins;
		public Label Rank;
		public Label Ping;

		public PlatesScoreboardEntry()
		{
			AddClass( "entry" );

			PlayerName = Add.Label( "PlayerName", "name" );
			Wins = Add.Label( "", "rank" );
			Rank = Add.Label( "", "wins" );
			Ping = Add.Label( "", "ping" );
		}

		RealTimeSince TimeSinceUpdate = 0;

		public override void Tick()
		{
			base.Tick();

			if ( !IsVisible )
				return;

			if ( !Client.IsValid() )
				return;

			if ( TimeSinceUpdate < 0.1f )
				return;

			TimeSinceUpdate = 0;
			UpdateData();
		}

		public virtual void UpdateData()
		{
			PlayerName.Text = Client.Name;
			Wins.Text = Client.GetInt( "wins" ).ToString();
			Rank.Text = Client.GetInt( "rank" ).ToString();
			Ping.Text = Client.Ping.ToString();
			SetClass( "me", Client == Local.Client );
		}

		public virtual void UpdateFrom( Client client )
		{
			Client = client;
			UpdateData();
		}
	}
}
