using Sandbox;
using Sandbox.UI;
using System.Collections.Generic;

public class InventoryBar : Panel
{
	readonly List<InventoryIcon> slots = new();
    //private int currentSlot = -1;

	public InventoryBar()
	{
		for ( int i = 0; i < 9; i++ )
		{
			var icon = new InventoryIcon( i + 1, this );
			slots.Add( icon );
		}
	}

	public override void Tick()
	{
		base.Tick();

		if(Game.LocalPawn is not PlatesPlayer player) return;
		if ( player.Inventory == null ) return;

		for ( int i = 0; i < slots.Count; i++ )
		{
			UpdateIcon( player.Inventory.GetSlot( i ), slots[i], i );
		}
	}

	private static void UpdateIcon( Entity ent, InventoryIcon inventoryIcon, int i )
	{
		if(Game.LocalPawn is not PlatesPlayer player) return;

		if ( ent == null )
		{
			inventoryIcon.Clear();
			return;
		}

		var di = DisplayInfo.For( ent );

		inventoryIcon.TargetEnt = ent;
		inventoryIcon.Label.Text = di.Name;
		inventoryIcon.SetClass( "active", player.ActiveChild == ent );
	}

	[GameEvent.Client.BuildInput]
	public void ProcessClientInput()
	{
		if(Game.LocalPawn is not PlatesPlayer player) return;
		var inventory = player.Inventory;
		if ( inventory == null )
			return;

		// if ( player.ActiveChild is PhysGun physgun && physgun.BeamActive )
		// {
		// 	return;
		// }

		if ( Input.Pressed( "Slot1" ) ) SetActiveSlot( inventory, 0 );
		if ( Input.Pressed( "Slot2" ) ) SetActiveSlot( inventory, 1 );
		if ( Input.Pressed( "Slot3" ) ) SetActiveSlot( inventory, 2 );
		if ( Input.Pressed( "Slot4" ) ) SetActiveSlot( inventory, 3 );
		if ( Input.Pressed( "Slot5" ) ) SetActiveSlot( inventory, 4 );
		if ( Input.Pressed( "Slot6" ) ) SetActiveSlot( inventory, 5 );
		if ( Input.Pressed( "Slot7" ) ) SetActiveSlot( inventory, 6 );
		if ( Input.Pressed( "Slot8" ) ) SetActiveSlot( inventory, 7 );
		if ( Input.Pressed( "Slot9" ) ) SetActiveSlot( inventory, 8 );

		if ( Input.MouseWheel != 0 ) SwitchActiveSlot( inventory, -Input.MouseWheel );
	}

	private static void SetActiveSlot( IBaseInventory inventory, int i )
	{
		if(Game.LocalPawn is PlatesPlayer player)
        {
            var ent = inventory.GetSlot( i );
            if ( player.ActiveChild == ent )
                return;

            if ( ent == null )
                return;
        }
    }

	private static void SwitchActiveSlot( IBaseInventory inventory, int idelta )
	{
		var count = inventory.Count();
		if ( count == 0 ) return;

		var slot = inventory.GetActiveSlot();
		var nextSlot = slot + idelta;

		while ( nextSlot < 0 ) nextSlot += count;
		while ( nextSlot >= count ) nextSlot -= count;

		SetActiveSlot( inventory, nextSlot );
	}
}