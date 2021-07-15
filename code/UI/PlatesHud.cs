using Sandbox;
using Sandbox.UI;

[Library]
public partial class PlatesHudEntity : HudEntity<RootPanel>
{
	public PlatesHudEntity()
	{
		if ( IsClient )
		{
			//RootPanel.SetTemplate( "/plateshud.html" );
			RootPanel.StyleSheet.Load("/ui/plateshud.scss");

			RootPanel.AddChild<NameTags>();
			RootPanel.AddChild<PlateNameTags>();
			RootPanel.AddChild<Crosshair>();
			RootPanel.AddChild<ChatBox>();
			RootPanel.AddChild<VoiceList>();
			RootPanel.AddChild<KillFeed>();
			RootPanel.AddChild<Scoreboard<ScoreboardEntry>>();
			RootPanel.AddChild<Logo>();
			RootPanel.AddChild<Vitals>();
			RootPanel.AddChild<EventHud>();
			RootPanel.AddChild<EventSub>();
			RootPanel.AddChild<InventoryBar>();

			if(IsServer){
				RootPanel.AddChild<DevTools>();
			}
		}
	}

	[ClientRpc]
	public void OnPlayerDied( string victim, string attacker = null )
	{
		Host.AssertClient();
	}

	[ClientRpc]
	public void ShowDeathScreen( string attackerName )
	{
		Host.AssertClient();
	}
}