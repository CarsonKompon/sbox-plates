using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;



public partial class MinesweeperUI : WorldPanel
{
	Panel MinedSweep;
	public Panel GameContainer;
	public bool Active;
	private bool populated = false;
	public List<MinesweeperTile> Tiles { get; set; }
	public MinesweeperPodium podium;



	public MinesweeperUI()
	{
	}
	public MinesweeperUI( float scale, MinesweeperPodium podi ) : this()
	{
		podium = podi;
		MinedSweep = Add.Panel( "minesweeper-card" );
		MinedSweep.Add.Label( "Plate Sweeper", "title" );
		GameContainer = MinedSweep.Add.Panel( "minesweeper-game-container" );


		StyleSheet.Load( "/entities/map/minesweeper/MinesweeperUI.scss" );
		AddClass( "minesweeper-ui" );


		var width = 200 * 20 * scale;
		var height = 200 * 20 * scale;
		PanelBounds = new Rect( -width * .5f, -height * .5f, width, height );
	}

	// }
	public override void Tick()
	{
		SetClass( "show-ui", true );
		// if ( podium.gameState.UIState == MinesweeperState.Playing && !populated )
		// {
		// 	FillBoard();
		// }
	}
	public void FillBoard( IList<MinesweeperTileType> sweepers )
	{
		Tiles = new List<MinesweeperTile>( podium.gameState.dimensions ^ 2 );
		for ( int forX = 0; forX < podium.gameState.dimensions; forX++ )
		{
			//y
			for ( int forY = 0; forY < podium.gameState.dimensions; forY++ )
			{
				Log.Info( $"Setting a {sweepers[forY * podium.gameState.dimensions + forX]}" );
				GameContainer.AddChild( new MinesweeperTile( this, sweepers[forY * podium.gameState.dimensions + forX], forX, forY ) );
			}
		}
		populated = true;
	}
	public void YeetMines()
	{
		GameContainer.DeleteChildren();
	}

	public void SetActive( bool inp )
	{
		Active = inp;
	}
	// private void SetupMines()
	// {

	// 	Log.Info( $"Setting up ms grid for x: {Tiles.GetLength( 0 )}, y: {Tiles.GetLength( 1 )}" );
	// 	// Mine Generation
	// 	for ( int forX = 0; forX < Tiles.GetLength( 0 ); forX++ )
	// 	{
	// 		//y
	// 		for ( int forY = 0; forY < Tiles.GetLength( 1 ); forY++ )
	// 		{
	// 			MinesweeperTile tile = new MinesweeperTile( Rand.Int( 0, 2 ) == 2 ? MinesweeperTileType.Mine : MinesweeperTileType.Money, forX, forY, podium );
	// 			Tiles[forX, forY] = tile;
	// 			GameContainer.AddChild( tile );
	// 		}
	// 	}


	// 	// // Adjacent Mine Labelling
	// 	//TODO: this can probably be better

	// 	for ( int forX = 0; forX < Tiles.GetLength( 0 ); forX++ )
	// 	{
	// 		//y
	// 		for ( int forY = 0; forY < Tiles.GetLength( 1 ); forY++ )
	// 		{
	// 			MinesweeperTile targetTile = Tiles[forX, forY];
	// 			// 1 = up, 2 = down, 3 = left, 4 = right
	// 			Vector2[] tilesToCheck = new Vector2[4];
	// 			Array.Fill( tilesToCheck, new Vector2( -1, -1 ) );
	// 			// Log.Info( $"tile {forX}, {forY}" );
	// 			if ( forY != 0 )
	// 			{
	// 				tilesToCheck[0] = new Vector2( forX, forY ) + Vector2.Down;
	// 			}
	// 			if ( forX != 0 )
	// 			{
	// 				tilesToCheck[1] = new Vector2( forX, forY ) + Vector2.Right;
	// 			}
	// 			if ( forY != 4 )
	// 			{
	// 				tilesToCheck[2] = new Vector2( forX, forY ) + Vector2.Up;
	// 			}
	// 			if ( forX != 4 )
	// 			{
	// 				tilesToCheck[3] = new Vector2( forX, forY ) + Vector2.Left;
	// 			}

	// 			foreach ( var coords in tilesToCheck )
	// 			{
	// 				int[] coordsAsInt = new int[2] { Convert.ToInt32( Math.Round( coords.x ) ), Convert.ToInt32( Math.Round( coords.y ) ) };
	// 				if ( coordsAsInt[0] != -1 && coordsAsInt[1] != -1 )
	// 				{
	// 					// Log.Info( $"looking for {coords}" );
	// 					MinesweeperTile tiletocheck = Tiles[coordsAsInt[0], coordsAsInt[1]];
	// 					if ( Tiles[coordsAsInt[0], coordsAsInt[1]].type == MinesweeperTileType.Mine )
	// 					{
	// 						targetTile.adjacentMines++;
	// 					};

	// 				}
	// 			}
	// 		}
	// 	}

	// public void Fail()
	// {
	// 	Log.Warning( "IM SCREAMING" );

	// 	foreach ( MinesweeperTile tile in Tiles )
	// 	{
	// 		tile.revealed = true;
	// 	}
	// }
}