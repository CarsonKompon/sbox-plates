using Sandbox;
using SandboxEditor;

[Library("plates_casino_event_wheel", Description = "The big wheel in the casino that determines the fate of the current game")]
[HammerEntity, EditorModel("models/casino/spinning_wheel.vmdl")]
public partial class EventWheel : Prop, IUse
{
    [Net] public float Speed {get;set;} = 0f;
    [Net] public bool Spinning {get;set;} = false;
    [Net] public RealTimeSince Timer {get;set;} = 0f;

    public override void Spawn()
    {
        base.Spawn();
        SetModel("models/casino/spinning_wheel.vmdl");
        SetupPhysicsFromModel(PhysicsMotionType.Keyframed);
    }

    public bool IsUsable( Entity user )
	{
        return !Spinning && PlayerDataManager.HasMoney(user.Client.PlayerId, 1);
		return (int)PlatesGame.GameState >= (int)PlatesGameState.SELECTING_EVENT;
	}


	public bool OnUse( Entity user )
	{
		if(IsServer)
        {
            PlayerDataManager.SpendMoney(user.Client.PlayerId, 1);
            Speed = Rand.Float(10,50);
            Spinning = true;
        }
        return false;
	}
    
    [Event.Tick.Server]
    public void Tick()
    {
        if(Speed > 0f)
        {
            Speed -= 0.05f;
            if(Speed <= 0f)
            {
                Speed = 0f;
            }
        }
        Rotation *= Rotation.FromYaw(Speed);
    }
}