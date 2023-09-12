using Sandbox;
using System;

namespace Plates;

public class TriggerEnt : ModelEntity
{
    public Func<Entity, bool> StartTrigger;
    public Func<Entity, bool> Trigger;
    public Func<Entity, bool> EndTrigger;
    public Entity Holder;
    public bool Enabled = false;

    public override void Spawn()
    {
        base.Spawn();

        // Set the default size
        SetTriggerRadius( 16 );

        // Client doesn't need to know about this
        Transmit = TransmitType.Never;
    }

    public void SetTriggerRadius( float radius )
    {
        SetupPhysicsFromCapsule(PhysicsMotionType.Keyframed, new Capsule(Vector3.Zero, Vector3.One * 0.1f, radius));
        Tags.Add("trigger");
    }

    [Event.Tick.Server]
    public void ServerTick()
    {
        if(Holder.IsValid()) Position = Holder.Position;
        else Delete();
    }

    public override void StartTouch(Entity other)
    {
        if(Enabled && StartTrigger != null) StartTrigger(other);
        base.StartTouch(other);
    }

    public override void Touch(Entity other)
    {
        if(Enabled && Trigger != null) Trigger(other);
        base.Touch(other);
    }

    public override void EndTouch(Entity other)
    {
        if(Enabled && EndTrigger != null) EndTrigger(other);
        base.EndTouch(other);
    }
}