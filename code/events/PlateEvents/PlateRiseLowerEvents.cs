using Sandbox;
using System;

// TODO: Fix these events

// 
public class PlateRiseEvent : PlatesEventAttribute
{
    public PlateRiseEvent(){
        name = "plate_rise";
        text = " plate(s) will rise in ";
        type = EventType.Plate;
    }

    public override void OnEvent(Plate plate){
        plate.Rise(50f);
    }
}

// 
public class PlateLowerEvent : PlatesEventAttribute
{
    public PlateLowerEvent(){
        name = "plate_lower";
        text = " plate(s) will lower in ";
        type = EventType.Plate;
    }
    
    public override void OnEvent(Plate plate){
        plate.Rise(-50f);
    }
}

// 
public class PlateRiseRandomEvent : PlatesEventAttribute
{
    public PlateRiseRandomEvent(){
        name = "plate_rise_random";
        text = " plate(s) will rise or lower a random amount in ";
        type = EventType.Plate;
    }

    public override void OnEvent(Plate plate){
        plate.Rise(Rand.Float(-100,100));
    }
}

