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
				button.AddClass( "revealed" );
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
		label = new Label();
		button = Add.Button( "" );
		button.AddClass( "sweep-tile" );
	}
	public MinesweeperTile( MinesweeperUI inpUI, MinesweeperTileType type, int x, int y ) : this()
	{
		SweepUI = inpUI;
		Xval = x;
		Yval = y;
		// button.Text = $"{Xval},{Yval}";
		this.type = type;
		if ( this.type == MinesweeperTileType.Money )
		{
			button.AddClass( "money" );
			button.AddChild( label );
		}
		else
		{
			button.AddClass( "mine" );
		}
	}
	public override void Tick()
	{
		label.Text = $"{(adjacentMines != 0 && revealed ? adjacentMines : "")}";
		switch ( adjacentMines )
		{
			case 1:
				label.Style.FontColor = Color.Green;
				break;
			case 2:
				label.Style.FontColor = Color.Yellow.Darken( .2f );
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

	protected override void OnClick( MousePanelEvent e )
	{
		base.OnClick( e );

		if ( Local.Client.PlayerId == SweepUI.podium.gameState.activePlayerId )
		{
			RevealTile();
		}
	}

	public void RevealTile()
	{
		// Log.Info( $"{SweepUI}" );
		if ( !revealed )
		{
			Log.Info( $"{Xval} {Yval} WAS PRESSED, IT WAS A {type}" );
			SweepUI.podium.handleTileClick( Xval, Yval );
		}
		// revealed = true;

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