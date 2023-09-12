using Sandbox;
using System;

namespace Plates;

// TODO: Fix these events

// 
public class PlateRiseEvent : PlatesEvent
{
    public PlateRiseEvent(){
        name = "Plate Rises";
        command = "plate_rise";
        text = " plate(s) will rise in ";
        type = EventType.Plate;
    }

    public override void OnEvent(Plate plate){
        plate.Rise(50f);
    }
}

// 
public class PlateLowerEvent : PlatesEvent
{
    public PlateLowerEvent(){
        name = "Plate Lowers";
        command = "plate_lower";
        text = " plate(s) will lower in ";
        type = EventType.Plate;
    }
    
    public override void OnEvent(Plate plate){
        plate.Rise(-50f);
    }
}

// 
public class PlateRiseRandomEvent : PlatesEvent
{
    public PlateRiseRandomEvent(){
        name = "Plate Rise/Lower Random Amount";
        command = "plate_rise_random";
        text = " plate(s) will rise or lower a random amount in ";
        type = EventType.Plate;
        rarity = EventRarity.Uncommon;
    }

    public override void OnEvent(Plate plate){
        Random Rand = new Random();
        plate.Rise(Rand.Float(-100,100));
    }
}

