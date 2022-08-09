using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

public partial class MinesweeperTile : Panel
{
	public int Xval;
	public int Yval;
	private Button button;
	public MinesweeperUI SweepUI;
	private bool _revealed = false;
	public bool revealed
	{
		get
		{
			return _revealed;
		}
		set
		{
			_revealed = value;
			if ( value )
			{
				this.AddClass( "revealed" );
			}
		}
	}
	public MinesweeperTileType type;
	public Player activePlayer = null;

	private Label label;

	public int adjacentMines = 0;
	public MinesweeperTile()
	{
		StyleSheet.Load( "/entities/map/minesweeper/MinesweeperTile.scss" );
		AddClass( "sweep-tile-wrapper" );
		AddEventListener( "onclick", RevealTile );
		label = new Label();
		button = Add.Button( "crung-oo-lean" );
		button.AddClass( "sweep-tile" );
	}
	public MinesweeperTile( MinesweeperUI inpUI, MinesweeperTileType type, int x, int y ) : this()
	{
		SweepUI = inpUI;
		Xval = x;
		Yval = y;
		button.Text = $"{Xval},{Yval}";
		this.type = type;
		if ( this.type == MinesweeperTileType.Money )
		{
			AddClass( "money" );
			button.AddChild( label );
		}
		else
		{
			AddClass( "mine" );
		}
	}
	public override void Tick()
	{
		label.Text = $"{(adjacentMines != 0 ? adjacentMines : "")}";
		switch ( adjacentMines )
		{
			case 1:
				label.Style.FontColor = Color.Green;
				break;
			case 2:
				label.Style.FontColor = Color.Yellow;
				break;
			case 3:
				label.Style.FontColor = Color.Orange;
				break;
			case 4:
				label.Style.FontColor = Color.Red;
				break;
			default:
				label.Style.FontColor = Color.White;
				break;
		}
		if ( Input.Pressed( InputButton.Jump ) || Input.Pressed( InputButton.Walk ) || Input.Pressed( InputButton.Duck ) )
		{
			// SetActive( false );
		}
	}

	public void RevealTile()
	{
		Log.Info( $"{Xval} {Yval} WAS PRESSED, IT WAS A {type}" );
		Log.Info( $"{SweepUI}" );

		SweepUI.podium.gameState.handleTileClick( Xval, Yval );
		revealed = true;

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