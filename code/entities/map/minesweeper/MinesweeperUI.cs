using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System.Collections.Generic;

public partial class MinesweeperUI : WorldPanel
{
	Panel MinedSweep;
	Panel GameContainer;
	public bool Active;
	public List<List<MinesweeperTile>> Tiles;
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
		for ( int forX = 0; forX < 5; forX++ )
		{
			//y
			for ( int forY = 0; forY < 5; forY++ )
			{
				Log.Info( $"{forX} {forY}" );
				GameContainer.AddChild( new MinesweeperTile( forX, forY, podium ) );
				// Tiles[forX][forY] = new MinesweeperTile();
			}
		}


		var width = 200 * 20 * scale;
		var height = 200 * 20 * scale;
		PanelBounds = new Rect( -width * .5f, -height * .5f, width, height );

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