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
	public List<MinesweeperTile> Tiles { get; set; } = new();
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
		GameContainer.Style.FlexBasis = Length.Percent( (1 / podium.gameState.dimensions) * 100 );
	}
	public void FillBoard( IList<MinesweeperTileType> sweepers, IList<bool> revealedPanels )
	{
		if ( Tiles.Count < (podium.gameState.dimensions * podium.gameState.dimensions) )
		{
			Tiles = new List<MinesweeperTile>( new MinesweeperTile[podium.gameState.dimensions * podium.gameState.dimensions] );
		}
		// clense ui
		GameContainer.DeleteChildren();
		if ( podium.gameState.UIState == MinesweeperState.Idle )
		{
			return;
		}
		for ( int forX = 0; forX < podium.gameState.dimensions; forX++ )
		{
			//y
			for ( int forY = 0; forY < podium.gameState.dimensions; forY++ )
			{
				MinesweeperTile t = new MinesweeperTile( this, sweepers[forY * podium.gameState.dimensions + forX], forX, forY );
				t.revealed = revealedPanels[forY * podium.gameState.dimensions + forX];
				// Log.Info( $"Setting a {sweepers[forY * podium.gameState.dimensions + forX]}, revealed is {t.revealed}" );
				GameContainer.AddChild( t );

				Tiles[forY * podium.gameState.dimensions + forX] = t;
			}
		}

		for ( int forX = 0; forX < podium.gameState.dimensions; forX++ )
		{
			//y
			for ( int forY = 0; forY < podium.gameState.dimensions; forY++ )
			{
				MinesweeperTileType targetTile = Tiles[forY * podium.gameState.dimensions + forX].type;
				// 1 = up, 2 = down, 3 = left, 4 = right
				Vector2[] tilesToCheck = new Vector2[4];
				Array.Fill( tilesToCheck, new Vector2( -1, -1 ) );
				if ( forY != 0 )
				{
					tilesToCheck[0] = new Vector2( forX, forY ) + Vector2.Down;
				}
				if ( forX != 0 )
				{
					tilesToCheck[1] = new Vector2( forX, forY ) + Vector2.Right;
				}
				if ( forY != podium.gameState.dimensions - 1 )
				{
					tilesToCheck[2] = new Vector2( forX, forY ) + Vector2.Up;
				}
				if ( forX != podium.gameState.dimensions - 1 )
				{
					tilesToCheck[3] = new Vector2( forX, forY ) + Vector2.Left;
				}

				foreach ( var coords in tilesToCheck )
				{
					int[] coordsAsInt = new int[2] { Convert.ToInt32( Math.Round( coords.x ) ), Convert.ToInt32( Math.Round( coords.y ) ) };
					if ( coordsAsInt[0] != -1 && coordsAsInt[1] != -1 )
					{
						MinesweeperTileType tiletocheck = Tiles[coordsAsInt[1] * podium.gameState.dimensions + coordsAsInt[0]].type;
						if ( Tiles[coordsAsInt[1] * podium.gameState.dimensions + coordsAsInt[0]].type == MinesweeperTileType.Mine )
						{
							Tiles[forY * podium.gameState.dimensions + forX].adjacentMines++;
						};

					}
				}
			}
		}
	}
	public void YeetMines()
	{
		Tiles.Clear();
		GameContainer.DeleteChildren();
	}

	public void SetActive( bool inp )
	{
		Active = inp;
	}

}