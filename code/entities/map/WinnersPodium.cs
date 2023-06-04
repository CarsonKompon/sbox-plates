using Sandbox;
using Editor;

[Library("plates_winners_podium", Description = "A character to be placed on a podium. This represents the winner in the specified position on round end."), HammerEntity]
[EditorModel("models/citizen/citizen.vmdl")]
public partial class WinnersPodium : AnimatedEntity
{
    [Property]
    public int WinPosition {get;set;} = 1;
    private ClothingContainer Clothing = new();

    public override void Spawn()
    {
        base.Spawn();
        SetModel("models/citizen/citizen.vmdl");
    }

    public void Dress(IClient client)
    {
        Clothing.LoadFromClient(client);
        Clothing.DressEntity(this);
    }
}