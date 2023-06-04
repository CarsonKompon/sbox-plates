using System.Diagnostics;
using System.ComponentModel;
using System;
using Sandbox;
using Sandbox.UI;
using Sandbox.Utility;
using Sandbox.UI.Construct;
using System.Collections.Generic;

public class AvatarHud : ScenePanel
{
    private SceneModel AvatarModel;
    private List<SceneModel> ClothingObjects = new();

    private SceneSpotLight LightWarm;
	private SceneSpotLight LightBlue;

    public void Rebuild()
    {
        if(Game.LocalClient.Pawn == null)
        {
            return;
        }

        // Cleanup
        World?.Delete();
        ClothingObjects.Clear();

        // Create
        World = new SceneWorld();
        AvatarModel = new SceneModel(World, "models/citizen/citizen.vmdl", Transform.Zero);
        DressAvatar();

        LightWarm = new SceneSpotLight(World);
        LightBlue = new SceneSpotLight(World);
        new SceneCubemap(World, Texture.Load("textures/cubemaps/default.vtex" ), BBox.FromPositionAndSize( Vector3.Zero, 1000 ) );

        Angles angles = new( 25, 180, 0 );
		Vector3 pos = Vector3.Up * 40 + angles.Forward * -20;

		Camera.Position = pos;
		Camera.Rotation = Rotation.From( angles );
		//Camera.AmbientLightColor = Color.Gray * 0.1f;
		Camera.Name = "Home Avatar";
    }

    public override void Tick()
    {
        base.Tick();

        if(AvatarModel == null)
        {
            Rebuild();
        }
        else
        {
            TickAvatar();
        }
    }

    void DressAvatar()
    {
        if(Game.LocalClient.Pawn == null)
        {
            return;
        }

        AvatarModel.SetMaterialGroup("Skin01");
        foreach(var model in ClothingObjects)
        {
            model?.Delete();
        }

        ClothingContainer clothing = new();
        clothing.Deserialize((Game.LocalPawn as PlatesPlayer)?.ClothingString ?? "");
        if(clothing != null)
        {
            Log.Info("CLOTHING:");
            Log.Info(clothing.Serialize());
            ClothingObjects = clothing.DressSceneObject(AvatarModel);
        }
    }

    Vector3 lookPos;
    Vector3 headPos;
    Vector3 aimPos;

    void TickAvatar()
    {
        var interestPos = AvatarModel.Position + AvatarModel.Rotation.Forward * 10;

		interestPos += AvatarModel.Rotation.Right * ( Noise.Fbm( 6, RealTime.Now * 3.0f, 0.2f ) - 0.5f) * 1000.0f;
		interestPos += AvatarModel.Rotation.Up * ( Noise.Fbm( 2, RealTime.Now * 10.0f, 0.2f ) - 0.5f) * 100.0f;
	   
		interestPos = AvatarModel.Transform.PointToLocal( interestPos );

		lookPos = interestPos;
		headPos = Vector3.Lerp( headPos, interestPos, Time.Delta * 12.0f );
		aimPos = Vector3.Lerp( aimPos, interestPos, Time.Delta * 60.0f );

        AvatarModel.Position = Vector3.Zero;
        AvatarModel.Rotation = Rotation.From(0, 0, 0);
        AvatarModel.SetAnimParameter("idle_states", 0);
        AvatarModel.SetAnimParameter("b_grounded", true);
        AvatarModel.SetAnimParameter("aim_eyes", lookPos);
        
        var speed = ( Noise.Perlin( RealTime.Now * 2.0f ) - 0.33f) * 2.0f;
		if ( speed < 0 ) speed = 0;

        PlatesPlayer LocalAvatar = Game.LocalClient.Pawn as PlatesPlayer;

        if(LocalAvatar != null)
        {
            var Velocity = Vector3.Zero;
            if(LocalAvatar.Controller != null)
            {
                Velocity = LocalAvatar.Controller.Velocity;
            }
            var dir = Velocity;
            var forward = AvatarModel.Rotation.Forward.Dot( dir );
            var sideward = AvatarModel.Rotation.Right.Dot( dir );

            var angle = MathF.Atan2( sideward, forward ).RadianToDegree().NormalizeDegrees();

            AvatarModel.SetAnimParameter("idle_states", LocalAvatar.GetAnimParameterInt("idle_states"));
            AvatarModel.SetAnimParameter("b_grounded", LocalAvatar.GetAnimParameterBool("b_grounded"));
            AvatarModel.SetAnimParameter("b_climbing", LocalAvatar.GetAnimParameterBool("b_climbing"));
            AvatarModel.SetAnimParameter("b_swim", LocalAvatar.GetAnimParameterBool("b_swim"));
            AvatarModel.SetAnimParameter("b_sit", LocalAvatar.GetAnimParameterBool("b_sit"));
            AvatarModel.SetAnimParameter("move_shuffle", LocalAvatar.GetAnimParameterFloat("move_shuffle"));
            AvatarModel.SetAnimParameter("duck", LocalAvatar.GetAnimParameterFloat("duck"));
            AvatarModel.SetAnimParameter("voice", Voice.Level);

            AvatarModel.SetAnimParameter("aim_eyes", LocalAvatar.GetAnimParameterVector("aim_eyes"));
            AvatarModel.SetAnimParameter("aim_eyes_weight", LocalAvatar.GetAnimParameterFloat("aim_eyes_weight"));
            AvatarModel.SetAnimParameter("aim_head", LocalAvatar.GetAnimParameterVector("aim_head"));
            AvatarModel.SetAnimParameter("aim_head_weight", LocalAvatar.GetAnimParameterFloat("aim_head_weight"));
            AvatarModel.SetAnimParameter("aim_body", LocalAvatar.GetAnimParameterVector("aim_body"));
            AvatarModel.SetAnimParameter("aim_body_weight", LocalAvatar.GetAnimParameterFloat("aim_body_weight"));
            
            AvatarModel.SetAnimParameter( "move_direction", LocalAvatar.GetAnimParameterFloat("move_direction") );
            AvatarModel.SetAnimParameter( "move_speed", LocalAvatar.GetAnimParameterFloat("move_speed") );
            AvatarModel.SetAnimParameter( "move_groundspeed", LocalAvatar.GetAnimParameterFloat("move_groundspeed") );
            AvatarModel.SetAnimParameter( "move_y", LocalAvatar.GetAnimParameterFloat("move_y") );
            AvatarModel.SetAnimParameter( "move_x", LocalAvatar.GetAnimParameterFloat("move_x") );
            AvatarModel.SetAnimParameter( "move_z", LocalAvatar.GetAnimParameterFloat("move_z") );
        }else{
            AvatarModel.SetAnimParameter("aim_head", headPos);
            AvatarModel.SetAnimParameter("aim_head_weight", 0.5f);
            AvatarModel.SetAnimParameter("aim_body", aimPos);
            AvatarModel.SetAnimParameter("aim_body_weight", 0.25f);
        }

        AvatarModel.Update(RealTime.Delta);

        Angles angles = new(2, 180, 0);
        Vector3 pos = AvatarModel.GetBoneWorldTransform("head").Position + angles.Forward * -80 + Vector3.Up * 5;

        Camera.Position = pos;
        Camera.Rotation = Rotation.From(angles);
        Camera.Ortho = false;
        Camera.FieldOfView = 20;

        foreach(var clothingObject in ClothingObjects)
        {
            clothingObject.Update(RealTime.Delta);
        }

        LightWarm.Position = Vector3.Up * 100.0f + Vector3.Forward * 100.0f + Vector3.Right * -200;
		LightWarm.LightColor = new Color( 1.0f, 0.95f, 0.8f ) * 20.0f;
		LightWarm.Rotation = Rotation.LookAt( -LightWarm.Position );
		LightWarm.ConeInner = 50;
		LightWarm.ConeOuter = 70;
		LightWarm.Radius = 280;

		LightBlue.Radius = 250;
		LightBlue.Position = Vector3.Up * 100.0f + Vector3.Forward * 100.0f + Vector3.Right * 100;
		LightBlue.LightColor = new Color( 1f, 1f, 1f ) * 5.0f;
		LightBlue.Rotation = Rotation.LookAt( -LightBlue.Position );
		LightBlue.ConeInner = 70;
		LightBlue.ConeOuter = 70;
    }
}