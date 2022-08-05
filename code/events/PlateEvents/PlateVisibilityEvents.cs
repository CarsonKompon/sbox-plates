using Sandbox;
using System;

[PlatesEvent]
public class PlateColourEvent : PlatesEventAttribute
{

    public PlateColourEvent(){
        name = "plate_colour";
        text = " plate(s) will change colour in ";
        type = EventType.Plate;
    }

    public override void OnEvent(Plate plate){
        plate.SetColor(Color.FromBytes(Rand.Int(0,255),Rand.Int(0,255),Rand.Int(0,255)));
    }
}

[PlatesEvent]
public class PlateInvisibleEvent : PlatesEventAttribute
{
    public PlateInvisibleEvent(){
        name = "plate_invisible";
        text = " plate(s) will become invisible in ";
        type = EventType.Plate;
    }

    public override void OnEvent(Plate plate){
        plate.SetAlpha(0f);
    }
}
