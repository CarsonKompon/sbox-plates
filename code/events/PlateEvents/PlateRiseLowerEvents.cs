using Sandbox;

[EventBase]
public class PlateRiseEvent : EventBase
{
    public PlateRiseEvent(){
        name = "plate_rise";
        text = " plate(s) will rise in ";
        type = EventType.Plate;
    }

    public override void OnEvent(Plate plate){
        plate.MoveTo(plate.Position + Vector3.Up * 50, 1);
    }
}

[EventBase]
public class PlateLowerEvent : EventBase
{
    public PlateLowerEvent(){
        name = "plate_lower";
        text = " plate(s) will lower in ";
        type = EventType.Plate;
    }
    
    public override void OnEvent(Plate plate){
        plate.MoveTo(plate.Position - Vector3.Up * 50, 1);
    }
}

