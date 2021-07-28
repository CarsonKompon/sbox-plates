using System.Linq;
using Sandbox;


[RoundTypeBase]
public class BigPlatesRoundType : RoundTypeBase
{
    public BigPlatesRoundType(){
        name = "Big Plates";
        command = "round_big_plates";
    }

    public override void OnEvent(){
        foreach(var plate in Entity.All.OfType<Plate>()){
            plate.toScale = 1.5f;
            plate.Scale = 1.5f;
        }
    }
}

[RoundTypeBase]
public class RandomSizePlatesRoundType : RoundTypeBase
{
    public RandomSizePlatesRoundType(){
        name = "Random Sizes";
        command = "round_random_sizes";
    }

    public override void OnEvent(){
        foreach(var plate in Entity.All.OfType<Plate>()){
            plate.toScale = Rand.Float(0.2f, 2f);
            plate.Scale = plate.toScale;
        }
    }
}

[RoundTypeBase]
public class MicroPlatesRoundType : RoundTypeBase
{
    public MicroPlatesRoundType(){
        name = "Micro Plates";
        command = "round_micro_plates";
    }

    public override void OnEvent(){
        foreach(var plate in Entity.All.OfType<Plate>()){
            plate.toScale = 0.2f;
            plate.Scale = 0.2f;
        }
    }
}