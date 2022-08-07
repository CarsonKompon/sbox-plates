using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;

public partial class MinesweeperUI : WorldPanel
{
	Panel MinedSweep;
	Panel GameContainer;
	public bool Active;
	public MinesweeperTile[,] Tiles = new MinesweeperTile[5, 5];
	public MinesweeperPodium podium;
	public MinesweeperUI() { }
	public MinesweeperUI( float scale, MinesweeperPodium podi )
	{
		podium = podi;
		MinedSweep = Add.Panel( "minesweeper-card" );
		MinedSweep.Add.Label( "Plate Sweeper", "title" );
		GameContainer = MinedSweep.Add.Panel( "minesweeper-game-container" );


		StyleSheet.Load( "/entities/map/minesweeper/MinesweeperUI.scss" );
		AddClass( "minesweeper-ui" );
		//x
		SetupMines();


		var width = 200 * 20 * scale;
		var height = 200 * 20 * scale;
		PanelBounds = new Rect( -width * .5f, -height * .5f, width, height );

	}
	private void SetupMines()
	{

		Log.Info( $"Setting up ms grid for x: {Tiles.GetLength( 0 )}, y: {Tiles.GetLength( 1 )}" );
		// Mine Generation
		for ( int forX = 0; forX < Tiles.GetLength( 0 ); forX++ )
		{
			//y
			for ( int forY = 0; forY < Tiles.GetLength( 1 ); forY++ )
			{
				MinesweeperTile tile = new MinesweeperTile( Rand.Int( 0, 2 ) == 2 ? MinesweeperTileType.Mine : MinesweeperTileType.Money, forX, forY, podium );
				Tiles[forX, forY] = tile;
				GameContainer.AddChild( tile );
			}
		}


		// // Adjacent Mine Labelling
		//TODO: this can probably be better

		for ( int forX = 0; forX < Tiles.GetLength( 0 ); forX++ )
		{
			//y
			for ( int forY = 0; forY < Tiles.GetLength( 1 ); forY++ )
			{
				MinesweeperTile targetTile = Tiles[forX, forY];
				// 1 = up, 2 = down, 3 = left, 4 = right
				Vector2[] tilesToCheck = new Vector2[4];
				Array.Fill( tilesToCheck, new Vector2( -1, -1 ) );
				Log.Info( $"tile {forX}, {forY}" );
				if ( forY != 0 )
				{
					tilesToCheck[0] = new Vector2( forX, forY ) + Vector2.Down;
				}
				if ( forX != 0 )
				{
					tilesToCheck[1] = new Vector2( forX, forY ) + Vector2.Right;
				}
				if ( forY != 4 )
				{
					tilesToCheck[2] = new Vector2( forX, forY ) + Vector2.Up;
				}
				if ( forX != 4 )
				{
					tilesToCheck[3] = new Vector2( forX, forY ) + Vector2.Left;
				}

				foreach ( var coords in tilesToCheck )
				{

					Log.Info( $"looking for {coords}" );
				}
				foreach ( var coords in tilesToCheck )
				{

					int[] coordsAsInt = new int[2] { Convert.ToInt32( Math.Round( coords.x ) ), Convert.ToInt32( Math.Round( coords.y ) ) };
					if ( coordsAsInt[0] != -1 && coordsAsInt[1] != -1 )
					{
						Log.Info( $"looking for aaaaaa {coords}" );
						MinesweeperTile tiletocheck = Tiles[coordsAsInt[0], coordsAsInt[1]];
						if ( Tiles[coordsAsInt[0], coordsAsInt[1]].type == MinesweeperTileType.Mine )
						{
							targetTile.adjacentMines++;
						};

					}
				}
			}
		}

	}
	public override void Tick()
	{
		SetClass( "show-ui", true );
	}

	public void SetActive( bool inp )
	{
		Active = inp;
	}
}