using Sandbox;

[EventBase]
public class PlateGrow10Event : EventBase
{
    public PlateGrow10Event(){
        name = "plate_grow_10";
        text = " plate(s) will grow 10% in ";
        type = EventType.Plate;
    }

    public override void OnEvent(Plate plate){
        plate.toScale += 0.10f;
    }
}

[EventBase]
public class PlateGrow25Event : EventBase
{
    public PlateGrow25Event(){
        name = "plate_grow_25";
        text = " plate(s) will grow 25% in ";
        type = EventType.Plate;
    }

    public override void OnEvent(Plate plate){
        plate.toScale += 0.25f;
    }
}

[EventBase]
public class PlateShrink10Event : EventBase
{
    public PlateShrink10Event(){
        name = "plate_shrink_10";
        text = " plate(s) will shrink 10% in ";
        type = EventType.Plate;
    }
    
    public override void OnEvent(Plate plate){
        plate.toScale -= 0.10f;
        if(plate.toScale < 0) plate.toScale = 0;
    }
}

[EventBase]
public class PlateShrink25Event : EventBase
{
    public PlateShrink25Event(){
        name = "plate_shrink_25";
        text = " plate(s) will shrink 25% in ";
        type = EventType.Plate;
    }
    
    public override void OnEvent(Plate plate){
        plate.toScale -= 0.25f;
        if(plate.toScale < 0) plate.toScale = 0;
    }
}

[EventBase]
public class PlateShrinkInfinitely : EventBase
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
        PlatesGame.GameEnts.Add(this);
        plate = plat;
    }

    [Event.Tick]
    public void Tick(){
        if(plate.IsValid()){
            plate.toScale -= 0.0001f;
        }else Delete();
    }
}

[EventBase]
public class PlateGrowInfinitely : EventBase
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
        PlatesGame.GameEnts.Add(this);
        plate = plat;
    }

    [Event.Tick]
    public void Tick(){
        if(plate.IsValid()){
            plate.toScale += 0.000005f;
        }else Delete();
    }
}