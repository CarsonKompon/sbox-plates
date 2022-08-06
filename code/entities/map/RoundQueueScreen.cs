using Sandbox;
using Sandbox.Internal;
using Sandbox.UI;
using Sandbox.UI.Construct;
using SandboxEditor;
using System;
using System.Net;

[Library("plates_screen_round_queue", Description = "A screen that displays the top ranks in a leaderboard"), HammerEntity]
[EditorModel("models/leaderboard_screen.vmdl")]
public partial class RoundQueueScreen : Prop
{
    private RoundQueueScreenUI screen = null;
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
            screen = new RoundQueueScreenUI(Scale);
            screen.Position = Position + (Rotation.Up * (64 * Scale)) + (Rotation.Left * (5.25f * Scale));
            screen.Rotation = Rotation.LookAt(Rotation.Left);
        }
    }
}

public partial class RoundQueueScreenUI : WorldPanel
{
    public RealTimeSince TimeSinceUpdate = 0f;
    public Label Header;
    public Panel Content;
    public static RoundQueueScreenUI Instance;

    public RoundQueueScreenUI(){}
    public RoundQueueScreenUI(float scale)
    {
        StyleSheet.Load("/entities/map/roundqueuescreen.scss");

        Header = Add.Label("Round Queue", "header");
        Content = Add.Panel("content");

        var width = 96 * 20 * scale;
        var height = 128 * 20 * scale;
        PanelBounds = new Rect(-width * .5f, -height * .5f, width, height);

        Instance = this;
    }

    [ClientRpc]
    public static void AddRound(string name)
    {
        var e = Instance.Content.AddChild<RoundQueueScreenUIEntry>();
        e.Name.Text = name;
        // foreach(var child in Instance.Content.Children)
        // {
        //     child.Delete();
        // }

        // var i = 0;
        // foreach(var round in PlatesGame.RoundQueue)
        // {
        //     var e = Instance.Content.AddChild<RoundQueueScreenUIEntry>();
        //     e.Name.Text = round.name;

        //     i++;
        //     if(i >= 10) break;
        // }
    }

    [ClientRpc]
    public static void RemoveLatest()
    {
        foreach(var child in Instance.Content.Children)
        {
            child.Delete();
            break;
        }
    }
}

public class RoundQueueScreenUIEntry : Panel
{
    public Label Name;

    public RoundQueueScreenUIEntry()
    {
        Name = Add.Label("", "name");
    }
}