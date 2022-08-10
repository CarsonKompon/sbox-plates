using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;

public enum MinesweeperState
{
	Idle,
	Playing,
	Failed,
}

public enum MinesweeperTileType
{
	Money,
	Mine,
};


public partial class MinesweeperGameState : Entity
{

	public static List<MinesweeperPodium> podiums = new();
	int localPodiumId { get; set; } = 0;

	[Net] public long activePlayerId { get; set; }
	[Net] public MinesweeperState UIState { get; set; }
	[Net] public List<MinesweeperTileType> Tiles { get; set; }
	[Net] public List<bool> revealedTiles { get; set; }
	[Net, Property] public int dimensions { get; set; } = 5;

	Func<int, MinesweeperPodium> currentPodium
	{
		get
		{
			return ( int nIdent ) => podiums.Find( pod => pod.NetworkIdent == nIdent );
		}
	}

	public MinesweeperGameState()
	{
		Tiles = new List<MinesweeperTileType>( new MinesweeperTileType[dimensions * dimensions] );
		revealedTiles = new List<bool>( new bool[dimensions * dimensions] );
		Log.Info( $"setting UI for {UIState}" );
	}
	// public MinesweeperGameState( MinesweeperPodium podi )
	// {
	// 	localPodiumId = podi.NetworkIdent;
	// }

	[ConCmd.Server]
	public static void handleTileClickGS( int nIdent, int x, int y )
	{
		MinesweeperPodium podi = podiums.Find( pod => pod.NetworkIdent == nIdent );
		MinesweeperTileType target = podi.gameState.Tiles[y * podi.gameState.dimensions + x];
		podi.gameState.revealedTiles[y * podi.gameState.dimensions + x] = true;
		if ( target == MinesweeperTileType.Mine )
		{
			Sound.FromEntity( "vine-boom", podi );
			podi.gameState.Lose( podi.NetworkIdent );
		}
		else
		{
			Sound.FromEntity( "yeah", podi );
		}
		podi.BuildUI();

	}
	private void SetupMines()
	{

		Tiles = new List<MinesweeperTileType>( new MinesweeperTileType[dimensions * dimensions] );
		Log.Info( $"Setting up ms grid for x: {dimensions}, y: {dimensions}" );
		// Mine Generation
		for ( int forX = 0; forX < dimensions; forX++ )
		{
			//y
			for ( int forY = 0; forY < dimensions; forY++ )
			{

				Tiles[forY * dimensions + forX] = Rand.Int( 1, 2 ) == 2 ? MinesweeperTileType.Mine : MinesweeperTileType.Money;
				// Log.Info( $"{Tiles[forX, forY]}" );
				revealedTiles[forY * dimensions + forX] = false;

			}
		}

	}

	public void Reset( int ident )
	{
		revealedTiles = new List<bool>();
		for ( int i = 0; i < dimensions * dimensions; i++ )
		{
			revealedTiles.Add( false );
		}
		currentPodium( ident ).ClearBoard();
	}
	public void Play()
	{
		UIState = MinesweeperState.Playing;
		SetupMines();
		Log.Info( $"{this}" );
	}
	async public void Lose( int ident )
	{
		MinesweeperPodium pod = currentPodium( ident );
		for ( int i = 0; i < revealedTiles.Count; i++ )
		{
			revealedTiles[i] = true;

		}
		UIState = MinesweeperState.Failed;
		await GameTask.DelaySeconds( 5 );
		UIState = MinesweeperState.Idle;
		Reset( ident );
		pod.ClearBoard();
		pod.BuildUI();

	}
}