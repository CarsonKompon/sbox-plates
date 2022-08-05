// using Sandbox;

// [Spawnable]
// [Library( "plates_ms_play_podium", Title = "Minesweeper Play Podium" )]
// public partial class HPodiumEntity : Prop, IUse
// {
// 	public override void Spawn()
// 	{
// 		base.Spawn();

// 		SetModel( "models/h_podium.vmdl" );
// 		Scale = 2.4f;
// 		SetupPhysicsFromModel( PhysicsMotionType.Dynamic, false );
// 	}

// 	public bool IsUsable( Entity user )
// 	{
// 		return true;
// 	}

// 	public bool OnUse( Entity user )
// 	{

// 		Sound.FromEntity( "captain morgan spiced h", this );
// 		MinesweeperUI.SetActive( To.Single( user ), true );

// 		return false;
// 	}

// }


