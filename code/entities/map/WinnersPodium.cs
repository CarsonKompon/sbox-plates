using Sandbox;

[Library("winners_podium")]
[Hammer.EditorModel( "models/citizen/citizen.vmdl" )]
public class WinnersPodium : ModelEntity
{
    [Property]
    public int WinPosition {get;set;} = 1;

    ModelEntity pants;
	ModelEntity jacket;
	ModelEntity shoes;
	ModelEntity hat;

    public override void Spawn(){
        base.Spawn();
        SetModel("");
    }

    public void Dress(PlatesPlayer ply)
	{
		foreach(var c in this.Children){
            c.Delete();
        }

        pants = new ModelEntity();
        pants.SetModel( ply.pants.GetModel() );
        pants.SetParent( this, true );
        pants.EnableShadowInFirstPerson = true;
        pants.EnableHideInFirstPerson = true;

        SetBodyGroup( "Legs", 1 );


        jacket = new ModelEntity();
        jacket.SetModel( ply.jacket.GetModel() );
        jacket.SetParent( this, true );
        jacket.EnableShadowInFirstPerson = true;
        jacket.EnableHideInFirstPerson = true;

        var propInfo = jacket.GetModel().GetPropData();
        if ( propInfo.ParentBodyGroupName != null )
        {
            SetBodyGroup( propInfo.ParentBodyGroupName, propInfo.ParentBodyGroupValue );
        }
        else
        {
            SetBodyGroup( "Chest", 0 );
        }


        shoes = new ModelEntity();
        shoes.SetModel( ply.shoes.GetModel() );
        shoes.SetParent( this, true );
        shoes.EnableShadowInFirstPerson = true;
        shoes.EnableHideInFirstPerson = true;

        SetBodyGroup( "Feet", 1 );


        hat = new ModelEntity();
        hat.SetModel( ply.hat.GetModel() );
        hat.SetParent( this, true );
        hat.EnableShadowInFirstPerson = true;
        hat.EnableHideInFirstPerson = true;
	}
}
