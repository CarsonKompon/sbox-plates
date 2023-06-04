using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using Editor;
using System;
using System.Collections.Generic;

[Library("plates_casino_event_wheel", Description = "The big wheel in the casino that determines the fate of the current game")]
[HammerEntity, EditorModel("models/casino/spinning_wheel.vmdl")]
public partial class EventWheel : Prop, IUse
{
    [Net] public float Speed {get;set;} = 0f;
    [Net] public bool Spinning {get;set;} = false;
    [Net] public RealTimeSince Timer {get;set;} = 0f;
    private List<EventWheelEntryUI> screens = new();

    public override void Spawn()
    {
        base.Spawn();
        SetModel("models/casino/spinning_wheel.vmdl");
        SetupPhysicsFromModel(PhysicsMotionType.Keyframed);
    }

    public override void ClientSpawn()
    {
        base.ClientSpawn();
        InitWheel();
    }

    public void ClearWheel()
    {
        foreach(var screen in screens)
        {
            screen.Delete();
        }
        screens.Clear();
    }

    public void InitWheel(int options = 8)
    {
        ClearWheel();
        for(int i=0; i<options; i++)
        {
            var screen = new EventWheelEntryUI(this, i, options);
            screen.RootPosition = Position + Rotation.Up * 40f;
        }
    }

    public bool IsUsable( Entity user )
	{
        return !Spinning && PlayerDataManager.HasMoney(user.Client.SteamId, 1);
		return (int)PlatesGame.GameState >= (int)PlatesGameState.SELECTING_EVENT; // TODO: Implement this
	}


	public bool OnUse( Entity user )
	{
		if(Game.IsServer)
        {
            Random Rand = new();
            PlayerDataManager.SpendMoney(user.Client.SteamId, 1);
            Speed = Rand.Float(10,20);
            Spinning = true;
        }
        return false;
	}
    
    [Event.Tick.Server]
    public void Tick()
    {
        if(Speed > 0f)
        {
            Rotation *= Rotation.FromYaw(Speed);
            Speed -= 0.05f;
            if(Speed <= 0f)
            {
                Speed = 0f;
                Spinning = false;
            }
        }
    }
}

public class EventWheelEntryUI : WorldPanel
{
    public EventWheel Holder;
    public Label Label;
    public Vector3 RootPosition;
    private int Index = 0;
    private int Count = 1;
    private float lastAngle = 0f;
    private float lastCorrectedAngle = -420f;
    private int lastDirection = 1;
    public EventWheelEntryUI()
    {
        StyleSheet.Load("/entities/map/casino/eventwheel.scss");
        Label = Add.Label("Entry", "text");

        var width = 256 * 20;
        var height = 196 * 20;
        PanelBounds = new Rect(-width * .5f, -height * .5f, width, height);
    }

    public EventWheelEntryUI(EventWheel creator, int index, int count) : this()
    {
        Holder = creator;
        Index = index;
        Count = count;
    }

    public override void Tick()
    {
        base.Tick();

        var angleOffset = (360f / (float)Count) * (float)Index;
        var angle = Holder.Rotation.Pitch() + angleOffset;
        var correctedAngle = angle;
        var distance = 320f;
        if(angle > lastAngle) lastDirection = 1;
        else if(angle < lastAngle) lastDirection = -1;
        if(lastDirection == 1) correctedAngle = angle + 90;
        else if(lastDirection == -1) correctedAngle = 270f - angle;
        lastCorrectedAngle = correctedAngle;
        if(Holder.Speed < MathF.Abs(correctedAngle - lastCorrectedAngle)) correctedAngle = lastCorrectedAngle;
        //Rotation = Holder.Rotation * Rotation.FromPitch(-90);//Rotation.LookAt(Holder.Rotation.Left, Holder.Rotation.Backward);
        Rotation = Holder.Rotation * Rotation.FromPitch(-90) * Rotation.FromRoll(correctedAngle-90);
        //if(Index == 0) Log.Info(correctedAngle);
        Position = RootPosition + (Holder.Rotation.Backward * MathF.Cos(MathX.DegreeToRadian(correctedAngle)) * distance) + (Holder.Rotation.Right * MathF.Sin(MathX.DegreeToRadian(correctedAngle)) * distance);
    
        lastAngle = angle;
    }


}