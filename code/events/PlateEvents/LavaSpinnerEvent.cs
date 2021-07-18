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
        var spinner = new PlateLavaSpinnerEnt(plate.Position + Vector3.Up * 5, plate.Scale);
        plate.PlateEnts.Add(spinner);
    }
}

public class PlateLavaSpinnerEnt : Prop
{

    public float size = 1;

    public PlateLavaSpinnerEnt() {}
    public PlateLavaSpinnerEnt(Vector3 pos, float scale){
        PlatesGame.GameEnts.Add(this);
        Position = pos;

        SetModel("models/lava_spinner.vmdl");
		SetupPhysicsFromModel(PhysicsMotionType.Dynamic);
        size = scale;
        RenderColor = Color.Red;
        Health = 0.4f;

        //Parent = plate;
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
        eventData.Entity.TakeDamage( DamageInfo.Generic( 10f ) );
        base.OnPhysicsCollision( eventData );
    }
}