using Sandbox;
using Sandbox.Component;
using System;
using System.Collections.Generic;  
using System.Threading.Tasks;

namespace Sandbox;

[Spawnable]
public partial class Plate : MeshEntity
{
    [Net] public Client owner {get;set;} = null;
    [Net] public string ownerName {get;set;}
    [Net] public bool isDead {get;set;} = false;

    [Net] public List<Entity> PlateEnts {get;set;} = new();
    [Net] private Vector3 TargetPosition {get;set;}
    [Net] private Rotation TargetRotation {get;set;}
    [Net] private RealTimeSince MovementTime {get;set;}
    [Net] private Vector3 MovementSpeed {get;set;}
    [Net] public bool IsFragile {get;set;} = false;
    private Glow glow;
    private PlateNameTag plateTag = null;

    int fadeOutIncrement = 0;

    public Plate(){}

    public Plate(Vector3 pos, float size, string own)
    {
        Tags.Add("plate");
        Position = pos;
        ownerName = own;
        TargetPosition = Position;
        TargetRotation = Rotation;
        
        var _newScale = new Vector3(size, size, 0.03f);
        scale = _newScale;
        toScale = _newScale;

        glow = Components.GetOrCreate<Glow>();
        glow.Active = false;
        glow.RangeMin = 0;
        glow.RangeMax = 2000;
        glow.Color = Color.Blue;

        motionType = PhysicsMotionType.Keyframed;
    }

    public Plate(Vector3 pos, float size, Client own) : this(pos, size, own.Name)
    {
        owner = own;
    }

    public override void Spawn(){
        base.Spawn();

        SetupPhysicsFromModel(motionType);
        EnableAllCollisions = true;
        Tags.Add("solid");
        
        RenderColor = Color.White;
    }

    public override void Tick(){
        base.Tick();

        if(IsClient && plateTag == null)
        {
            plateTag = new PlateNameTag(this);
        }

        var lastScale = scale;
        scale = MathC.Lerp(scale,toScale,0.125f);
        if(scale != lastScale)
        {
            ConstructModel();
        }
    }

    [Event.Tick.Server]
    public void ServerTick()
    {
        if(scale.x <= 0 || scale.y <= 0 || scale.z <= 0)
        {
            Delete();
        }

        if(MovementTime < 0f)
        {
            //if(motionType == PhysicsMotionType.Static) SetMotionType(PhysicsMotionType.Dynamic);
            Position += MovementSpeed;
            Velocity = MovementSpeed;
        }

        if(isDead)
        {
            if(fadeOutIncrement % 2 == 0 && RenderColor.a > 0f)
            {
                SetAlpha(RenderColor.a - 0.004f);
            }
            fadeOutIncrement++;
        }
    }

    // public override void Simulate(Client cl)
    // {
    //     base.Simulate(cl);

    //     var lastScale = scale;
    //     scale = MathC.Lerp(scale,toScale,0.125f);
    //     if(scale != lastScale)
    //     {
    //         ConstructModel();
    //     }

    //     if(scale.x <= 0 || scale.y <= 0 || scale.z <= 0)
    //     {
    //         Delete();
    //     }
    // }

    public void Kill(){
        if(!isDead){
            Sound.FromEntity("plates_death", this);
            SetColor(Color.Red);
            DeleteAsync(7);
            isDead = true;
        }
    }

    protected override void OnDestroy()
    {
        if(plateTag != null) plateTag.Delete();
        if(IsServer)
        {
            foreach(Entity ent in PlateEnts)
            {
                ent.Delete();
            }
        }
        base.OnDestroy();
    }

    public void SetMotionType(PhysicsMotionType type)
    {
        motionType = type;
        SetupPhysicsFromModel(motionType);
    }

    public void AddEntity(Entity ent, bool setTransform = false)
    {
        if(setTransform) ent.Parent = this;
        PlateEnts.Add(ent);
    }

    public void SetColor(Color color)
    {
        RenderColor = color;//.WithAlpha(RenderColor.a);
    }

    public void SetAlpha(float alpha)
    {
        RenderColor = RenderColor.WithAlpha(alpha);
    }

    public void SetGlow(bool visible, Color color = default)
    {
        if ( color == default )
            color = glow.Color;

        glow.Active = visible;
    }

    public void SetPosition(Vector3 target)
    {
        Position = target;
        TargetPosition = target;
        MovementTime = 0f;
        MovementSpeed = Vector3.Zero;
    }

    new public void MoveTo(Vector3 target, float time = 1f)
    {
        TargetPosition = target;
        if(MovementTime > 0) MovementTime = 0f;
        MovementTime -= time;
        MovementSpeed = (TargetPosition - Position) / (Math.Abs(MovementTime) * 60f);
    }

    public void MoveToLocal(Vector3 localTarget, float time = 1f)
    {
        MoveTo(TargetPosition + localTarget, time);
    }

    public void Rise(float amount, float time = 1f)
    {
        MoveTo(TargetPosition.WithZ(TargetPosition.z + amount), time);
    }

    public void SetSize(float _size)
    {
        toScale = new Vector3(_size).WithZ(toScale.z);
    }

    public float GetSize()
    {
        return toScale.x;
    }

    public void Grow(float _amount)
    {
        toScale = (toScale + _amount).WithZ(toScale.z);
    }

    public void Shrink(float _amount)
    {
        toScale = (toScale - _amount).WithZ(toScale.z);
    }

    public void SetHeight(float _height)
    {
        toScale = toScale.WithZ(_height);
    }

    public void AddHeight(float _amount)
    {
        toScale = toScale.WithZ(toScale.z + _amount);
    }

    public override void StartTouch(Entity other)
    {
        base.StartTouch(other);

        if(IsFragile && (other.Velocity.Length > 80 || Rand.Int(99999)==1))
        {
            Sound.FromWorld("plates_glass_break", Position);
            Delete();
        }
    }

}
