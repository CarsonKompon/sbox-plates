using Sandbox;

[Library("kill_wall")]
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
    }

/*
    public override void StartTouch( Entity other )
    {
        base.StartTouch( other );

        if(other.IsValid()) other.Delete();
    }
*/
}
