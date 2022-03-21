using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;

public class AvatarHud : Panel
{
	
	public ScenePanel scene { get; set; }
	public SceneWorld world { get; set; }
	public SceneModel avatar;

	public AvatarHud()
	{
		Style.FlexWrap = Wrap.Wrap;
		Style.JustifyContent = Justify.Center; 
		Style.AlignItems = Align.Center;
		Style.AlignContent = Align.Center;

		Angles camAngle = new( 25, 180, 0 );
		Vector3 camPos = Vector3.Up * 40 + camAngle.Direction * -100;

		world = new SceneWorld();
		scene = Add.ScenePanel( world, camPos, Rotation.From( camAngle ), 70 );
		scene.Style.Width = 512;
		scene.Style.Height = 512;

		avatar = new SceneModel( world, Model.Load( "models/citizen/citizen.vmdl" ), Transform.Zero );

		new SceneLight( world, Vector3.Up * 10.0f + Vector3.Forward * 100.0f + Vector3.Right * 100.0f, 200f, Color.White * 150.0f );
	}

	Vector3 lookPos;
	Vector3 headPos;
	Vector3 aimPos;

	public override void Tick()
	{
		base.Tick();

		if ( avatar == null )
			return;

		// Get mouse position
		var mousePosition = Mouse.Position;

		// subtract what we think is about the player's eye position
		if(Mouse.Active){
		mousePosition.x -= scene.Box.Rect.width * 0.45f;
		mousePosition.y -= scene.Box.Rect.height * 1.85f;
		}else{
			mousePosition.x = 0;
			mousePosition.y = 0;
			//Log.Info(mousePosition.x + ", " + mousePosition.y);
		}
		mousePosition /= scene.ScaleToScreen;

		// convert it to an imaginary world position
		var worldpos = new Vector3( 200, mousePosition.x, -mousePosition.y );

		// convert that to local space for the model
		lookPos = avatar.Transform.PointToLocal( worldpos );
		headPos = Vector3.Lerp( headPos, avatar.Transform.PointToLocal( worldpos ), Time.Delta * 20.0f );
		aimPos = Vector3.Lerp( aimPos, avatar.Transform.PointToLocal( worldpos ), Time.Delta * 5.0f );


		avatar.SetAnimParameter( "b_grounded", true );
		avatar.SetAnimParameter( "aim_eyes", lookPos );
		avatar.SetAnimParameter( "aim_head", headPos );
		avatar.SetAnimParameter( "aim_body", aimPos );
		avatar.SetAnimParameter( "aim_body_weight", 1.0f );

		avatar.Update( Time.Now );

		Angles angles = new( 15, 180, 0 );
		Vector3 pos = Vector3.Up * 45 + angles.Direction * -80;

		scene.CameraPosition = pos;
		scene.CameraRotation = angles.ToRotation();
	}

}
