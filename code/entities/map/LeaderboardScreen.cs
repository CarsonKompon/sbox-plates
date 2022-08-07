using Sandbox;
using Sandbox.Internal;
using Sandbox.UI;
using Sandbox.UI.Construct;
using SandboxEditor;
using System;
using System.Net;

[Library("plates_screen_leaderboard", Description = "A screen that displays the top ranks in a leaderboard"), HammerEntity]
[EditorModel("models/leaderboard_screen.vmdl")]
public partial class LeaderboardScreen : Prop
{
    private LeaderboardScreenUI screen = null;
    public override void Spawn()
    {
        base.Spawn();
        SetModel("models/leaderboard_screen.vmdl");
        SetupPhysicsFromModel(PhysicsMotionType.Static);
        EnableAllCollisions = true;
        Tags.Add("solid");
    }

    [Event.Tick]
    public void Tick()
    {
        if(IsClient && screen == null)
        {
            screen = new LeaderboardScreenUI(Scale);
            screen.Position = Position + (Rotation.Up * (64 * Scale)) + (Rotation.Left * (5.25f * Scale));
            screen.Rotation = Rotation.LookAt(Rotation.Left);
        }
    }
}

public class LeaderboardScreenUI : WorldPanel
{
    public RealTimeSince TimeSinceUpdate = 0f;
    public Label Header;
    public Panel Subheader;
    public Panel Content;

    public LeaderboardScreenUI(){}
    public LeaderboardScreenUI(float scale)
    {
        StyleSheet.Load("/entities/map/leaderboardscreen.scss");

        Header = Add.Label("Leaderboards", "header");
        Subheader = Add.Panel("subheader");
        Subheader.Add.Label("Rank", "rank-header");
        Subheader.Add.Label("", "filler-header");
        Subheader.Add.Label("Name", "name-header");
        Subheader.Add.Label("Wins", "wins-header");
        Content = Add.Panel("content");

        var width = 96 * 20 * scale;
        var height = 128 * 20 * scale;
        PanelBounds = new Rect(-width * .5f, -height * .5f, width, height);

        FetchTopTen();
    }

    public override void Tick()
    {
        base.Tick();
        
        if(TimeSinceUpdate > 60f)
        {
            FetchTopTen();
            TimeSinceUpdate = 0f;
        }
    }

    public async void FetchTopTen()
    {
        foreach(var child in Content.Children)
        {
            child.Delete();
        }

        var http = new Http(new Uri("https://sap.facepunch.com/asset/carsonk.plates/leaderboard"));
        var response = await http.GetStringAsync();
        var board = Json.Deserialize<LeaderboardResult>(response);

        var count = 0;
        foreach(var entry in board.Entries)
        {
            var rankHttp = new Sandbox.Internal.Http(new Uri("https://sap.facepunch.com/asset/carsonk.plates/rank/" + entry.PlayerId));
            var rankResponse = await rankHttp.GetStringAsync();
            var gameRank = Json.Deserialize<PlayerGameRank>(rankResponse);

            var e = Content.AddChild<LeaderboardScreenUIEntry>();
            e.Rank.Text = $"#{count + 1}";
            e.Name.Text = entry.DisplayName;
            e.Avatar.SetTexture($"avatar:{entry.PlayerId}");
            e.Wins.Text = gameRank.Wins.ToString();

            e.SetClass("open", true);

            count++;
            if(count >= 10) break;
        }
    }
}

public class LeaderboardScreenUIEntry : Panel
{
    public Label Rank;
    public Panel AvatarPanel;
    public Image Avatar;
    public Label Name;
    public Label Wins;

    public LeaderboardScreenUIEntry()
    {
        Rank = Add.Label("#1", "rank");
        AvatarPanel = Add.Panel("avatar-panel");
        Avatar = AvatarPanel.Add.Image("", "avatar");
        Name = Add.Label("", "name");
        Wins = Add.Label("", "wins");
    }
}