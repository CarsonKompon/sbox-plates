using Sandbox;
using SandboxEditor;

[Library("plates_kill_trigger", Description = "A trigger that instantly kills players.")]
[HammerEntity, Solid]
public class KillWallEntity : ModelEntity
{
    public override void Spawn()
    {
        base.Spawn();

        SetupPhysicsFromModel( PhysicsMotionType.Static );
        Tags.Add("trigger");
        EnableSolidCollisions = false;
        EnableTouch = true;

        Transmit = TransmitType.Never;

		Name = "The Void";
    }

	public override void StartTouch( Entity other )
	{
		base.StartTouch( other );

		if ( other is Entity ent ) {
			var dmg = DamageInfo.Generic( 1000 );
			dmg.Attacker = this;
			ent.TakeDamage( dmg );
		}
	}

	/*
		public override void StartTouch( Entity other )
		{
			base.StartTouch( other );

			if(other.IsValid()) other.Delete();
		}
	*/
}
