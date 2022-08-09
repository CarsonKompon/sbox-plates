using Sandbox;
using Sandbox.Internal;
using Sandbox.UI;
using Sandbox.UI.Construct;
using SandboxEditor;
using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;

[Library("plates_screen_round_queue", Description = "A screen that displays the top ranks in a leaderboard"), HammerEntity]
[EditorModel("models/leaderboard_screen.vmdl")]
public partial class RoundQueueScreen : Prop
{
    public static RoundQueueScreen Instance = null;
    public List<string> RoundQueue = new();
    private RoundQueueScreenUI screen = null;
    private RealTimeSince timer = 0f;

    public override void Spawn()
    {
        base.Spawn();
        SetModel("models/leaderboard_screen.vmdl");
        SetupPhysicsFromModel(PhysicsMotionType.Static);
        EnableAllCollisions = true;
        Tags.Add("solid");
        Instance = this;
    }

    public override void ClientSpawn()
    {
        screen = new RoundQueueScreenUI(this, Scale);
        screen.Position = Position + (Rotation.Up * (64 * Scale)) + (Rotation.Left * (5.25f * Scale));
        screen.Rotation = Rotation.LookAt(Rotation.Left);

        PlatesGame.RequestRoundQueueForScreen();

        Instance = this;
    }

    [Event.Tick.Client]
    public void ClientTick()
    {
        if(timer > 5f)
        {
            if(RoundQueue.Count == 0)
            {
                PlatesGame.RequestRoundQueueForScreen();
            }
            timer = 0f;
        }
    }

    [ClientRpc]
    public static void Populate(string[] rounds)
    {
        if(Instance.RoundQueue.Count == 0)
        {
            Instance.RoundQueue = rounds.ToList();
            Instance?.screen?.Populate(Instance);
        }
    }

    [ClientRpc]
    public static void AddRound(string name)
    {
        Instance?.screen?.AddRound(name);
    }

    [ClientRpc]
    public static void RemoveLatest()
    {
        Instance?.screen?.RemoveLatest();
    }
}

public partial class RoundQueueScreenUI : WorldPanel
{
    public RoundQueueScreen owner;
    public Label Header;
    public Panel Content;

    public RoundQueueScreenUI(){}
    public RoundQueueScreenUI(RoundQueueScreen own, float scale)
    {
        StyleSheet.Load("/entities/map/roundqueuescreen.scss");

        Header = Add.Label("Round Queue", "header");
        //Add.Label("This screen is WIP and is not working properly yet", "subheader");
        Content = Add.Panel("content");

        owner = own;

        var width = 96 * 20 * scale;
        var height = 128 * 20 * scale;
        PanelBounds = new Rect(-width * .5f, -height * .5f, width, height);
    }

    // [Event.Tick.Client]
    // public void ClientTick()
    // {
    //     if(TimeSinceUpdate > 5f)
    //     {
    //         if(Content.ChildrenCount == 0)
    //         {
    //             Populate();
    //         }
    //         TimeSinceUpdate = 0f;
    //     }
    // }

    public void Populate(RoundQueueScreen own)
    {
        Content.DeleteChildren();
        owner = own;
        foreach(var round in owner.RoundQueue)
        {
            AddRound(round);
        }
    }

    public void AddRound(string name)
    {
        var e = Content.AddChild<RoundQueueScreenUIEntry>();
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

    public void RemoveLatest()
    {
        Content.Children.ToList()[0]?.Delete();
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