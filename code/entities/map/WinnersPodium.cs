using Sandbox;

[Library("winners_podium")]
[Hammer.EditorModel( "models/citizen/citizen.vmdl" )]
public class WinnersPodium : ModelEntity
{
    [Property]
    public int WinPosition {get;set;} = 1;

    public override void Spawn(){
        base.Spawn();
        SetModel("");
    }
}
