using Sandbox;
using Sandbox.UI;

[Library]
public partial class PlatesHudEntity : HudEntity<RootPanel>
{
	//public static AvatarHud avHud;

	public PlatesHudEntity()
	{
		if ( IsClient )
		{
			//RootPanel.SetTemplate( "/plateshud.html" );
			RootPanel.StyleSheet.Load("/ui/plateshud.scss");

			//avHud = RootPanel.AddChild<AvatarHud>();
			RootPanel.AddChild<NameTags>();
			RootPanel.AddChild<PlateNameTags>();
			RootPanel.AddChild<CrosshairCanvas>();
			RootPanel.AddChild<ChatBox>();
			RootPanel.AddChild<VoiceList>();
			RootPanel.AddChild<KillFeed>();
			RootPanel.AddChild<Logo>();
			RootPanel.AddChild<RoundInfo>();
			RootPanel.AddChild<Vitals>();
			RootPanel.AddChild<EventHud>();
			RootPanel.AddChild<EventSub>();
			RootPanel.AddChild<InventoryBar>();

			//Load additional UI
			foreach(LoadUI _ui in Library.GetAttributes<LoadUI>()){
				//Events.Add(_ui.Create<EventBase>());
				var type = _ui.GetType();
				RootPanel.AddChild(_ui.Create<Panel>());
			}

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
