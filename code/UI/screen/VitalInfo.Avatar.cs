using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System.Collections.Generic;

public partial class VitalInfo
{
    public ScenePanel AvatarScene  {get;set;}
    public SceneWorld AvatarWorld {get;set;}

    private SceneModel CitizenModel;
    private List<SceneModel> clothingObjects = new();
    private SceneSpotLight LightWarm;
    private SceneSpotLight LightBlue;

    public ClothingContainer Container = new();

    void RefreshAvatar()
    {
        // if(AvatarWorld != null) return;

        // clothingObjects.Clear();
        // AvatarWorld = new SceneWorld();

        // CitizenModel = new SceneModel(AvatarWorld, "models/citizen/citizen.vmdl", Transform.Zero);
        // DressModel();

        // LightWarm = new SceneSpotLight( AvatarWorld, Vector3.Up * 50.0f + Vector3.Forward * 200.0f + Vector3.Right * -50, new Color( 1.0f, 1.0f, 1.0f ) * 60.0f );
		// LightWarm.Rotation = Rotation.LookAt( -LightWarm.Position );
		// LightWarm.ConeInner = 90;
        // LightWarm.ConeOuter = 120;

		// LightBlue = new SceneSpotLight( AvatarWorld, Vector3.Up * 50.0f + Vector3.Forward * -100.0f + Vector3.Right * 50, new Color( 0f, 0.02f, 0.2f ) * 100.0f );
		// LightBlue.Rotation = Rotation.LookAt( -LightBlue.Position );
		// LightBlue.ConeInner = 90;
        // LightBlue.ConeOuter = 120;

		// Angles angles = new( 25, 180, 0 );
		// Vector3 pos = Vector3.Up * 40 + angles.Direction * -100;

        // AvatarScene = Add.ScenePanel(AvatarWorld, Vector3.Zero, Rotation.Identity, 28, "avatar");

		// AvatarScene.World = AvatarWorld;
		// AvatarScene.CameraPosition = pos;
		// AvatarScene.CameraRotation = Rotation.From( angles );
		// AvatarScene.AmbientColor = Color.Gray * 0.2f;
    }

    void TickAvatar()
    {
        // if(CitizenModel == null) return;

        // if(Local.Pawn is PlatesPlayer ply)
        // {
        //     // CitizenModel.SetAnimParameter("move_x", ply.GetAnimParameterFloat("move_x"));
        //     // CitizenModel.SetAnimParameter("move_y", ply.GetAnimParameterFloat("move_y"));
        //     // CitizenModel.SetAnimParameter("move_z", ply.GetAnimParameterFloat("move_z"));
        //     CitizenModel.SetAnimParameter("move_direction", ply.GetAnimParameterFloat("move_direction"));
        //     CitizenModel.SetAnimParameter("move_speed", ply.GetAnimParameterFloat("move_speed"));
        //     CitizenModel.SetAnimParameter("move_shuffle", ply.GetAnimParameterFloat("move_shuffle"));
        //     CitizenModel.SetAnimParameter("aim_body", ply.GetAnimParameterVector("aim_body"));
        //     CitizenModel.SetAnimParameter("aim_eyes", ply.GetAnimParameterVector("aim_eyes"));
        //     CitizenModel.SetAnimParameter("aim_head", ply.GetAnimParameterVector("aim_head"));
        //     CitizenModel.SetAnimParameter("aim_body_weight", ply.GetAnimParameterFloat("aim_body_weight"));
        //     CitizenModel.SetAnimParameter("aim_head_weight", ply.GetAnimParameterFloat("aim_head_weight"));
        //     CitizenModel.SetAnimParameter("b_attack", ply.GetAnimParameterBool("b_attack"));
        //     CitizenModel.SetAnimParameter("b_climbing", ply.GetAnimParameterBool("b_climbing"));
        //     CitizenModel.SetAnimParameter("b_deploy", ply.GetAnimParameterBool("b_deploy"));
        //     CitizenModel.SetAnimParameter("b_grounded", ply.GetAnimParameterBool("b_grounded"));
        //     CitizenModel.SetAnimParameter("b_jump", ply.GetAnimParameterBool("b_jump"));
        //     CitizenModel.SetAnimParameter("b_long_idle", false);
        //     CitizenModel.SetAnimParameter("b_noclip", ply.GetAnimParameterBool("b_noclip"));
        //     CitizenModel.SetAnimParameter("b_reload", ply.GetAnimParameterBool("b_reload"));
        //     CitizenModel.SetAnimParameter("b_swim", ply.GetAnimParameterBool("b_swim"));
        //     CitizenModel.SetAnimParameter("b_weapon_lower", ply.GetAnimParameterBool("b_weapon_lower"));
        //     CitizenModel.SetAnimParameter("cycle_control", ply.GetAnimParameterFloat("cycle_control"));
        //     CitizenModel.SetAnimParameter("duck", ply.GetAnimParameterFloat("duck"));
        //     CitizenModel.SetAnimParameter("health", ply.GetAnimParameterFloat("health"));
        //     CitizenModel.SetAnimParameter("hit", ply.GetAnimParameterBool("hit"));
        //     CitizenModel.SetAnimParameter("hit_bone", ply.GetAnimParameterInt("hit_bone"));
        //     CitizenModel.SetAnimParameter("hit_direction", ply.GetAnimParameterVector("hit_direction"));
        //     CitizenModel.SetAnimParameter("hit_strength", ply.GetAnimParameterFloat("hit_strength"));
        //     CitizenModel.SetAnimParameter("voice", ply.GetAnimParameterFloat("voice"));
        //     CitizenModel.SetAnimParameter("holdtype", ply.GetAnimParameterInt("holdtype"));
        //     CitizenModel.SetAnimParameter("holdtype_handedness", ply.GetAnimParameterInt("holdtype_handedness"));
        //     CitizenModel.SetAnimParameter("holdtype_attack", ply.GetAnimParameterFloat("holdtype_attack"));
        //     CitizenModel.SetAnimParameter("holdtype_pose", ply.GetAnimParameterFloat("holdtype_pose"));
        //     CitizenModel.SetAnimParameter("holdtype_hand", ply.GetAnimParameterFloat("holdtype_hand"));
        //     CitizenModel.SetAnimParameter("idle_states", ply.GetAnimParameterInt("idle_states"));
        //     CitizenModel.SetAnimParameter("wish_x", ply.GetAnimParameterFloat("wish_x"));
        //     CitizenModel.SetAnimParameter("wish_y", ply.GetAnimParameterFloat("wish_y"));
        //     CitizenModel.SetAnimParameter("wish_z", ply.GetAnimParameterFloat("wish_z"));
        //     CitizenModel.SetAnimParameter("wish_direction", ply.GetAnimParameterFloat("wish_direction"));
        //     CitizenModel.SetAnimParameter("wish_groundspeed", ply.GetAnimParameterFloat("wish_groundspeed"));
        //     CitizenModel.SetAnimParameter("wish_speed", ply.GetAnimParameterFloat("wish_speed"));
        //     CitizenModel.SetAnimParameter("left_hand_ik.position", ply.GetAnimParameterVector("left_hand_ik.position"));
        //     CitizenModel.SetAnimParameter("right_hand_ik.position", ply.GetAnimParameterVector("right_hand_ik.position"));
        // }

        // CitizenModel.Update( RealTime.Delta );

        // Angles angles = new( 15, 180, 0 );
		// Vector3 pos = Vector3.Up * 52.5f + angles.Direction * -100;

		// AvatarScene.CameraPosition = pos;
		// AvatarScene.CameraRotation = Rotation.From( angles );

		// foreach ( var clothingObject in clothingObjects )
		// {
		// 	clothingObject.Update( RealTime.Delta );
		// }
    }

    // [Event("avatar.changed")]
    // void DressModel()
    // {
    //     Container.Deserialize(ConsoleSystem.GetValue("avatar"));
        
    //     CitizenModel.SetMaterialGroup("Skin01");

    //     foreach(var model in clothingObjects)
    //     {
    //         model?.Delete();
    //     }

    //     clothingObjects = Container.DressSceneObject(CitizenModel);
    // }

}