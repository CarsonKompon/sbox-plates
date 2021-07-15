using Sandbox;


//[EventBase]
public class PlayerRagdollEvent : EventBase
{
    public PlayerRagdollEvent(){
        name = "player_ragdoll";
        text = " player(s) will become a ragdoll for 30s in ";
        type = EventType.Player;
    }

    public override void OnEvent(Entity ent){
        var ply = (ent as PlatesPlayer);
        ply.Inventory.Add(new Pistol());
    }
}