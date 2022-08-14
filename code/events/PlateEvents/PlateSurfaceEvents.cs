using Sandbox;

 
public class PlateIceEvent : PlatesEventAttribute
{
    public PlateIceEvent(){
        name = "Icy Plate";
        command = "plate_ice";
        text = " plate(s) will become an ice rink in ";
        type = EventType.Plate;
        rarity = EventRarity.Uncommon;
    }

    public override void OnEvent(Plate plate){
        plate.SetSurface("plate_slippery");
        plate.SetMaterial("materials/plate_ice.vmat");
    }
}
