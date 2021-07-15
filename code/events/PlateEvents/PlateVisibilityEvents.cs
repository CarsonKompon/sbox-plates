using Sandbox;
using System;

[EventBase]
public class PlateColourEvent : EventBase
{
    Random random = new Random();

    public PlateColourEvent(){
        name = "plate_colour";
        text = " plate(s) will change colour in ";
        type = EventType.Plate;
    }

    public override void OnEvent(Plate plate){
        plate.RenderColor = Color.FromBytes(random.Next(0,255),random.Next(0,255),random.Next(0,255));
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
        plate.RenderAlpha = 0f;
    }
}
