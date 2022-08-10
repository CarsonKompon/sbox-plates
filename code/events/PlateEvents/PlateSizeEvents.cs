using Sandbox;

 
public class PlateGrow10Event : PlatesEventAttribute
{
    public PlateGrow10Event(){
        name = "plate_grow_10";
        text = " plate(s) will grow 10% in ";
        type = EventType.Plate;
    }

    public override void OnEvent(Plate plate){
        plate.Grow(0.1f);
    }
}

 
public class PlateGrow25Event : PlatesEventAttribute
{
    public PlateGrow25Event(){
        name = "plate_grow_25";
        text = " plate(s) will grow 25% in ";
        type = EventType.Plate;
    }

    public override void OnEvent(Plate plate){
        plate.Grow(0.25f);
    }
}

 
public class PlateShrink10Event : PlatesEventAttribute
{
    public PlateShrink10Event(){
        name = "plate_shrink_10";
        text = " plate(s) will shrink 10% in ";
        type = EventType.Plate;
    }
    
    public override void OnEvent(Plate plate){
        plate.Shrink(0.1f);
    }
}

 
public class PlateShrink25Event : PlatesEventAttribute
{
    public PlateShrink25Event(){
        name = "plate_shrink_25";
        text = " plate(s) will shrink 25% in ";
        type = EventType.Plate;
    }
    
    public override void OnEvent(Plate plate){
        plate.Shrink(0.25f);
    }
}

 
public class PlateShrinkInfinitely : PlatesEventAttribute
{
    public PlateShrinkInfinitely(){
        name = "plate_shrink_infinitely";
        text = " plate(s) will slowly shrink infinitely in ";
        type = EventType.Plate;
    }
    
    public override void OnEvent(Plate plate){
        plate.AddEntity(new PlateShrinkInfinitelyEnt(plate));
    }
}

public partial class PlateShrinkInfinitelyEnt : Entity
{
    [Net] public Plate plate {get;set;}
    private RealTimeSince timer = 0f;

    public PlateShrinkInfinitelyEnt(){}
    public PlateShrinkInfinitelyEnt(Plate plat){
        plate = plat;
    }

    [Event.Tick.Server]
    public void Tick(){
        if(plate.IsValid() && timer > 0.5f){
            plate.Shrink(0.004f);
            timer = 0f;
        }
    }
}

 
public class PlateGrowInfinitely : PlatesEventAttribute
{
    public PlateGrowInfinitely(){
        name = "plate_grow_infinitely";
        text = " plate(s) will slowly grow infinitely in ";
        type = EventType.Plate;
    }
    
    public override void OnEvent(Plate plate){
        plate.AddEntity(new PlateGrowInfinitelyEnt(plate));
    }
}

public partial class PlateGrowInfinitelyEnt : Entity
{
    [Net] public Plate plate {get;set;}
    private RealTimeSince timer = 0f;

    public PlateGrowInfinitelyEnt(){}
    public PlateGrowInfinitelyEnt(Plate plat){
        plate = plat;
    }

    [Event.Tick.Server]
    public void Tick(){
        if(plate.IsValid() && timer > 2f){
            plate.Grow(0.004f);
            timer = 0f;
        }
    }
}