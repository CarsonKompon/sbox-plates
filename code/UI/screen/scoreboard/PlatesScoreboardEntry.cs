
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;

namespace Sandbox.UI
{
	public partial class PlatesScoreboardEntry : Panel
	{
		public IClient Client;

		public Label PlayerName;
		public Label Wins;
		public Label Rank;
		public Label Ping;

		public PlatesScoreboardEntry()
		{
			AddClass( "entry" );

			PlayerName = Add.Label( "PlayerName", "name" );
			Rank = Add.Label( "", "rank" );
			Wins = Add.Label( "", "wins" );
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
			Rank.Text = PlayerDataManager.GetMoney(Client.SteamId).ToString();
			Wins.Text = Client.GetInt( "wins" ).ToString();
			Ping.Text = Client.Ping.ToString();
			SetClass( "me", Client == Game.LocalClient );
		}

		public virtual void UpdateFrom( IClient client )
		{
			Client = client;
			UpdateData();
		}
	}
}
