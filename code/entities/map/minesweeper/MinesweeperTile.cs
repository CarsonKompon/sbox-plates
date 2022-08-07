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
	public MinesweeperTileType type;
	public Player activePlayer = null;

	private Label label;

	public int adjacentMines = 0;
	public MinesweeperTile()
	{
		StyleSheet.Load( "/entities/map/minesweeper/MinesweeperTile.scss" );
		AddClass( "sweep-tile-wrapper" );
		label = new Label();
		button = Add.Button( "crung-oo-lean", () =>
		{
			Log.Info( $"{Xval} {Yval} WAS PRESSED, IT WAS A {type}" );
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
		this.type = type;
		if ( this.type == MinesweeperTileType.Money )
		{
			button.AddChild( label );
		}
	}
	public override void Tick()
	{
		label.Text = $"{adjacentMines}";
		if ( Input.Pressed( InputButton.Jump ) || Input.Pressed( InputButton.Walk ) || Input.Pressed( InputButton.Duck ) )
		{
			// SetActive( false );
		}
	}

	public void Reset()
	{
		this.revealed = false;
		adjacentMines = 0;
		//TODO: Make this not random
		this.type = Rand.Int( 0, 2 ) == 2 ? MinesweeperTileType.Mine : MinesweeperTileType.Money;
	}

	// [ClientRpc]
	// public static void SetActive( bool status )
	// {
	// 	Active = status;
	// }
}