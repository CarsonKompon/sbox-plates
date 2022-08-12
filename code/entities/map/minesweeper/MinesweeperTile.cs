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
		Style.Width = Length.Percent( (1f / SweepUI.podium.gameState.dimensions) * 100f );
		Style.Height = Length.Percent( (1f / SweepUI.podium.gameState.dimensions) * 100f );
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
		if ( !revealed )
		{
			SweepUI.podium.handleTileClick( Xval, Yval );
		}

	}


}