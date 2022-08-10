using System.Linq;
using Sandbox;
using System;

 
public class PlateLandMineEvent : PlatesEventAttribute
{

    public PlateLandMineEvent(){
        name = "plate_landmine";
        text = " plate(s) will receive a landmine in ";
        type = EventType.Plate;
        minAffected = 1;
        maxAffected = 1;
    }

    public override void OnEvent(Plate plate){
        var ent = new LandMineEnt();
        ent.Position = plate.Position + (Vector3.Up * 3f);
        var size = plate.GetSize();
        ent.Position += Vector3.Left * Rand.Int(-50,50) * size;
        ent.Position += Vector3.Forward * Rand.Int(-50,50) * size;
        plate.AddEntity(ent, true);
    }
}

public class LandMineEnt : ModelEntity
{

    public Player nearest;
    private RealTimeSince timer = 0f;

    public LandMineEnt() {}

    public override void Spawn()
    {
        SetModel("models/landmine.vmdl");
		SetupPhysicsFromModel(PhysicsMotionType.Dynamic);
        Name = "Landmine";
        RenderColor = Color.White.WithAlpha(0);
        EnableAllCollisions = false;

        var trigger = new TriggerEnt();
        trigger.SetTriggerRadius(6);
        trigger.Holder = this;
        trigger.Trigger = (Entity other) => {
            if(this.IsValid() && other.IsValid())
            {
                if(timer >= 3f && !(other is Plate) && !(other is LandMineEnt) && !(other is TriggerEnt))
                {
                    PlatesGame.Explosion(this, Position, 250, 100, 1.0f);
                    Delete();
                }
            }
            return true;
        };
        trigger.Enabled = true;

        PlatesGame.AddEntity(trigger);
    }

    [Event.Tick]
    public void Tick()
    {
        if(RenderColor.a < 1f)
        {
            RenderColor = RenderColor.WithAlpha(RenderColor.a + 0.008f);
        }
    }
}