using Sandbox;

 
public class PlateGrow10Event : PlatesEventAttribute
{
    public PlateGrow10Event(){
        name = "Plate Grows 10%";
        command = "plate_grow_10";
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
        name = "Plate Grows 25%";
        command = "plate_grow_25";
        text = " plate(s) will grow 25% in ";
        type = EventType.Plate;
        rarity = EventRarity.Uncommon;
    }

    public override void OnEvent(Plate plate){
        plate.Grow(0.25f);
    }
}

 
public class PlateShrink10Event : PlatesEventAttribute
{
    public PlateShrink10Event(){
        name = "Plate Shrinks 10%";
        command = "plate_shrink_10";
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
        name = "Plate Shrinks 25%";
        command = "plate_shrink_25";
        text = " plate(s) will shrink 25% in ";
        type = EventType.Plate;
        rarity = EventRarity.Uncommon;
    }
    
    public override void OnEvent(Plate plate){
        plate.Shrink(0.25f);
    }
}

 
public class PlateShrinkInfinitely : PlatesEventAttribute
{
    public PlateShrinkInfinitely(){
        name = "Plate Shrinks Infinitely";
        command = "plate_shrink_infinitely";
        text = " plate(s) will slowly shrink infinitely in ";
        type = EventType.Plate;
        rarity = EventRarity.Rare;
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

    public override void Spawn()
    {
        // Client doesn't need to know about this
        Transmit = TransmitType.Never;
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
        name = "Plate Grows Infinitely";
        command = "plate_grow_infinitely";
        text = " plate(s) will slowly grow infinitely in ";
        type = EventType.Plate;
        rarity = EventRarity.Epic;
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

    public override void Spawn()
    {
        // Client doesn't need to know about this
        Transmit = TransmitType.Never;
    }

    [Event.Tick.Server]
    public void Tick(){
        if(plate.IsValid() && timer > 2f){
            plate.Grow(0.004f);
            timer = 0f;
        }
    }
}