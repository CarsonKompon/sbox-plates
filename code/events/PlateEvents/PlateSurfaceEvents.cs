using Sandbox;

 
public class PlateSlipperyEvent : PlatesEventAttribute
{
    public PlateSlipperyEvent(){
        name = "plate_slippery";
        text = " plate(s) will become slippery in ";
        type = EventType.Plate;
    }

    public override void OnEvent(Plate plate){
        plate.SetSurface("plate_slippery");
    }
}
