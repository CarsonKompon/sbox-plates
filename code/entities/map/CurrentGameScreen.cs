using Sandbox;
using Sandbox.Internal;
using Sandbox.UI;
using Sandbox.UI.Construct;
using SandboxEditor;
using System;
using System.Net;
using System.Collections.Generic;

[Library("plates_screen_current_game", Description = "A screen that displays the top ranks in a leaderboard"), HammerEntity]
[EditorModel("models/leaderboard_screen.vmdl")]
public partial class CurrentGameScreen : Prop
{
    private CurrentGameScreenUI screen = null;
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
            screen = new CurrentGameScreenUI(Scale);
            screen.Position = Position + (Rotation.Up * (64 * Scale)) + (Rotation.Left * (5.25f * Scale));
            screen.Rotation = Rotation.LookAt(Rotation.Left);
        }
    }
}

public partial class CurrentGameScreenUI : WorldPanel
{
    public RealTimeSince TimeSinceUpdate = 0f;
    public Label Header;
    public Label Subheader;
    public Panel Content;
    public static CurrentGameScreenUI Instance;
    public static List<CurrentGameScreenUIEntry> Entries = new();

    public CurrentGameScreenUI(){}
    public CurrentGameScreenUI(float scale)
    {
        StyleSheet.Load("/entities/map/currentgamescreen.scss");

        Header = Add.Label("Current Round", "header");
        Subheader = Add.Label("No active game", "subheader");

        Content = Add.Panel("content");

        var width = 96 * 20 * scale;
        var height = 128 * 20 * scale;
        PanelBounds = new Rect(-width * .5f, -height * .5f, width, height);

        Instance = this;
        PopulateList();
    }

    [ClientRpc]
    public static void ClearList()
    {
        foreach(var entry in Entries)
        {
            entry.Delete();
        }
        Entries.Clear();
        Instance.Subheader.Text = "No active game";
    }

    [ClientRpc]
    public static void PopulateList()
    {
        if(Entries.Count > 0) ClearList();
        foreach(var cl in PlatesGame.GameClients)
        {
            AddClient(cl);
        }
        SetText();
    }

    [ClientRpc]
    public static void AddClient(Client cl)
    {
        var e = Instance.Content.AddChild<CurrentGameScreenUIEntry>();
        e.Avatar.SetTexture($"avatar:{cl.PlayerId}");
        e.Name.Text = cl.Name;
        e.PlayerId = cl.PlayerId;
        Entries.Add(e);
        SetText();
    }

    [ClientRpc]
    public static void RemoveClient(Client cl)
    {
        Log.Info("WHOA: " + cl.Name);
        foreach(var entry in Entries)
        {
            if(entry.PlayerId == cl.PlayerId)
            {
                Entries.Remove(entry);
                entry.Delete();
                SetText();
                break;
            }
        }
    }

    public static void SetText()
    {
        Instance.Subheader.Text = Entries.Count.ToString() + " players remaining";
    }
}

public class CurrentGameScreenUIEntry : Panel
{
    public long PlayerId;
    public Image Avatar;
    public Label Name;

    public CurrentGameScreenUIEntry()
    {
        Avatar = Add.Image("", "avatar");
        Name = Add.Label("", "name");
    }
}