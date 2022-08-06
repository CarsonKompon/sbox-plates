using System;
using System.Linq;
using Sandbox;


public class CircleFormationRoundType : PlatesRoundAttribute
{
    public CircleFormationRoundType(){
        name = "Circular Formation";
        command = "round_circle_form";
        description = "All plates form a circle surrounding the middle of the Arena";
    }

    public override void OnEvent()
    {
        float clientCount = PlatesGame.GameClients.Count;
        float distance = MathX.Clamp((clientCount/32f)*1200f, 100f, 1200f);
        var i = 0;
        foreach(var client in PlatesGame.GameClients)
        {
            if(client.Pawn is PlatesPlayer ply && ply.CurrentPlate is Plate plate)
            {
                plate.Position = new Vector3(MathF.Sin(2f * MathF.PI * ((float)i/(float)clientCount))*distance, MathF.Cos(2f * MathF.PI * ((float)i/(float)clientCount))*distance, 0);
                ply.Position = plate.Position + Vector3.Up*100;
            }
            i++;
        }

        // var allPlates = Entity.All.OfType<Plate>();
        // float plateCount = allPlates.ToList().Count;
        // var distance = MathX.Clamp((plateCount/32f)*1200f, 100f, 1200f);
        // var i = 0;
        // foreach(var plate in allPlates){
        //     plate.Position = new Vector3(MathF.Sin(2f * MathF.PI * ((float)i/(float)plateCount))*distance, MathF.Cos(2f * MathF.PI * ((float)i/(float)plateCount))*distance, 0);
        //     if(plate.owner is Client client)
        //     {
        //         client.Pawn.Position = plate.Position + Vector3.Up * 100f;
        //     }
        //     i++;
        // }
    }
}