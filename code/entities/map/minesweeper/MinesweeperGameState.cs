using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;

public enum MinesweeperState
{
	Playing,
	Idle,
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

	[Net] public MinesweeperState UIState { get; set; }
	[Net] public List<MinesweeperTileType> Tiles { get; set; }
	[Net] public List<bool> revealedTiles { get; set; }
	[Net] public int dimensions { get; set; } = 5;

	public MinesweeperGameState()
	{
		Tiles = new List<MinesweeperTileType>( new MinesweeperTileType[dimensions * dimensions] );
		revealedTiles = new List<bool>( new bool[dimensions * dimensions] );
		Log.Info( $"setting UI for {UIState}" );
	}
	// public MinesweeperGameState( MinesweeperPodium podi ) : this()
	// {
	// 	podium = podi;
	// 	Log.Info( $"{podium} {IsClient} {IsServer}" );
	// }

	[ConCmd.Server]
	public static void handleTileClickGS( int nIdent, int x, int y )
	{
		MinesweeperPodium podi = podiums.Find( pod => pod.NetworkIdent == nIdent );
		MinesweeperTileType target = podi.gameState.Tiles[y * podi.gameState.dimensions + x];
		podi.gameState.revealedTiles[y * podi.gameState.dimensions + x] = true;
		podi.BuildUI();
		Log.Info( $"{target}" );

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
		UIState = MinesweeperState.Playing;

	}

	public void Reset()
	{
		revealedTiles = new List<bool>();
		for ( int i = 0; i < dimensions * dimensions; i++ )
		{
			revealedTiles.Add( false );
		}
	}
	public void Play()
	{
		SetupMines();
		Log.Info( $"{this}" );
	}
	public void Lose()
	{
		// foreach ( MinesweeperTileType tile in Tiles )
		// {
		// 	tile.revealed = true;
		// }
	}
}