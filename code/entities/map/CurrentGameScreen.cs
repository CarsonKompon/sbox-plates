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
    public static CurrentGameScreen Instance;
    public Dictionary<long, string> InGame = new();
    public Dictionary<long, string> Eliminated;
    private CurrentGameScreenUI screen = null;
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
        screen = new CurrentGameScreenUI(this, Scale);
        screen.Position = Position + (Rotation.Up * (64 * Scale)) + (Rotation.Left * (5.25f * Scale));
        screen.Rotation = Rotation.LookAt(Rotation.Left);

        Instance = this;
    }

    [Event.Tick.Client]
    public void ClientTick()
    {
        if(timer > 5f)
        {
            if((int)PlatesGame.GameState > (int)PlatesGameState.STARTING_SOON && InGame.Count == 0)
            {
                PlatesGame.RequestGamePlayersForScreen();
            }
            timer = 0f;
        }
    }

    [ClientRpc]
    public static void ClearList()
    {
        Instance?.screen?.ClearList();
    }

    [ClientRpc]
    public static void Populate(long[] ingameIds, string[] ingameNames)
    {
        Dictionary<long, string> ingame = new();
        for(int i=0; i<ingameIds.Length; i++)
        {
            ingame.Add(ingameIds[i], ingameNames[i]);
        }
        Instance.InGame = ingame;
        Instance?.screen?.Populate(Instance);
    }
}

public partial class CurrentGameScreenUI : WorldPanel
{
    public CurrentGameScreen owner;
    public Label Header;
    public Label Subheader;
    public Label SubheaderLower;
    public Panel ContentInGame;
    public Panel ContentEliminated;

    public CurrentGameScreenUI(){}
    public CurrentGameScreenUI(CurrentGameScreen own, float scale)
    {
        StyleSheet.Load("/entities/map/currentgamescreen.scss");

        Header = Add.Label("Current Round", "header");
        Subheader = Add.Label("No active game", "subheader");

        ContentInGame = Add.Panel("content");

        //SubheaderLower = Add.Label("", "subheader-lower");
        //ContentEliminated = Add.Panel("content eliminated");

        owner = own;

        var width = 96 * 20 * scale;
        var height = 128 * 20 * scale;
        PanelBounds = new Rect(-width * .5f, -height * .5f, width, height);
    }

    public void ClearList()
    {
        ContentInGame.DeleteChildren();
        //ContentEliminated.DeleteChildren();
        Subheader.Text = "No active game";
    }

    public void Populate(CurrentGameScreen own = null)
    {
        ContentInGame.DeleteChildren();
        //ContentEliminated.DeleteChildren();
        if(own != null) owner = own;
        if(owner != null)
        {
            foreach(var ingame in owner.InGame)
            {
                AddInGame(ingame.Key, ingame.Value);
            }
            // foreach(var elim in owner.Eliminated)
            // {
            //     AddEliminated(elim.Key, elim.Value);
            // }
        }
    }

    public void AddInGame(long id, string name)
    {
        var e = ContentInGame.AddChild<CurrentGameScreenUIEntry>();
        e.Avatar.SetTexture($"avatar:{id}");
        e.Name.Text = name;
        e.PlayerId = id;
        SetText();
    }

    public void AddEliminated(long id, string name)
    {
        var e = ContentEliminated.AddChild<CurrentGameScreenUIEntry>();
        e.Avatar.SetTexture($"avatar:{id}");
        e.Name.Text = name;
        e.PlayerId = id;
        SetText();
    }

    public void SetText()
    {
        Subheader.Text = owner.InGame.Count.ToString() + " players remaining";
        // if(owner.Eliminated.Count > 0)
        // {
        //     SubheaderLower.Text = owner.Eliminated.Count.ToString() + " players eliminated";
        // }
        // else
        // {
        //     SubheaderLower.Text = "";
        // }
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