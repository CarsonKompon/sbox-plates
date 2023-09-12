using System;
using System.Threading.Tasks;
using Sandbox;
using System.Collections.Generic;
using Editor;

namespace Plates;

[Library( "plates_casino_minesweep", Title = "Minesweeper Play Podium" )]
[HammerEntity, EditorModel( "models/casino/podium_minesweep.vmdl" )]
public partial class MinesweeperPodium : Prop, IUse
{
	[Property(Title = "Board Dimensions")] public int dimensions {get;set;} = 5;
	public MinesweeperUI screen;
	[Net] public MinesweeperGameState gameState { get; set; } = new();
	RealTimeSince randomTimer = 0f;

	public MinesweeperPodium()
	{
	}
	public override void Spawn()
	{
		base.Spawn();

		SetModel( "models/casino/podium_minesweep.vmdl" );
		SetupPhysicsFromModel( PhysicsMotionType.Static );
		if ( Game.IsServer )
		{
			gameState = new MinesweeperGameState(dimensions);
			MinesweeperGameState.podiums.Add( this );
		}
		BuildUI( gameState );


	}

	public override void ClientSpawn()
	{
		base.ClientSpawn();

		if ( screen == null )
		{
			screen = new MinesweeperUI( Scale, this );
			screen.Position = Position + (Rotation.Backward * (80 * Scale));
			screen.Position += (Rotation.Up * (90 * Scale));
			screen.Rotation = Rotation;
		}
	}
	[Event.Tick]
	public void Tick()
	{ }

	public bool IsUsable( Entity user )
	{
		return gameState.UIState == MinesweeperState.Idle;
	}


	public bool OnUse( Entity user )
	{
		gameState.Reset( NetworkIdent );
		ClearBoard();
		gameState.activeSteamId = user.Client.SteamId;
		gameState.Play();
		BuildUI( gameState );
		// Sound.FromEntity( "captain morgan spiced h", this );
		return false;
	}

	[ClientRpc, Event.Hotload]
	public void BuildUI( MinesweeperGameState state )
	{
		screen.FillBoard( state.Tiles, state.revealedTiles );
	}
	[ClientRpc]
	public void ClearBoard()
	{
		screen.YeetMines();
	}

	[ClientRpc]
	public void RevealTile( int index )
	{
		screen.Tiles[index].RevealTile();

	}
	public void handleTileClick( int x, int y )
	{

		MinesweeperTileType target = gameState.Tiles[y * gameState.dimensions + x];
		gameState.revealedTiles[y * gameState.dimensions + x] = true;
		MinesweeperGameState.handleTileClickGS( this.NetworkIdent, x, y );
		BuildUI( gameState );
		// Log.Info( $"{target}" );
	}

	public void Fail()
	{
		gameState.Lose( this.NetworkIdent );
	}

}


