using Sandbox;

[Library("plates_kill_trigger", Description = "A trigger that instantly kills players.")]
[Hammer.Solid]
public class KillWallEntity : ModelEntity
{
    public override void Spawn()
    {
        base.Spawn();

        SetupPhysicsFromModel( PhysicsMotionType.Static );
        CollisionGroup = CollisionGroup.Trigger;
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
