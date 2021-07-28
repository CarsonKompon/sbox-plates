using System.Linq;
using Sandbox;


[RoundTypeBase]
public class KnifeRoundType : RoundTypeBase
{
    public KnifeRoundType(){
        name = "Knife Party";
        command = "round_knife";
    }

    public override void OnEvent(){
        foreach(var ply in Client.All){
            (ply.Pawn as PlatesPlayer).Inventory.Add(new SWB_CSS.Knife());
        }
    }
}