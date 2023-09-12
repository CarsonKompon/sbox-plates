using System;
using Sandbox;

namespace Plates;

public partial class FlashlightComponent : EntityComponent<Player>
{
    [ClientInput] private bool FlashlightEnabled { get; set; } = false;
	private SpotLightEntity ViewFlashlight;
	private SpotLightEntity WorldFlashlight;

    protected override void OnActivate()
    {
        if(Game.IsServer)
        {
            WorldFlashlight = CreateFlashlight();
            WorldFlashlight.EnableHideInFirstPerson = true;
            WorldFlashlight.SetParent(Entity, "head" );
        }
        if(Game.IsClient)
        {
            ViewFlashlight = CreateFlashlight();
		    ViewFlashlight.EnableViewmodelRendering = true;
        }
    }

    public void Simulate()
    {
        if(WorldFlashlight.IsValid())
		{
			var bone = Entity.GetBoneTransform(Entity.GetBoneIndex("head"));
			WorldFlashlight.Enabled = FlashlightEnabled;
			WorldFlashlight.Position = bone.Position + bone.Rotation.Left * 8f;
			WorldFlashlight.Rotation = Rotation.LookAt(bone.Rotation.Left);
		}
		if(ViewFlashlight.IsValid()) ViewFlashlight.Enabled = FlashlightEnabled;
    }

    public void FrameSimulate()
    {
        if(ViewFlashlight.IsValid()) ViewFlashlight.Transform = new Transform(Camera.Position + (Camera.Rotation.Forward * 5f), Camera.Rotation);
    }

    [GameEvent.Client.BuildInput]
    internal void BuildInput()
    {
        if(Input.Pressed("flashlight"))
		{
			FlashlightEnabled = !FlashlightEnabled;
			Sound.FromEntity( FlashlightEnabled ? "flashlight-on" : "flashlight-off", Entity );
		}
    }

    public SpotLightEntity CreateFlashlight()
	{
		var light = new SpotLightEntity
		{
			Enabled = true,
			DynamicShadows = true,
			Range = 512,
			Falloff = 1.0f,
			LinearAttenuation = 0.0f,
			QuadraticAttenuation = 1.0f,
			Brightness = 2.0f,
			Color = Color.White,
			InnerConeAngle = 30,
			OuterConeAngle = 50,
			FogStrength = 1.0f,
			LightCookie = Texture.Load( "materials/effects/lightcookie.vtex" ),
			Transmit = TransmitType.Always,
			
		};

		return light;
	}
}