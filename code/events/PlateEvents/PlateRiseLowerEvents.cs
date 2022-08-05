using Sandbox;
using System;

// TODO: Fix these events

//[PlatesEvent]
public class PlateRiseEvent : PlatesEventAttribute
{
    public PlateRiseEvent(){
        name = "plate_rise";
        text = " plate(s) will rise in ";
        type = EventType.Plate;
    }

    public override void OnEvent(Plate plate){
        plate.Raise(50f);
    }
}

//[PlatesEvent]
public class PlateLowerEvent : PlatesEventAttribute
{
    public PlateLowerEvent(){
        name = "plate_lower";
        text = " plate(s) will lower in ";
        type = EventType.Plate;
    }
    
    public override void OnEvent(Plate plate){
        plate.Lower(50f);
    }
}

//[PlatesEvent]
public class PlateRiseRandomEvent : PlatesEventAttribute
{
    public PlateRiseRandomEvent(){
        name = "plate_rise_random";
        text = " plate(s) will rise or lower a random amount in ";
        type = EventType.Plate;
    }

    public override void OnEvent(Plate plate){
        plate.Raise(Rand.Float(-100,100));
    }
}

