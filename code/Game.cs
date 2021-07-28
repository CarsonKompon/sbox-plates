
using Sandbox;
using Sandbox.UI.Construct;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;  

/// <summary>
/// This is your game class. This is an entity that is created serverside when
/// the game starts, and is replicated to the client. 
/// 
/// You can use this to create things like HUDs and declare which player class
/// to use for spawned players.
/// </summary>
[Library( "plates", Title = "Plates" )]
public partial class PlatesGame : Sandbox.Game
{

	[Net] public static float GameTimer {get;set;} = 30;
	[Net] public static int GameState {get;set;} = 0;
	[Net] public static string EventText {get;set;} = "";
	[Net] public static string EventSubtext {get;set;} = "";
	[Net] public static int StartingPlayers {get;set;} = 0;
	[Net] public static EventBase CurrentEvent {get;set;}
	[Net] public static RoundTypeBase CurrentRound {get;set;}
	[Net] public static EventBase NextEvent {get;set;} = null;
	[Net] public static RoundTypeBase NextRound {get;set;} = null;
	[Net] public static int AffectedPlayers {get;set;} = 0;
	[Net] public static int TotalAffectedPlayers {get;set;} = 0;
	[Net] public static List<Client> InGamePlayers {get;set;} = new();
	[Net] public static List<Client> Winners {get;set;} = new();
	[Net] public static List<Entity> GameEnts {get;set;} = new();

	public static List<EventBase> Events = new List<EventBase>();
	public static List<RoundTypeBase> RoundTypes = new List<RoundTypeBase>();


	public PlatesGame()
	{
		if ( IsServer )
		{
			Log.Info( "Plates Server Loaded!" );

			// Create a HUD entity. This entity is globally networked
			// and when it is created clientside it creates the actual
			// UI panels. You don't have to create your HUD via an entity,
			// this just feels like a nice neat way to do it.
			new PlatesHudEntity();

			//Load events from attribute
			InitEvents();
		}

		if ( IsClient )
		{
			Log.Info( "Plates Client Loaded!" );
		}

	}

	[Event.Hotload] //Reload Events on Hotload (Makes life easier while developing)
	public static void InitEvents(){
		Events = new();
		//Load events from attribute
		foreach(EventBase _ev in Library.GetAttributes<EventBase>()){
			Events.Add(_ev.Create<EventBase>());
		}
		foreach(RoundTypeBase _rb in Library.GetAttributes<RoundTypeBase>()){
			RoundTypes.Add(_rb.Create<RoundTypeBase>());
		}
	}

	public override void ClientJoined( Client client )
	{
		base.ClientJoined( client );

		var player = new PlatesPlayer();
		client.Pawn = player;

		player.Respawn();
	}


	[Event.Tick]
	public void Tick()
	{
		if(IsServer){
			if(GameState == -2){
				GameTimer -= 1.0f/60.0f;
				EventText = "Not Enough Players!";
				EventSubtext = "Type plates_start in console to start anyway";
				if(MathF.Ceiling(GameTimer) <= 0){
					GameState = 0;
					GameTimer = 30;
				}
			}else if(GameState == -1){
				GameTimer -= 1.0f/60.0f;
				EventText = "Game Over! Winner: " + Winners[Winners.Count-1].Name;
				if(MathF.Ceiling(GameTimer) <= 0){
					for(var i=0;i<InGamePlayers.Count;i++){
						(InGamePlayers[i].Pawn as PlatesPlayer)?.Respawn();
					}
					GameTimer = 15;
					GameState = 0;
				}
			}else if(GameState == 0){
				//Game Starting in ...
				GameTimer -= 1.0f/60.0f;
				EventText = "Game Starting in " + MathF.Ceiling(GameTimer).ToString() + "s";
				EventSubtext = "";
				if(MathF.Ceiling(GameTimer) <= 0){
					if(Client.All.Count > 1){
						StartGame();
					}else{
						GameTimer = 8;
						GameState = -2;
					}
				}
			}else if(GameState >= 1){
				//In-Game
				var prevTime = GameTimer;
				GameTimer -= 1.0f/60.0f;
				if(GameState == 1){
					if(GameTimer > 0.0f && MathF.Floor(GameTimer)<MathF.Floor(prevTime)) Sound.FromScreen("plates_timer");
					var str = TotalAffectedPlayers + CurrentEvent.text;
					if(CurrentEvent.type == EventType.Arena) str = CurrentEvent.text;
					if(GameTimer <= 0) EventText = str + "0s";
					else EventText = str + MathF.Ceiling(GameTimer).ToString() + "s";
				}
				if(MathF.Ceiling(GameTimer) <= 0){
					if(AffectedPlayers <= 0) GetNewEvent();
					else PerformEvent();
				}
			}
			EventHud.updateText(EventText);
			EventSub.updateText(EventSubtext);
		}
	}

	//Game Start
	[ServerCmd("plates_start", Help = "Skips the waiting for players timer")]
	public static void StartGame(){
		StartingPlayers = Client.All.Count;
		ResetPlayers();
		InGamePlayers = new();
		Winners = new();
		for(var i=0;i<Client.All.Count;i++){
			InGamePlayers.Add(Client.All[i]);
		}
		InitPlates();
		AssignPlates();

		//Round Type Check
		//TODO: Make UI for what round type a game is on start
		if(NextRound != null){
			CurrentRound = NextRound;
			CurrentRound.OnEvent();
			NextRound = null;
		}else{
			if(Rand.Int(0,1) == 0){ //50% chance of a random round type
				CurrentRound = Rand.FromList(RoundTypes);
				CurrentRound.OnEvent();
			}else{ //Otherwise, Normal Round
				CurrentRound = new RoundTypeBase();
			}
		}
		RoundInfo.NewRound(CurrentRound.name);
		//Log.Info("Current Round Type: " + CurrentRound.name);

		GetNewEvent();
		GameState = 1;
	}

	public static void GetNewEvent(){
		GameState = 1;
		EventSubtext = "";

		ResetGlows();
		
		if(NextEvent != null) CurrentEvent = NextEvent;
		else CurrentEvent = Rand.FromList(Events);
		NextEvent = null;

		AffectedPlayers = Rand.Int(CurrentEvent.minAffected,CurrentEvent.maxAffected-1);
		TotalAffectedPlayers = AffectedPlayers;
		GameTimer = 4;
		
		//Check if game end
		var finishCount = 1;
		if(StartingPlayers == 1) finishCount = 0;
		if(InGamePlayers.Count <= finishCount){
			EndGame();
		}
	}

	public static void PerformEvent(){
		GameState = 2;
		Entity ent;
		ResetGlows();
		if(CurrentEvent.type == EventType.Player){
			if(InGamePlayers.Count == 0){
				AffectedPlayers--;
				GameTimer = 1;
				return;
			}
			var ply = InGamePlayers[Rand.Int(0,InGamePlayers.Count-1)];
			ent = ply.Pawn;
			EventSubtext = EventSubtext + ply.Name;
			CurrentEvent.OnEvent(ent);
		}else if(CurrentEvent.type == EventType.Plate){
			if(Entity.All.OfType<Plate>().ToArray().Length == 0){
				AffectedPlayers--;
				GameTimer = 1;
				return;
			}
			var plat = Entity.All.OfType<Plate>().OrderBy(x => Rand.Double()).ToArray()[0];
			ent = plat;
			EventSubtext = EventSubtext + plat.ownerName;
			CurrentEvent.OnEvent(ent as Plate);
		}else{
			ent = null;
			CurrentEvent.OnEvent();
			EventSubtext = CurrentEvent.subtext;
		}
		// CurrentEvent.OnEvent(ent);
		if(CurrentEvent.type == EventType.Arena){
			Sound.FromWorld("plates_buzzer", Vector3.Zero);
		}else{
			Sound.FromEntity("plates_buzzer", ent);
			if(ent is ModelEntity _ent) _ent.GlowActive = true;
		}

		AffectedPlayers--;
		GameTimer = 1;
		if(AffectedPlayers == 0) GameTimer = 2;
		else EventSubtext = EventSubtext + ", ";
	}

	public static void SetGlows(bool active){
		foreach(var plate in Entity.All.OfType<Plate>()){
			plate.GlowActive = active;
			plate.GlowColor = Color.Blue;
		}
		for(var i=0;i<InGamePlayers.Count;i++){
			if(InGamePlayers[i].Pawn is ModelEntity ply){
				ply.GlowActive = active;
				ply.GlowColor = Color.Blue;
			}
		}
	}

	public static void ResetGlows(){
		foreach(var plate in Entity.All.OfType<Plate>()){
			plate.GlowActive = false;
			plate.GlowColor = Color.Blue;
		}
		for(var i=0;i<InGamePlayers.Count;i++){
			if(InGamePlayers[i].Pawn is ModelEntity ply){
				ply.GlowActive = false;
				ply.GlowColor = Color.Blue;
			}
		}
	}

	public override void OnKilled( Client client, Entity pawn )
	{
		base.OnKilled( client, pawn );

		foreach(var plate in Entity.All.OfType<Plate>())
		{
			if(plate.owner == client.SteamId){
				plate.Kill();
				break;
			}
		}

		if(InGamePlayers.Contains(client)){
			Winners.Add(client);
			InGamePlayers.Remove(client);
		}
	}

	public static void EndGame(){
		EventSubtext = "";
		if(InGamePlayers.Count > 0) Winners.Add(InGamePlayers[0]);
		
		foreach(var podium in Entity.All.OfType<WinnersPodium>().ToList()){
			if(podium.IsValid() && podium.WinPosition <= Winners.Count){
				podium.SetModel( "models/citizen/citizen.vmdl" );
				podium.Dress(Winners[Winners.Count-podium.WinPosition].Pawn as PlatesPlayer);
			}
		}
		
		var _r = Rand.Int(1,17);
		Log.Info("plates_round_end_" + _r);
		Sound.FromScreen("plates_round_end_" + _r);

		//TODO: Round End UI

		foreach(var plate in Entity.All.OfType<Plate>()) plate.Kill();
		foreach(var ev in GameEnts){
			if(ev.IsValid()) ev.Delete();
		}
		GameEnts = new();
		GameTimer = 6;
		GameState = -1;
	}

	public static void ResetPlayers(){
		for(var i=0;i<Client.All.Count;i++){
			(Client.All[i].Pawn as PlatesPlayer)?.Respawn();
		}
	}

	public static void InitPlates(){
		for(var i=-4;i<4;i++){
			for(var j=-4;j<4;j++){
				Plate plate = new Plate(new Vector3((i+0.5f)*92*4,(j+0.5f)*92*4,0), 1, 0, "Nobody");
				// plate.Position = new Vector3((i+0.5f)*92*4,(j+0.5f)*92*4,0);
				// plate.owner = 0;
				// plate.ownerName = "Nobody";
			}
		}
	}

	public static void AssignPlates(){

		var _playerCount = Client.All.Count;
		var _curPlayer = 0;

		foreach(var plate in Entity.All.OfType<Plate>().OrderBy(x => Rand.Double()) )
		{
			 if(_curPlayer >= _playerCount){
				 plate.Delete();
			 }else{
				 plate.owner = Client.All[_curPlayer].SteamId;
				 plate.ownerName = Client.All[_curPlayer].Name;
				 (Client.All[_curPlayer].Pawn as PlatesPlayer).CurrentPlate = plate;
				 Client.All[_curPlayer].Pawn.Position = plate.Position + Vector3.Up * 100;
			 }
			 _curPlayer++;
		}

	}

	[ServerCmd( "plates_event", Help = "Sets the current event in the current game" )]
	public static void SetEventFromName(string eventName){
		foreach(var _ev in Events){
			if(_ev.name == eventName){
				NextEvent = _ev;
			}
		}
	}

	[ServerCmd( "plates_round", Help = "Sets the round type for the next game" )]
	public static void SetRoundTypeFromName(string roundName){
		foreach(var _rt in RoundTypes){
			if(_rt.command == roundName){
				NextRound = _rt;
			}
		}
	}

}
