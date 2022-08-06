using System.Xml.Schema;
using System;
using System.Collections.Generic;
using Sandbox;

 
public class PlateLavaSpinnerEvent : PlatesEventAttribute
{
    public PlateLavaSpinnerEvent(){
        name = "plate_lava_spinner";
        text = " plate(s) will get a lava spinner in ";
        type = EventType.Plate;
    }
    
    public override void OnEvent(Plate plate){
        var spinner = new PlateLavaSpinnerEnt(plate.Position + Vector3.Up * 5, plate.Scale);
        plate.AddEntity(spinner, true);
    }
}

public class PlateLavaSpinnerEnt : ModelEntity
{

    public float size = 1;

    public PlateLavaSpinnerEnt() {}
    public PlateLavaSpinnerEnt(Vector3 pos, float scale){
        Position = pos;

        SetModel("models/lava_spinner.vmdl");
		SetupPhysicsFromModel(PhysicsMotionType.Keyframed);
        size = scale;
        RenderColor = Color.Red;
        //Health = 0.4f;

        //Parent = plate;
    }

    [Event.Tick]
    public void Tick(){
        if(IsServer){
            //Rotation += Rotation.FromYaw(1);
            //LocalRotation *= Rotation.FromYaw(1f);
            Scale = size;
        }
    }


    public override void Touch( Entity other )
    {
        base.Touch(other);
        other.TakeDamage( DamageInfo.Generic( 0.05f ) );
    }
}