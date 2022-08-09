using System.Linq;
using Sandbox;


 
public class IcyPlateRound : PlatesRoundAttribute
{
    public IcyPlateRound(){
        name = "Icy Plates";
        command = "round_icy_plates";
        description = "Plates are a little slippery!";
    }

    public override void OnEvent(){
        foreach(var plate in Entity.All.OfType<Plate>()){
            plate.SetSurface("plate_slippery");
            plate.SetMaterial("materials/plate_ice.vmat");
        }
    }
}