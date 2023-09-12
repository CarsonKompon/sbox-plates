using Sandbox;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Plates;

public partial class PlatesGame
{
    public static PlatesEvent CurrentEvent {get;set;} = new();
	public static List<PlatesEvent> EventQueue {get;set;} = new();

    public static List<PlatesEvent> Events = new List<PlatesEvent>();
	public static List<PlatesRound> RoundTypes = new List<PlatesRound>();
	public static List<PlatesRound> RoundQueue {get;set;} = new();


    [Event.Hotload] // Reload Events on Hotload (Makes life easier when developing)
	public static void LoadEvents()
	{
		// Re-initialize list
		Events = new();
		RoundTypes = new();
		// Populate it with classes that match the attribute
		foreach(TypeDescription _td in TypeLibrary.GetTypes<PlatesEvent>()){
			Events.Add(TypeLibrary.Create<PlatesEvent>(_td.TargetType));
		}
		foreach(TypeDescription _td in TypeLibrary.GetTypes<PlatesRound>()){
			RoundTypes.Add(TypeLibrary.Create<PlatesRound>(_td.TargetType));
		}
	}

    /// <summary>
	/// Gets the next event in the queue or adds a new one if none exist
	/// </summary>
	public void GetNextEvent()
	{
		GameState = PlatesGameState.SELECTING_EVENT;
		EventSubtext = "";
		LastTimer = -10f;

		PlayerDataManager.GiveAllMoney(10);

		ResetGlows();

		Random Rand = new();

		if(EventQueue.Count > 0)
		{
			CurrentEvent = EventQueue[0];
			EventQueue.RemoveAt(0);
		}
		else
		{
            do
            {
                CurrentEvent = Rand.FromList(Events);
            }
            while(CurrentEvent.hidden || CurrentEvent.name == "nothing");
		}

		AffectedPlayers = Rand.Int(CurrentEvent.minAffected, CurrentEvent.maxAffected);
		TotalAffectedPlayers = AffectedPlayers;
		
		GameTimer = -4f;

		// Check if game end
		var finishCount = 1;
		if(StartingPlayerCount == 1) finishCount = 0;
		if(GameClients.Count <= finishCount) EndGame();
	}

	public void PerformEvent()
	{
		GameState = PlatesGameState.PERFORMING_EVENT;
		Entity ent;
		ResetGlows();

		AffectedPlayers--;
		GameTimer = -1f;

		Random Rand = new();

		switch(CurrentEvent.type)
		{
			case EventType.Player:
				if(GameClients.Count == 0) return;
				IClient ply = Rand.FromList<IClient>(GameClients.ToList<IClient>());
				ent = (Entity)ply.Pawn;
				EventSubtext = EventSubtext + ply.Name;
				CurrentEvent.OnEvent(ent);
				(ply.Pawn as Player).EventCount++;
				break;
			case EventType.Plate:
				if(Entity.All.OfType<Plate>().ToArray().Length == 0) return;
				var attempts = 0;
				Plate plat = null;
				while(plat == null)
				{
					plat = Rand.FromList(Entity.All.OfType<Plate>().ToList());
					if(plat.isDead)
					{
						plat = null;
						attempts++;
						if(attempts > 50) return;
					}
				}
				ent = plat;
				EventSubtext = EventSubtext + plat.ownerName;
				CurrentEvent.OnEvent(ent as Plate);
                if(plat.owner is IClient)
                {
                    if(plat.owner.Pawn is Player player) player.EventCount++;
                }
				break;
			default:
				ent = null;
				CurrentEvent.OnEvent();
				EventSubtext = CurrentEvent.subtext;
				break;
		}

		if(CurrentEvent.type == EventType.Arena)
		{
			Sound.FromWorld("plates_buzzer", Vector3.Zero);
		}
		else
		{
			Sound.FromEntity("plates_buzzer", ent);
			if(ent is Plate _plate) _plate.SetGlow(true, Color.Blue);
			if(ent is Player _player) _player.SetGlow(true, Color.Blue);
		}

		if(AffectedPlayers == 0) GameTimer = -2f;
		else EventSubtext = EventSubtext + ", ";
	}

    public static PlatesEvent GetEventFromCommand(string eventName)
	{
		foreach(var _ev in Events)
		{
			if(_ev.command == eventName)
			{
				return _ev;
			}
		}
		return null;
	}

    public static PlatesRound GetRoundFromCommand(string roundName)
	{
		foreach(var _round in RoundTypes)
		{
			if(_round.command == roundName)
			{
				return _round;
			}
		}
		return null;
	}

	/// <summary>
	/// Fills the queue
	/// </summary>
	public static void FillQueue(int amount = 4)
	{
		while(RoundQueue.Count < amount)
		{
			QueueRound();
		}
	}

    /// <summary>
	/// Add an Event to the queue for the current game
	/// </summary>
	[ConCmd.Admin("plates_event", Help = "Adds an event to the queue for the current game")]
	public static void QueueEvent(string eventName)
	{
		var _event = GetEventFromCommand(eventName);
		if(_event == null)
		{
			Log.Error("PLATES: There is no event with name " + eventName);
		}
		else
		{
			EventQueue.Add(_event);
			Log.Info("PLATES: " + eventName + " added to event queue");
		}
	}

    /// <summary>
	/// Add a Round to the queue for the round list
	/// </summary>
	[ConCmd.Admin("plates_round", Help = "Adds a round to the queue for the round list")]
	public static void QueueRound(string roundName)
	{
		var round = GetRoundFromCommand(roundName);
		if(round == null)
		{
			Log.Error("PLATES: There is no round with name " + roundName);
		}
		else
		{
			QueueRound(round);
            Log.Info("PLATES: " + roundName + " added to round queue");
		}
    }

    public static void QueueRound(PlatesRound round = null)
    {
		if(round == null)
		{
			Random Rand = new();
			do
			{
				round = Rand.FromList(RoundTypes);
			}
			while(RoundQueue.Count > 0 && round == RoundQueue[RoundQueue.Count-1]);
		}
        RoundQueue.Add(round);
        RoundQueueScreen.AddRound(round.name);
    }
}