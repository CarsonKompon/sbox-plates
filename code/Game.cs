using Sandbox;
using System;
using System.Linq;
using System.Collections.Generic;

public enum PlatesGameState {NOT_ENOUGH_PLAYERS = -2, GAME_OVER = -1, STARTING_SOON = 0, SELECTING_EVENT = 1, PERFORMING_EVENT = 2}

public partial class PlatesGame : Sandbox.Game
{
	[Net] public static PlatesGameState GameState {get;set;} = PlatesGameState.STARTING_SOON;

	// Game-related variables:
	[Net] public static RealTimeSince GameTimer {get;set;} = -30f;
	[Net] public static RealTimeSince GameLength {get;set;} = 0f;
	[Net] public static float LastTimer {get;set;} = -10f;
	[Net] public static List<Client> GameClients {get;set;} = new();
	[Net] public static List<LossInformation> Eliminated {get;set;} = new();
	[Net] public static List<Entity> GameEntities {get;set;} = new();
	[Net] public static int StartingPlayerCount {get;set;} = 1;
	[Net] public static PlatesEventAttribute CurrentEvent {get;set;} = new();
	[Net] public static List<PlatesEventAttribute> EventQueue {get;set;} = new();
	[Net] public static PlatesRoundAttribute GameRound {get;set;}
	[Net] public static int AffectedPlayers {get;set;} = 0;
	[Net] public static int TotalAffectedPlayers {get;set;} = 0;


	// Event and Queue-related variables:
	public static List<PlatesEventAttribute> Events = new List<PlatesEventAttribute>();
	public static List<PlatesRoundAttribute> RoundTypes = new List<PlatesRoundAttribute>();
	[Net] public static List<PlatesRoundAttribute> RoundQueue {get;set;} = new();

	// Networked text
	[Net] public static string EventText {get;set;} = "";
	[Net] public static string EventSubtext {get;set;} = "";

	public PlatesGame()
	{
		if(IsServer)
		{
			// Create the HUD Instance
			new PlatesHud();

			// Load the game events
			LoadEvents();
		}
	}

	[Event.Hotload] // Reload Events on Hotload (Makes life easier when developing)
	public static void LoadEvents()
	{
		// Re-initialize list
		Events = new();
		RoundTypes = new();
		// Populate it with classes that match the attribute
		foreach(TypeDescription _td in TypeLibrary.GetDescriptions<PlatesEventAttribute>()){
			Events.Add(TypeLibrary.Create<PlatesEventAttribute>(_td.TargetType));
		}
		foreach(TypeDescription _td in TypeLibrary.GetDescriptions<PlatesRoundAttribute>()){
			RoundTypes.Add(TypeLibrary.Create<PlatesRoundAttribute>(_td.TargetType));
		}
	}

	/// <summary>
	/// A client joined the server. Give them a player to play with
	/// </summary>
	public override void ClientJoined( Client client )
	{
		base.ClientJoined( client );

		// Create a player for this client to play with
		var player = new PlatesPlayer( client );
		player.Respawn();
		client.Pawn = player;

		// Set client variables
		GetClientRank(client);
	}
	/// <summary>
	/// A client is killed. Eliminate them if they were in-game
	/// </summary>
	public override void OnKilled( Client client, Entity pawn )
	{
		base.OnKilled(client, pawn);
		
		foreach(var plate in Entity.All.OfType<Plate>())
		{
			if(plate.owner == client)
			{
				plate.Kill();
				break;
			}
		}

		// Remove client from game clients list
		SetLose(client);
		GameClients.Remove(client);
	}

	[Event.Tick]
	public void Tick()
	{
		if(!IsServer) return;

		switch(GameState)
		{
			case PlatesGameState.NOT_ENOUGH_PLAYERS:
				EventText = "Not Enough Players!";
				EventSubtext = "Type plates_start in console to start anyway";
				if(GameTimer >= 0)
				{
					GameState = PlatesGameState.STARTING_SOON;
					GameTimer = -30f;
				}
				break;
			case PlatesGameState.GAME_OVER:
				EventText = "Game Over! Winner: " + GetWinner().Name;
				if(GameTimer >= 0)
				{
					for(var i=0; i<GameClients.Count; i++)
					{
						(GameClients[i].Pawn as PlatesPlayer)?.Respawn();
					}
					GameTimer = -10f;
					GameState = PlatesGameState.STARTING_SOON;
				}
				break;
			case PlatesGameState.STARTING_SOON:
				EventText = "Game Starting in " + MathF.Ceiling(-GameTimer).ToString() + "s";
				EventSubtext = "";
				if(GameTimer >= 0)
				{
					if(Client.All.Count > 1)
					{
						StartGame();
					}
					else
					{
						GameTimer = -8f;
						GameState = PlatesGameState.NOT_ENOUGH_PLAYERS;
					}
				}
				break;
			default:
				var prevTime = GameTimer;
				if(GameState == PlatesGameState.SELECTING_EVENT)
				{
					if(GameTimer <= 0f && Math.Floor(GameTimer) > LastTimer)
					{
						Sound.FromScreen("plates_timer");
						LastTimer = (float)Math.Floor(GameTimer);
					}
					var str = TotalAffectedPlayers + CurrentEvent.text;
					if(CurrentEvent.type == EventType.Arena) str = CurrentEvent.text;
					if(GameTimer >= 0) EventText = str + "0s";
					else EventText = str + MathF.Ceiling(-GameTimer).ToString() + "s";
				}
				if(GameTimer >= 0)
				{
					if(AffectedPlayers <= 0) GetNextEvent();
					else PerformEvent();
				}
				break;
		}

		HeaderText.updateText(EventText, EventSubtext);
	}

	/// <summary>
	/// Starts a game of Plates assuming one isn't already in progress
	/// </summary>
	[ConCmd.Admin("plates_start", Help = "Forces the game to start if one isn't already active")]
	public static void StartGame()
	{
		// If game is already active, do nothing
		if((int)GameState > (int)PlatesGameState.STARTING_SOON) return;

		// Start keeping track of game length
		GameLength = 0f;

		// Respawn all players
		RespawnAllPlayers();

		// Clear all entries from the previous Round Report
		RoundReport.ClearEntries();

		// Add clients to the GameClients list
		GameClients = new();
		Eliminated = new();
		for(var i=0; i<Client.All.Count; i++)
		{
			var client = Client.All[i];
			GameClients.Add(client);
			GetClientRank(client);
		}

		// Keep track of how many players we started with
		StartingPlayerCount = Client.All.Count;

		// Initialize plates and assign a plate to each player
		InitPlates();
		AssignPlates();

		// Set the round type if there's one in the queue
		if(RoundQueue.Count > 0)
		{
			GameRound = RoundQueue[0];
			GameRound.OnEvent();
			RoundQueue.RemoveAt(0);
		}
		else
		{
			// If not one in the queue, there's a 50% chance to choose a random one
			if(Rand.Int(0,1) == 0)
			{
				GameRound = Rand.FromList(RoundTypes);
				GameRound.OnEvent();
			}
			else
			{
				GameRound = new PlatesRoundAttribute();
			}
		}

		RoundInfo.SetRoundText(GameRound.name, GameRound.description);

		GetNextEvent();

		GameServices.StartGame();
	}

	/// <summary>
	/// Ends a game of Plates assuming one is already in progress
	/// </summary>
	public static void EndGame()
	{
		// Make sure winners are set proper
		foreach(var client in GameClients)
		{
			SetLose(client);
		}

		// Set winners podiums
		foreach(var podium in Entity.All.OfType<WinnersPodium>().ToList())
		{
			if(podium.IsValid() && podium.WinPosition <= Eliminated.Count){
				podium.Dress(Eliminated[Eliminated.Count - podium.WinPosition].client);
			}
		}

		// Set player win condition
		var _winner = GetWinner();
		_winner.SetGameResult(GameplayResult.Win);

		// Play Round End Music
		var _r = Rand.Int(1, 9);
		Sound.FromScreen("plates_round_end_" + _r);

		// Show Round Report UI
		RoundReport.Show();

		foreach(var cl in GameClients)
		{
			(cl.Pawn as PlatesPlayer).Respawn();
		}
		GameClients = new();
		foreach(var plate in Entity.All.OfType<Plate>()) plate.Delete();
		foreach(var ent in GameEntities)
		{
			if(ent.IsValid()) ent.Delete();
		}
		GameEntities = new();
		GameTimer = -10;
		GameState = PlatesGameState.GAME_OVER;

		GameServices.EndGame();
	}

	/// <summary>
	/// Gets the next event in the queue or adds a new one if none exist
	/// </summary>
	public static void GetNextEvent()
	{
		GameState = PlatesGameState.SELECTING_EVENT;
		EventSubtext = "";
		LastTimer = -10f;

		PlatesPlayer.GiveMoney(10);

		ResetGlows();

		if(EventQueue.Count > 0)
		{
			CurrentEvent = EventQueue[0];
			EventQueue.RemoveAt(0);
		}
		else
		{
			CurrentEvent = Rand.FromList(Events);
		}

		AffectedPlayers = Rand.Int(CurrentEvent.minAffected, CurrentEvent.maxAffected);
		TotalAffectedPlayers = AffectedPlayers;
		
		GameTimer = -4f;

		// Check if game end
		var finishCount = 1;
		if(StartingPlayerCount == 1) finishCount = 0;
		if(GameClients.Count <= finishCount) EndGame();
	}

	public static void PerformEvent()
	{
		GameState = PlatesGameState.PERFORMING_EVENT;
		Entity ent;
		ResetGlows();

		AffectedPlayers--;
		GameTimer = -1f;

		switch(CurrentEvent.type)
		{
			case EventType.Player:
				if(GameClients.Count == 0) return;
				var ply = Rand.FromList(GameClients);
				ent = ply.Pawn;
				EventSubtext = EventSubtext + ply.Name;
				CurrentEvent.OnEvent(ent);
				GameServices.RecordEvent(ply, "Player Event: " + CurrentEvent.Name);
				(ply.Pawn as PlatesPlayer).EventCount++;
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
				GameServices.RecordEvent(plat.owner, "Plate Event: " + CurrentEvent.Name);
				(plat.owner.Pawn as PlatesPlayer).EventCount++;
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
			if(ent is PlatesPlayer _player) _player.SetGlow(true, Color.Blue);
		}

		if(AffectedPlayers == 0) GameTimer = -2f;
		else EventSubtext = EventSubtext + ", ";
	}

	public static async void GetClientRank(Client client)
	{
		var gameRank = await client.FetchGameRankAsync();
		client.SetInt("wins", gameRank.Level);
		client.SetInt("rank", gameRank.Wins);
	}

	/// <summary>
	/// Spawn a grid of plates
	/// </summary>
	public static void InitPlates()
	{
		for(var i=-4; i<4; i++){
			for(var j=-4; j<4; j++){
				Plate plate = new Plate(new Vector3((i+0.5f)*92*4,(j+0.5f)*92*4,0), 1, "Nobody");
			}
		}
	}

	/// <summary>
	/// Assigns a plate to each player and destroys the excess
	/// </summary>
	public static void AssignPlates()
	{
		var _playerCount = Client.All.Count;
		var _curPlayer = 0;

		foreach(var plate in Entity.All.OfType<Plate>().OrderBy(x => Rand.Double()))
		{
			if(_curPlayer >= _playerCount)
			{
				plate.Delete();
			}
			else
			{
				var client = Client.All[_curPlayer];
				plate.owner = client;
				plate.ownerName = plate.owner.Name;
				if(client.Pawn is PlatesPlayer ply)
				{
					ply.CurrentPlate = plate;
					ply.InGame = true;
				}
				Client.All[_curPlayer].Pawn.Position = plate.Position + Vector3.Up * 100.0f;
			}
			_curPlayer++;
		}
	}

	public static void AddEntity(Entity ent)
	{
		GameEntities.Add(ent);
	}

	public static Client GetWinner()
	{
		return Eliminated[Eliminated.Count-1].client;
	}

	public static void SetLose(Client client)
	{
		if(GameClients.Contains(client)){
			var _loss = new LossInformation(client, GameLength, (client.Pawn as PlatesPlayer).EventCount, GameClients.Count);
			Eliminated.Add(_loss);
			RoundReport.AddEntry(_loss.position, _loss.client.PlayerId, _loss.client.Name, _loss.timeAlive, _loss.eventCount);
		}
	}

	public static void ResetGlows()
	{
		foreach(var plate in Entity.All.OfType<Plate>())
		{
			plate.SetGlow(false);
		}
		foreach(var client in Client.All)
		{
			if(client.Pawn is PlatesPlayer player)
			{
				player.SetGlow(false);
			}
		}
	}

	public static PlatesEventAttribute GetEventFromName(string eventName)
	{
		foreach(var _ev in Events)
		{
			if(_ev.name == eventName)
			{
				return _ev;
			}
		}
		return null;
	}

	public override void DoPlayerNoclip( Client player ) {}

	public override void DoPlayerDevCam( Client client )
	{
		if(client.Pawn is PlatesPlayer ply)
		{
			if(!ply.InGame) base.DoPlayerDevCam( client );
		}
	}

	/// <summary>
	/// Respawn all connected clients
	/// </summary>
	public static void RespawnAllPlayers()
	{
		for(var i=0; i<Client.All.Count; i++)
		{
			(Client.All[i].Pawn as PlatesPlayer)?.Respawn();
		}
	}

	/// </summary>
	/// Sends a log to the kill feed
	/// </summary>
	[ClientRpc]
	public override void OnKilledMessage(long leftid, string left, long rightid, string right, string method)
	{
		var _showKill = false;
		foreach(Client cl in Client.All)
		{
			if(cl.PlayerId == rightid && cl.Pawn is PlatesPlayer ply)
			{
				_showKill = ply.InGame;
				break;
			}
		}
		if(_showKill) KillFeed.Current?.AddEntry(leftid, left, rightid, right, method);
	}

	/// <summary>
	/// Add an Event to the queue for the current game
	/// </summary>
	[ConCmd.Admin("plates_event", Help = "Adds an event to the queue for the current game")]
	public static void QueueEvent(string eventName)
	{
		var _event = GetEventFromName(eventName);
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

}

public class LossInformation : BaseNetworkable
{
	public Client client = null;
	public float timeAlive = 0f;
	public int eventCount = 0;
	public int position = 1;

	public LossInformation(Client _client, float _time, int _events, int _position)
	{
		client = _client;
		timeAlive = _time;
		eventCount = _events;
		position = _position;
	}
}