using Sandbox;
using SandboxEditor;

[Library("plates_conveyor_belt", Description = "A conveyor belt that pushes entities on top")]
[HammerEntity, Solid]
public class ConveyorBelt : ModelEntity
{
	[Property(Title = "Conveyor Direction")]
	public Vector3 Direction {get;set;} = Vector3.Zero;

	[Property(Title = "Conveyor Speed")]
	public float Speed {get;set;} = 400f;

    public override void Spawn()
    {
        base.Spawn();

        SetupPhysicsFromModel( PhysicsMotionType.Static );
        EnableSolidCollisions = true;
        EnableTouch = true;

		Name = "Converyor Belt";
    }

	public override void Touch( Entity other )
	{
		base.Touch( other );

		Log.Info($"{other} is standing on me");
	}

	/*
		public override void StartTouch( Entity other )
		{
			base.StartTouch( other );

			if(other.IsValid()) other.Delete();
		}
	*/
}
