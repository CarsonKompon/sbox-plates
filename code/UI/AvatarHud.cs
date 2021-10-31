using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System.Collections.Generic;

public class AvatarHud : Panel
{
	
	public ScenePanel scene { get; set; }
	public SceneWorld world { get; set; }
	public AnimSceneObject avatar;
	private SpotLight LightWarm;
	//private SpotLight LightBlue;

	public AvatarHud()
	{
		world = new SceneWorld();
		Style.FlexWrap = Wrap.Wrap;
		Style.JustifyContent = Justify.Center; 
		Style.AlignItems = Align.Center;
		Style.AlignContent = Align.Center;

		using ( SceneWorld.SetCurrent( world ) )
		{
			//SceneObject.CreateModel( "models/citizen_props/roadcone01.vmdl", Transform.Zero );
			//SceneObject.CreateModel( "models/room.vmdl", Transform.Zero );

			avatar = new AnimSceneObject(Model.Load("models/citizen/citizen.vmdl"), Transform.Zero);

			// foreach(var c in Local.Pawn.Children){
			// 	var clothingObject = new AnimSceneObject((c as ModelEntity).GetModel(), Transform.Zero);
			// 	avatar.AddChild("h", clothingObject);
			// }

			LightWarm = new SpotLight( Vector3.Up * 10.0f + Vector3.Forward * 100.0f + Vector3.Right * 100.0f, Color.White * 15000.0f );
			LightWarm.Rotation = Rotation.LookAt( -LightWarm.Position );
			LightWarm.SpotCone = new SpotLightCone { Inner = 90, Outer = 90 };

			//LightBlue = new SpotLight( Vector3.Up * 10.0f + Vector3.Forward * 100.0f + Vector3.Right * 100.0f, Color.White * 15000.0f );
			//LightBlue.Ang = Rotation.LookAt( -LightBlue.Pos ).Angles();
			//LightBlue.SpotCone = new SpotLightCone { Inner = 90, Outer = 90 };

			Angles angles = new( 25, 180, 0 );
			Vector3 pos = Vector3.Up * 40 + angles.Direction * -100;

			scene = Add.ScenePanel(world, pos, angles.ToRotation(), 28);
			scene.AmbientColor = Color.Gray * 0.2f;
			scene.Style.Width = 512;
			scene.Style.Height = 512;
		}
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


		avatar.SetAnimBool( "b_grounded", true );
		avatar.SetAnimVector( "aim_eyes", lookPos );
		avatar.SetAnimVector( "aim_head", headPos );
		avatar.SetAnimVector( "aim_body", aimPos );
		avatar.SetAnimFloat( "aim_body_weight", 1.0f );

		avatar.Update( Time.Now );

		Angles angles = new( 15, 180, 0 );
		Vector3 pos = Vector3.Up * 45 + angles.Direction * -80;

		scene.CameraPosition = pos;
		scene.CameraRotation = angles.ToRotation();

		LightWarm.Position = Vector3.Up * 100.0f + Vector3.Forward * 200.0f + Vector3.Right * -100;
		LightWarm.LightColor = new Color( 1.0f, 0.95f, 0.8f ) * 60.0f;
		LightWarm.Rotation = Rotation.LookAt( -LightWarm.Position );
		LightWarm.SpotCone = new SpotLightCone { Inner = 90, Outer = 90 };

		//LightBlue.Pos = Vector3.Up * 100.0f + Vector3.Forward * -100.0f + Vector3.Right * 100;
		//LightBlue.LightColor = new Color( 1.0f, 0.95f, 0.8f ) * 60.0f;
		//LightBlue.Ang = Rotation.LookAt( -LightBlue.Pos ).Angles();
		//LightBlue.SpotCone = new SpotLightCone { Inner = 90, Outer = 90 };
	}

}