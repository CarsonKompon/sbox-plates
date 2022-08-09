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

	public void handleTileClick( int x, int y )
	{
		MinesweeperTileType target = Tiles[y * dimensions + x];
		revealedTiles[y * dimensions + x] = true;
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


		// // Adjacent Mine Labelling
		//TODO: this can probably be better

		// for ( int forX = 0; forX < dimensions; forX++ )
		// {
		// 	//y
		// 	for ( int forY = 0; forY < dimensions; forY++ )
		// 	{
		// 		MinesweeperTileType targetTile = Tiles[forY * dimensions + forX];
		// 		// 1 = up, 2 = down, 3 = left, 4 = right
		// 		Vector2[] tilesToCheck = new Vector2[4];
		// 		Array.Fill( tilesToCheck, new Vector2( -1, -1 ) );
		// 		// Log.Info( $"tile {forX}, {forY}" );
		// 		if ( forY != 0 )
		// 		{
		// 			tilesToCheck[0] = new Vector2( forX, forY ) + Vector2.Down;
		// 		}
		// 		if ( forX != 0 )
		// 		{
		// 			tilesToCheck[1] = new Vector2( forX, forY ) + Vector2.Right;
		// 		}
		// 		if ( forY != 4 )
		// 		{
		// 			tilesToCheck[2] = new Vector2( forX, forY ) + Vector2.Up;
		// 		}
		// 		if ( forX != 4 )
		// 		{
		// 			tilesToCheck[3] = new Vector2( forX, forY ) + Vector2.Left;
		// 		}

		// 		foreach ( var coords in tilesToCheck )
		// 		{
		// 			int[] coordsAsInt = new int[2] { Convert.ToInt32( Math.Round( coords.x ) ), Convert.ToInt32( Math.Round( coords.y ) ) };
		// 			if ( coordsAsInt[0] != -1 && coordsAsInt[1] != -1 )
		// 			{
		// 				// Log.Info( $"looking for {coords}" );
		// 				MinesweeperTileType tiletocheck = Tiles[coordsAsInt[1] * dimensions + coordsAsInt[0]];
		// 				if ( Tiles[coordsAsInt[1] * dimensions + coordsAsInt[0]] == MinesweeperTileType.Mine )
		// 				{
		// 					adjacentMines[forY * dimensions + forX]++;
		// 				};

		// 			}
		// 		}
		// 	}
		// }

	}

	// [ClientRpc]
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

	// public void ResetUIAfterDelay( float timeToWait )
	// {

	// 	GameTask.RunInThreadAsync( async () =>
	// 	{
	// 		// Log.Info( $"{IsServer}" );

	// 		Log.Info( $"Resetting UI for {ui}" );
	// 		await GameTask.DelaySeconds( timeToWait );
	// 		Log.Info( "RESETTING UI" );
	// 		ui.Reset();
	// 	} );

	// }
}