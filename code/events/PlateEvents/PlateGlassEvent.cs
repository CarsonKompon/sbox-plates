using Sandbox;

 
public class PlateGlassEvent : PlatesEventAttribute
{
    public PlateGlassEvent(){
        name = "Glass Plate";
        command = "plate_glass";
        text = " plate(s) will become glass in ";
        type = EventType.Plate;
        rarity = EventRarity.Uncommon;
    }

    public override void OnEvent(Plate plate){
        plate.IsFragile = true;
        plate.SetSurface("glass");
        plate.SetMaterial("materials/plate_glass.vmat");
    }
}
