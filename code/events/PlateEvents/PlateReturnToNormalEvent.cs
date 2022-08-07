using Sandbox;


 
public class PlateReturnToNormalEvent : PlatesEventAttribute
{
    public PlateReturnToNormalEvent(){
        name = "plate_return_normal";
        text = " plate(s) will return to normal in ";
        type = EventType.Plate;
    }

    public override void OnEvent(Plate plate){
        Plate newPlate;
        if(plate.owner.IsValid())
        {
            newPlate = new Plate(plate.Position, 1, plate.owner);
            if(plate.owner is PlatesPlayer ply)
            {
                ply.CurrentPlate = newPlate;
            }
        }
        else
        {
            newPlate = new Plate(plate.Position, 1, plate.ownerName);
        }
        newPlate.SetGlow( true, Color.Blue );
        plate.Delete();
    }
}
