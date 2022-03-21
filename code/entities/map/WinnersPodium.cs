using Sandbox;

[Library("plates_winners_podium", Description = "A character to be placed on a podium. This represents the winner in the specified position on round end." )]
[Hammer.EditorModel( "models/citizen/citizen.vmdl")]
public class WinnersPodium : ModelEntity
{
    [Property]
    public int WinPosition {get;set;} = 1;

    public override void Spawn(){
        base.Spawn();
        base.SetModel("");
    }

    public void Init(PlatesPlayer player){
        
    }

    public void Dress(PlatesPlayer ply)
	{
		foreach(var c in Children){
            c.Delete();
        }

        foreach(var c in ply.Children){
            var clothes = new ModelEntity();
            clothes.SetModel( (c as ModelEntity).GetModelName() );
            clothes.SetParent( this, true );
        }
	}
}
