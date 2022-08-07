using Sandbox;
using Sandbox.UI;

public partial class PlatesHud : HudEntity<RootPanel>
{
    public PlatesHud()
    {
        if(!IsClient) return;

        RootPanel.StyleSheet.Load("/ui/PlatesHud.scss");

        RootPanel.AddChild<VitalInfo>();

        RootPanel.AddChild<HeaderText>();
        RootPanel.AddChild<RoundInfo>();
        RootPanel.AddChild<RoundReport>();

        RootPanel.AddChild<InventoryBar>();
        RootPanel.AddChild<Crosshair>();

        RootPanel.AddChild<PlatesChatBox>();
        RootPanel.AddChild<KillFeed>();
        RootPanel.AddChild<PlatesScoreboard<PlatesScoreboardEntry>>();

        RootPanel.AddChild<VoiceList>();
        RootPanel.AddChild<VoiceSpeaker>();
    }
}