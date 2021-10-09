using Sandbox;
using System;

[EventBase]
public class PlateColourEvent : EventBase
{

    public PlateColourEvent(){
        name = "plate_colour";
        text = " plate(s) will change colour in ";
        type = EventType.Plate;
    }

    public override void OnEvent(Plate plate){
        plate.RenderColor = Color.FromBytes(Rand.Int(0,255),Rand.Int(0,255),Rand.Int(0,255));
    }
}

[EventBase]
public class PlateInvisibleEvent : EventBase
{
    public PlateInvisibleEvent(){
        name = "plate_invisible";
        text = " plate(s) will become invisible in ";
        type = EventType.Plate;
    }

    public override void OnEvent(Plate plate){
        plate.RenderColor = plate.RenderColor.WithAlpha(0f);
    }
}
