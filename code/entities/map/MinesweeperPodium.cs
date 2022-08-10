using System;
using System.Threading.Tasks;
using Sandbox;
using System.Collections.Generic;
using SandboxEditor;

[Spawnable]
[Library( "plates_ms_play_podium", Title = "Minesweeper Play Podium" ), HammerEntity]
[EditorModel( "models/ms_podium.vmdl" )]
public partial class MinesweeperPodium : Prop, IUse
{
	public MinesweeperUI screen { get; set; }
	[Net] public MinesweeperGameState gameState { get; set; } = new();
	RealTimeSince randomTimer = 0f;

	public MinesweeperPodium()
	{
	}
	public override void Spawn()
	{
		base.Spawn();

		SetModel( "models/ms_podium.vmdl" );
		SetupPhysicsFromModel( PhysicsMotionType.Static );
		if ( IsServer )
		{
			gameState = new MinesweeperGameState();
			MinesweeperGameState.podiums.Add( this );
		}
		BuildUI();


	}

	public override void ClientSpawn()
	{
		base.ClientSpawn();
		// if ( IsClient )
		// {
		// 	gameState = gameState;
		// }

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
	{ }

	public bool IsUsable( Entity user )
	{
		return true;
	}


	public bool OnUse( Entity user )
	{
		gameState.Reset();
		ClearBoard();
		gameState.Play();
		BuildUI();
		// Sound.FromEntity( "captain morgan spiced h", this );
		return false;
	}

	[ClientRpc]
	public void BuildUI()
	{
		Log.Info( $"{gameState}" );
		screen.FillBoard( gameState.Tiles, gameState.revealedTiles );
	}
	[ClientRpc]
	public void ClearBoard()
	{
		screen.YeetMines();
	}

	[ClientRpc]
	public void RevealTile( int index )
	{
		screen.Tiles[index].RevealTile();

	}
	public void handleTileClick( int x, int y )
	{

		MinesweeperTileType target = gameState.Tiles[y * gameState.dimensions + x];
		gameState.revealedTiles[y * gameState.dimensions + x] = true;
		MinesweeperGameState.handleTileClickGS( this.NetworkIdent, x, y );
		BuildUI();
		// Log.Info( $"{target}" );
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


