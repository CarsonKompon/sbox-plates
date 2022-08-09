using Sandbox;
using System;


public class PlateRotateEvent : PlatesEventAttribute
{
    public PlateRotateEvent(){
        name = "plate_rotate";
        text = " plate(s) will start rotating in ";
        type = EventType.Plate;
    }

    public override void OnEvent(Plate plate){
        plate.AddEntity(new PlateRotateEnt(plate));
    }
}

public partial class PlateRotateEnt : Entity
{

    [Net] public Plate plate {get;set;}

    public PlateRotateEnt(){}
    public PlateRotateEnt(Plate plat){
        plate = plat;
    }

    [Event.Tick]
    public void Tick(){
        if(IsServer){
            if(plate.IsValid()){
                plate.Rotation *= Rotation.FromYaw(1f);
                return;
            }else Delete();
        }
    }
    
}

public class PlateFlipEvent : PlatesEventAttribute
{
    public PlateFlipEvent(){
        name = "plate_flip";
        text = " plate(s) will flip in ";
        type = EventType.Plate;
        hidden = true;
    }

    public override void OnEvent(Plate plate){
        plate.AddEntity(new PlateFlipEnt(plate));
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
            if(plate.IsValid()){
                plate.Rotation *= Rotation.FromRoll(1f);
                return;
            }else Delete();
        }
    }
    
}