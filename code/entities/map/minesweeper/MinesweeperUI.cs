using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System.Collections.Generic;

public partial class MinesweeperUI : Panel
{
	public static MinesweeperUI Instance;
	public Panel screaming;
	Panel MinedSweep;
	Panel GameContainer;
	public static bool Active;
	public List<List<MinesweeperTile>> Tiles;
	public MinesweeperUI()
	{
		MinedSweep = Add.Panel( "minesweeper-card" );
		MinedSweep.Add.Label( "Plate Sweeper", "title" );
		GameContainer = MinedSweep.Add.Panel( "minesweeper-game-container" );


		StyleSheet.Load( "/ui/minesweeper/MinesweeperUI.scss" );
		AddClass( "minesweeper-ui" );
		//x
		for ( int forX = 0; forX < 10; forX++ )
		{
			//y
			for ( int forY = 0; forY < 10; forY++ )
			{
				Log.Info( $"{forX} {forY}" );
				GameContainer.AddChild( new MinesweeperTile( forX, forY ) );
				// Tiles[forX][forY] = new MinesweeperTile();
			}
		}
	}
	public override void Tick()
	{
		SetClass( "show-ui", Active );
		if ( Input.Pressed( InputButton.Jump ) || Input.Pressed( InputButton.Walk ) || Input.Pressed( InputButton.Duck ) )
		{
			SetActive( false );
		}
	}

	[ClientRpc]
	public static void SetActive( bool status )
	{
		Active = status;
	}
}