using Sandbox;


[EventBase]
public class PlateReturnToNormalEvent : EventBase
{
    public PlateReturnToNormalEvent(){
        name = "plate_return_normal";
        text = " plate(s) will return to normal in ";
        type = EventType.Plate;
    }

    public override void OnEvent(Plate plate){
        Plate newPlate = new Plate(plate.Position, 1, plate.owner, plate.ownerName);
        newPlate.GlowActive = true;
		newPlate.GlowColor = Color.Blue;
        plate.Delete();
    }
}