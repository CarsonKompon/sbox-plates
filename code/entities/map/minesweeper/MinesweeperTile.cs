using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

public partial class MinesweeperTile : Button
{
	public int Xval;
	public int Yval;
	public MinesweeperTile()
	{
		StyleSheet.Load( "/ui/minesweeper/MinesweeperTile.scss" );
		AddClass( "sweep-tile" );
	}
	public MinesweeperTile( int x, int y ) : this()
	{
		Xval = x;
		Yval = y;
		Add.Label( $"{Xval} {Yval}" );
	}
	public override void Tick()
	{
		if ( Input.Pressed( InputButton.Jump ) || Input.Pressed( InputButton.Walk ) || Input.Pressed( InputButton.Duck ) )
		{
			// SetActive( false );
		}
	}

	// [ClientRpc]
	// public static void SetActive( bool status )
	// {
	// 	Active = status;
	// }
}