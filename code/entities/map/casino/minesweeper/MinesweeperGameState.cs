using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;

public enum MinesweeperState
{
	Idle,
	Playing,
	Failed,
}

public enum MinesweeperTileType
{
	Money,
	Mine,
};


public partial class MinesweeperGameState : Entity
{

	public static List<MinesweeperPodium> podiums = new();
	int localPodiumId { get; set; } = 0;

	[Net] public long activePlayerId { get; set; }
	[Net] public MinesweeperState UIState { get; set; }
	[Net] public List<MinesweeperTileType> Tiles { get; set; }
	[Net] public List<bool> revealedTiles { get; set; }

	public float rewardMultiplier
	{
		get
		{
			double revealedMonies = 0;
			foreach ( bool revealed in revealedTiles )
			{
				if ( revealed ) revealedMonies++;

			}
			return MathC.Map( (float)revealedMonies, 0, totalMoneyTiles, 0.5f, 2.5f + dimensions - 3 );
		}
	}
	public int totalMoneyTiles
	{
		get
		{
			int temp = 0;
			foreach ( MinesweeperTileType tile in Tiles )
			{
				if ( tile == MinesweeperTileType.Money )
				{
					temp++;
				}
			}
			return temp;
		}
	}

	[Net] public int dimensions { get; set; } = 5;
	[Net] public int wager { get; set; } = 0;

	Func<int, MinesweeperPodium> currentPodium
	{
		get
		{
			return ( int nIdent ) => podiums.Find( pod => pod.NetworkIdent == nIdent );
		}
	}

	public MinesweeperGameState() { }
	public MinesweeperGameState( int dim = 5 )
	{
		dimensions = dim;
		Tiles = new List<MinesweeperTileType>( new MinesweeperTileType[dimensions * dimensions] );
		revealedTiles = new List<bool>( new bool[dimensions * dimensions] );
	}
	// public MinesweeperGameState( MinesweeperPodium podi )
	// {
	// 	localPodiumId = podi.NetworkIdent;
	// }

	[ConCmd.Server]
	public static void handleTileClickGS( int nIdent, int x, int y )
	{
		MinesweeperPodium podi = podiums.Find( pod => pod.NetworkIdent == nIdent );
		MinesweeperTileType target = podi.gameState.Tiles[y * podi.gameState.dimensions + x];
		podi.gameState.revealedTiles[y * podi.gameState.dimensions + x] = true;
		if ( target == MinesweeperTileType.Mine )
		{
			Sound.FromEntity( "vine-boom", podi );
			podi.gameState.Lose( podi.NetworkIdent );
		}
		else
		{
			Sound.FromEntity( "yeah", podi );
			podi.RevealTile( y * podi.gameState.dimensions + x );
			// Log.Info( $"{podi.gameState.rewardMultiplier}" );
		}

	}
	private void SetupMines()
	{

		Tiles = new List<MinesweeperTileType>( new MinesweeperTileType[dimensions * dimensions] );
		// Mine Generation

		switch ( Rand.Int( 0, 4 ) )
		{
			//random
			default:
				for ( int forX = 0; forX < dimensions; forX++ )
				{
					//y
					for ( int forY = 0; forY < dimensions; forY++ )
					{

						Tiles[forY * dimensions + forX] = Rand.Int( 0, 10 ) > (5 + Rand.Int( -1, 5 )) ? MinesweeperTileType.Mine : MinesweeperTileType.Money;
						// Log.Info( $"{Tiles[forY * dimensions + forX]}" );
						revealedTiles[forY * dimensions + forX] = false;

					}
				}
				break;

		}

	}

	public void Reset( int ident )
	{
		revealedTiles = new List<bool>();
		for ( int i = 0; i < dimensions * dimensions; i++ )
		{
			revealedTiles.Add( false );
		}
		currentPodium( ident ).ClearBoard();
		wager = 0;
	}
	public void Play( long userId, int bet )
	{
		activePlayerId = userId;

		UIState = MinesweeperState.Playing;
		SetupMines();
		wager = bet;
	}

	public void UpdateTile( int tileIndex )
	{
		MinesweeperPodium podi = podiums.Find( pod => pod.gameState.NetworkIdent == NetworkIdent );
		podi.RevealTile( tileIndex );
	}
	async public void Lose( int ident )
	{
		MinesweeperPodium pod = currentPodium( ident );
		for ( int i = 0; i < revealedTiles.Count; i++ )
		{
			revealedTiles[i] = true;

		}
		pod.BuildUI( this );
		UIState = MinesweeperState.Failed;

		await GameTask.DelayRealtimeSeconds( 5 );

		UIState = MinesweeperState.Idle;
		Reset( ident );
		pod.ClearBoard();
		pod.BuildUI( this );

	}
}