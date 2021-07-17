using Sandbox;
using System;

//[EventBase]
public class PlateFlipEvent : EventBase
{
    public PlateFlipEvent(){
        name = "plate_flip";
        text = " plate(s) will flip in ";
        type = EventType.Plate;
    }

    public override void OnEvent(Plate plate){
        new PlateFlipEnt(plate);
    }
}

public partial class PlateFlipEnt : Entity
{

    [Net] public Plate plate {get;set;}

    public PlateFlipEnt(){}
    public PlateFlipEnt(Plate plat){
        plate = plat;
    }

    [Event.Tick]
    public void Tick(){
        if(IsServer){
            //Rotation += Rotation.FromYaw(1);
            plate.Rotation *= Rotation.FromRoll(1f);
        }
    }
}