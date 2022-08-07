using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class RoundReport : Panel
{
    public static RoundReport instance;
    public Panel Header { get; protected set; }
    public Panel SubHeader {get; protected set; }
    public static Panel Canvas {get; set; }
    public static List<RoundReportEntry> Entries = new();
	private Label headerLabel;
    private static RealTimeSince Timer = 0f;

	public RoundReport(){
        StyleSheet.Load( "/ui/screen/roundreport.scss" );
        AddClass( "roundreport" );

        Header = Add.Panel( "header" );
        headerLabel = Header.Add.Label( "Round Report", "round" );

        Canvas = Header.Add.Panel("canvas");
        SubHeader = Canvas.Add.Panel( "subheader" );
		SubHeader.Add.Label( "Place", "place" );
		SubHeader.Add.Label( "Player", "name" );
		SubHeader.Add.Label( "Time", "time" );
		SubHeader.Add.Label( "Events", "events" );

        instance = this;
    }

    public override void Tick()
    {
        var _open = Timer < 0f && !Input.Down(InputButton.Score);
        SetClass("open", _open);
        //Canvas.SetClass( "open", _open);

        Canvas.SortChildren<RoundReportEntry>((x) => (x.position));

        foreach(var entry in Entries)
        {
            var _timer = (1f-((Timer + 30f) / 15f))*64f + Entries.Count;
            var _flag = _timer < entry.position; //&& ((_timer+.6f) * Entries.Count > entry.position || entry.position <= 10);
            entry.SetClass("show", _flag);
        }
    }

	[ClientRpc]
	public static void Show(){
		Timer = -15f;
	}

    [ClientRpc]
    public static void AddEntry(int position, long id, string name, float time, int events)
    {
        var _entry = Canvas.AddChild<RoundReportEntry>();
        _entry.SetInfo(position, id, name, time, events);
        Entries.Add(_entry);
    }

    [ClientRpc]
    public static void ClearEntries()
    {
        foreach ( var entry in Entries )
        {
            entry?.Delete();
        }
        Entries.Clear();
    }
}

public partial class RoundReportEntry : Panel
{
    public int position = 1;
    public Label labelPlace;
    public Label labelName;
    public Label labelTime;
    public Label labelEvents;

    public RoundReportEntry()
    {
        AddClass("entry");

        labelPlace = Add.Label("1st", "place");
        labelName = Add.Label("Missingname.", "name");
        labelTime = Add.Label("00:00", "time");
        labelEvents = Add.Label("0", "events");
    }

    public void SetInfo(int pos, long id, string name, float time, int events)
    {
        position = pos;
        labelPlace.Text = "#" + pos.ToString();
        labelName.Text = name;
        labelName.Add.Image($"avatar:{id}");
        labelEvents.Text = events.ToString();

        var mins = MathF.Floor(time / 60);
        var secs = MathF.Floor(time - (mins*60));
        var minString = mins.ToString();
        if(mins < 10) minString = "0" + minString;
        var secString = secs.ToString();
        if(secs < 10) secString = "0" + secString;
        labelTime.Text = minString + ":" + secString;
    }
}