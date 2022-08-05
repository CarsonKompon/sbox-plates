using Sandbox;

[PlatesEvent]
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

[PlatesEvent]
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

[PlatesEvent]
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

[PlatesEvent]
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

[PlatesEvent]
public class PlateShrinkInfinitely : PlatesEventAttribute
{
    public PlateShrinkInfinitely(){
        name = "plate_shrink_infinitely";
        text = " plate(s) will slowly shrink infinitely in ";
        type = EventType.Plate;
    }
    
    public override void OnEvent(Plate plate){
        new PlateShrinkInfinitelyEnt(plate);
    }
}

public class PlateShrinkInfinitelyEnt : Entity
{
    public Plate plate;

    public PlateShrinkInfinitelyEnt(Plate plat){
        PlatesGame.AddEntity(this);
        plate = plat;
    }

    [Event.Tick]
    public void Tick(){
        if(plate.IsValid()){
            plate.Shrink(0.0001f);
        }else Delete();
    }
}

[PlatesEvent]
public class PlateGrowInfinitely : PlatesEventAttribute
{
    public PlateGrowInfinitely(){
        name = "plate_grow_infinitely";
        text = " plate(s) will slowly grow infinitely in ";
        type = EventType.Plate;
    }
    
    public override void OnEvent(Plate plate){
        new PlateGrowInfinitelyEnt(plate);
    }
}

public class PlateGrowInfinitelyEnt : Entity
{
    public Plate plate;

    public PlateGrowInfinitelyEnt(Plate plat){
        PlatesGame.AddEntity(this);
        plate = plat;
    }

    [Event.Tick]
    public void Tick(){
        if(plate.IsValid()){
            plate.Grow(0.000005f);
        }else Delete();
    }
}