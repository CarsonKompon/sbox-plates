using Sandbox;
using System;

 
public class PlateColourEvent : PlatesEventAttribute
{

    public PlateColourEvent(){
        name = "Plate Colour Change";
        command = "plate_colour";
        text = " plate(s) will change colour in ";
        type = EventType.Plate;
    }

    public override void OnEvent(Plate plate){
        plate.SetColor(Color.FromBytes(Rand.Int(0,255),Rand.Int(0,255),Rand.Int(0,255)));
    }
}

 
public class PlateInvisibleEvent : PlatesEventAttribute
{
    public PlateInvisibleEvent(){
        name = "Invisible Plate";
        command = "plate_invisible";
        text = " plate(s) will become invisible in ";
        type = EventType.Plate;
    }

    public override void OnEvent(Plate plate){
        plate.SetAlpha(0f);
    }
}

public class PlateFadeInOutEvent : PlatesEventAttribute
{
    public PlateFadeInOutEvent(){
        name = "Plate Fade In And Out";
        command = "plate_fade_in_out";
        text = " plate(s) will fade in and out in ";
        type = EventType.Plate;
    }

    public override void OnEvent(Plate plate){
        plate.AddEntity(new PlateFadeInOutEnt(plate));
    }
}

public partial class PlateFadeInOutEnt : Entity
{
    [Net] public Plate plate {get;set;}
    [Net] public RealTimeSince timer {get;set;}
    [Net] public bool fadeIn {get;set;} = false;

    public PlateFadeInOutEnt(){}
    public PlateFadeInOutEnt(Plate pl)
    {
        plate = pl;
    }

    [Event.Tick.Server]
    public void Tick()
    {
        if(fadeIn)
        {
            if(plate.RenderColor.a < 1f) plate.SetAlpha(plate.RenderColor.a + 0.004f);
        }
        else
        {
            if(plate.RenderColor.a > 0f) plate.SetAlpha(plate.RenderColor.a - 0.004f);
        }
        if(timer >= 5f)
        {
            fadeIn = !fadeIn;
            timer = 0;
        }
    }
}
