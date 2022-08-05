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
    [Net] public DateTime deathTime {get;set;} 

    [Net] public List<Entity> PlateEnts {get;set;} = new();
    [Net] public Vector3 TargetPostion {get;set;}
    [Net] private RealTimeSince MovementTime {get;set;}
    private Glow glow;
    private PlateNameTag plateTag = null;

    public Plate(){}

    public Plate(Vector3 pos, float size, string own)
    {
        Tags.Add("plate");
        Position = pos;
        ownerName = own;
        TargetPostion = Position;
        
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

    [Event.Tick]
    public override void Tick(){
        base.Tick();

        if(IsClient && plateTag == null)
        {
            plateTag = new PlateNameTag(this);
        }

        if(IsServer){
            var lastScale = scale;
            scale = MathC.Lerp(scale,toScale,0.125f);
            if(scale != lastScale)
            {
                ConstructModel();
            }

            if(scale.x <= 0 || scale.y <= 0 || scale.z <= 0)
            {
                Delete();
            }
        }
        if(isDead){
            if(RenderColor.a > 0) SetAlpha(RenderColor.a - 1.0f/(7*60.0f));
        }
    }

    public void Kill(){
        if(!isDead){
            Sound.FromEntity("plates_death", this);
            isDead = true;
            SetColor(Color.Red);
            deathTime = DateTime.Now;
            //MoveTo(Position+Vector3.Up,8);
            DeleteAsync(7);
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        if(plateTag != null) plateTag.Delete();
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

    public async void Raise(float _amount, float _time = 1f)
    {
        TargetPostion = Position.WithZ(TargetPostion.z + _amount);
        if(MovementTime > 0) MovementTime = 0f;
        MovementTime -= _time;
        await LocalKeyframeTo(TargetPostion-Position, MovementTime);
    }

    public void Lower(float _amount, float _time = 1f)
    {
        // Raise(-_amount, _time);
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

    int movement = 0;
    int movementTick = 0;
    /// <summary>
    /// Move to given transform in given amount of time
    /// </summary>
    /// <param name="target">The target transform</param>
    /// <param name="seconds">How many seconds to take to move to target transform</param>
    /// <param name="easing">If set, the easing funtion</param>
    /// <returns>Whether we successded moving to given target or not</returns>
    public async Task<bool> KeyframeTo( Transform target, float seconds, Easing.Function easing = null )
    {
        var moveId = ++movement;
        var start = Transform;

        var lastTime = Time.Now;
        bool skipFirstWait = movementTick != Time.Tick;
        if ( skipFirstWait ) lastTime -= Time.Delta;
        for ( float f = 0; f < 1; )
        {
            // Do not wait on the first movement this tick
            if ( !skipFirstWait )
            {
                await Task.NextPhysicsFrame();
                if ( moveId != movement || !this.IsValid() ) return false;
            }
            skipFirstWait = false;

            var timeDelta = Math.Max( Time.Now - lastTime, 0 );
            lastTime = Time.Now;
            movementTick = Time.Tick;

            var eased = easing != null ? easing( f ) : f;

            var newtx = Transform.Lerp( start, target, eased, false );

            TryKeyframeTo( newtx );

            f += timeDelta / seconds;
        }

        Transform = target;
        return true;
    }

    /// <summary>
    /// Used by KeyframeTo methods to try to move to a given transform
    /// </summary>
    public virtual bool TryKeyframeTo( Transform pos )
    {
        Transform = pos;
        return true;
    }

    int local_movement = 0;

    /// <summary>
    /// Move to a given local position in given amount of time
    /// </summary>
    /// <param name="deltaTarget">The target local position</param>
    /// <param name="seconds">How many seconds to take to move to target transform</param>
    /// <param name="easing">If set, the easing funtion</param>
    /// <returns>Whether we successded moving to given local target or not</returns>
    public async Task<bool> LocalKeyframeTo( Vector3 deltaTarget, float seconds, Easing.Function easing = null )
    {
        var moveId = ++local_movement;
        var startPos = LocalPosition;

        var lastTime = Time.Now;
        bool skipFirstWait = movementTick != Time.Tick;
        if ( skipFirstWait ) lastTime -= Time.Delta;
        for ( float f = 0; f < 1; )
        {
            // Do not wait on the first movement this tick
            if ( !skipFirstWait )
            {
                await Task.NextPhysicsFrame();
                if ( moveId != local_movement || !this.IsValid() ) return false;
            }
            skipFirstWait = false;

            var timeDelta = Math.Max( Time.Now - lastTime, 0 );
            lastTime = Time.Now;
            movementTick = Time.Tick;

            var eased = easing != null ? easing( f ) : f;

            TryLocalKeyframeTo( Vector3.Lerp( startPos, deltaTarget, eased, false ), timeDelta );

            f += timeDelta / seconds;
        }

        LocalPosition = deltaTarget;
        LocalVelocity = 0;

        return true;
    }

    /// <summary>
    /// Used by KeyframeTo methods to try to move to a given local position
    /// </summary>
    public virtual bool TryLocalKeyframeTo( Vector3 pos, float delta )
    {
        LocalVelocity = (pos - LocalPosition) / Math.Max( delta, 0.001f );
        LocalPosition = pos;
        return true;
    }

}
