using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

public enum MinesweeperTileType
{
	Money,
	Mine,
};
public partial class MinesweeperTile : Panel
{
	public int Xval;
	public int Yval;
	private Button button;
	public MinesweeperPodium podium;
	public bool revealed = false;
	public MinesweeperTileType? type = null;
	public MinesweeperTile()
	{
		StyleSheet.Load( "/entities/map/minesweeper/MinesweeperTile.scss" );
		AddClass( "sweep-tile-wrapper" );
		button = Add.Button( "crung-oo-lean", () =>
		{
			Log.Info( $"{Xval} {Yval} WAS PRESSED" );
			this.AddClass( "revealed" );
			this.revealed = true;
			// Sound.FromEntity( "captain morgan spiced h", podium );

		} );
		button.AddClass( "sweep-tile" );
	}
	public MinesweeperTile( MinesweeperTileType type, int x, int y, MinesweeperPodium podi ) : this()
	{
		podium = podi;
		Xval = x;
		Yval = y;
		button.Text = $"";
	}
	public override void Tick()
	{
		if ( Input.Pressed( InputButton.Jump ) || Input.Pressed( InputButton.Walk ) || Input.Pressed( InputButton.Duck ) )
		{
			// SetActive( false );
		}
	}

	public void Reset()
	{
		this.revealed = false;
		this.type = null;
	}

	// [ClientRpc]
	// public static void SetActive( bool status )
	// {
	// 	Active = status;
	// }
}