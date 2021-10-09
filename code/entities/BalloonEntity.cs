using Sandbox;

[Library( "ent_balloon", Title = "Balloon", Spawnable = true )]
public partial class BalloonEntity : Prop
{
	public PhysicsJoint AttachJoint;
	public Particles AttachRope;

	private static float GravityScale => -0.3f; //-0.2f;

	public override void Spawn()
	{
		base.Spawn();

		SetModel( "models/citizen_props/balloonregular01.vmdl" );
		SetupPhysicsFromModel( PhysicsMotionType.Dynamic, false );
		PhysicsBody.GravityScale = GravityScale;
		RenderColor = Color.Random;
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();

		if ( AttachJoint.IsValid() )
		{
			AttachJoint.Remove();
		}

		if ( AttachRope != null )
		{
			AttachRope.Destroy( true );
		}
	}

	public override void OnKilled()
	{
		base.OnKilled();

		PlaySound( "balloon_pop_cute" );
	}

	// [Event.Physics.PostStep]
	// public void OnPostPhysicsStep()
	// {
	// 	if ( !this.IsValid() )
	// 		return;

	// 	var body = PhysicsBody;
	// 	if ( !body.IsValid() )
	// 		return;

	// 	body.GravityScale = GravityScale;
	// }
}
