using Sandbox;


 
public class PlateReturnToNormalEvent : PlatesEventAttribute
{
    public PlateReturnToNormalEvent(){
        name = "plate_return_normal";
        text = " plate(s) will return to normal in ";
        type = EventType.Plate;
    }

    public override void OnEvent(Plate plate){
        Plate newPlate = new Plate(plate.Position, 1, plate.owner);
        newPlate.SetGlow( true, Color.Blue );
        if(plate.owner is PlatesPlayer ply) ply.CurrentPlate = newPlate; 
        plate.Delete();
    }
}
