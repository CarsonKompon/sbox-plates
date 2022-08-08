using System;
using System.Threading.Tasks;
using Sandbox;
using Sandbox.Html;
using SandboxEditor;

[Spawnable]
[Library( "plates_ms_play_podium", Title = "Minesweeper Play Podium" ), HammerEntity]
[EditorModel( "models/ms_podium.vmdl" )]
public partial class MinesweeperPodium : Prop, IUse
{
	public MinesweeperUI screen { get; set; }
	[Net]
	public MinesweeperGameState gameState { get; set; }
	RealTimeSince randomTimer = 0f;

	public MinesweeperPodium()
	{
		if ( IsServer )
		{
			gameState = new MinesweeperGameState();
		}
	}
	public override void Spawn()
	{
		base.Spawn();

		SetModel( "models/ms_podium.vmdl" );
		SetupPhysicsFromModel( PhysicsMotionType.Static );

	}

	public override void ClientSpawn()
	{
		base.ClientSpawn();
		if ( screen == null )
		{
			screen = new MinesweeperUI( Scale, this );
			screen.Position = Position + (Rotation.Backward * (80 * Scale));
			screen.Position += (Rotation.Up * (90 * Scale));
			screen.Rotation = Rotation.LookAt( Rotation.Forward );

		}
	}
	[Event.Tick]
	public void Tick()
	{
		// Log.Info( $"{IsClientOnly}" );
		// if ( IsClient && screen == null )
		// {
		// 	screen = new MinesweeperUI( Scale, this );
		// 	screen.Position = Position + (Rotation.Backward * (80 * Scale));
		// 	screen.Position += (Rotation.Up * (90 * Scale));
		// 	screen.Rotation = Rotation.LookAt( Rotation.Forward );
		// 	gameState = new MinesweeperGameState();

		// }
	}

	public bool IsUsable( Entity user )
	{
		return true;
	}


	public bool OnUse( Entity user )
	{
		ClearBoard();
		gameState.Play();
		BuildUI();
		// Sound.FromEntity( "captain morgan spiced h", this );
		return false;
	}

	[ClientRpc]
	public void BuildUI()
	{
		foreach ( var item in gameState.Tiles )
		{
			Log.Info( $"{item}" );
		}
		screen.FillBoard( gameState.Tiles );
	}
	[ClientRpc]
	public void ClearBoard()
	{
		screen.YeetMines();
	}

	public void Fail()
	{
		gameState.Lose();
	}
	// [ClientRpc]
	// public void ResetUI()
	// {
	// 	screen?.Reset();
	// }
	// [ClientRpc]
	// public void FailClientUI()
	// {
	// 	screen?.Fail();
	// }


}


