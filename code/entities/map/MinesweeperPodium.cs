using Sandbox;
using SandboxEditor;

[Spawnable]
[Library( "plates_ms_play_podium", Title = "Minesweeper Play Podium" ), HammerEntity]
[EditorModel( "models/ms_podium.vmdl" )]
public partial class MinesweeperPodium : Prop, IUse
{
	public MinesweeperUI screen = null;
	public override void Spawn()
	{
		base.Spawn();

		SetModel( "models/ms_podium.vmdl" );
		SetupPhysicsFromModel( PhysicsMotionType.Static );
	}
	[Event.Tick]
	public void Tick()
	{
		if ( IsClient && screen == null )
		{
			screen = new MinesweeperUI( Scale, this );
			screen.Position = Position + (Rotation.Backward * (80 * Scale));
			screen.Position += (Rotation.Up * (90 * Scale));
			screen.Rotation = Rotation.LookAt( Rotation.Forward );
		}
	}

	public bool IsUsable( Entity user )
	{
		return false;
	}

	public bool OnUse( Entity user )
	{

		// Sound.FromEntity( "captain morgan spiced h", this );
		// MinesweeperUI.SetActive( To.Single( user ), true );

		return false;
	}

}


