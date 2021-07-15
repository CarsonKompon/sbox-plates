using System.Xml.Schema;
using System;
using System.Collections.Generic;
using Sandbox;

[EventBase]
public class PlateLavaSpinnerEvent : EventBase
{
    public PlateLavaSpinnerEvent(){
        name = "plate_lava_spinner";
        text = " plate(s) will get a lava spinner in ";
        type = EventType.Plate;
    }
    
    public override void OnEvent(Plate plate){
        var spinner = new PlateLavaSpinnerEnt(plate.Position + Vector3.Up * 5, plate);
    }
}

public class PlateLavaSpinnerEnt : ModelEntity
{

    public float size = 1;

    public PlateLavaSpinnerEnt(){}
    public PlateLavaSpinnerEnt(Vector3 pos, Plate plate){
        PlatesGame.GameEnts.Add(this);
        Position = pos;

        SetModel("models/lava_spinner.vmdl");
        MoveType = MoveType.Physics;
		//SetupPhysicsFromModel(PhysicsMotionType.Static);
        size = plate.Scale;
        RenderColor = Color.Red;

        Parent = plate;
    }

    [Event.Tick]
    public void Tick(){
        if(IsServer){
            //Rotation += Rotation.FromYaw(1);
            Rotation *= Rotation.FromYaw(1f);
            Scale = size;
        }
    }

    protected override void OnPhysicsCollision( CollisionEventData eventData )
    {
        eventData.Entity.TakeDamage( DamageInfo.Generic( 99999f ) );
        base.OnPhysicsCollision( eventData );
    }
}