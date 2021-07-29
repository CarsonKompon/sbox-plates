using Sandbox;
using System;
using System.Linq;

partial class Inventory : BaseInventory
{
	public Inventory( Player player ) : base( player )
	{
	}

	public override bool CanAdd( Entity entity )
	{
		if ( !entity.IsValid() )
			return false;

		if ( !base.CanAdd( entity ) )
			return false;

		return true;//!IsCarryingType( entity.GetType() );
	}

	public override bool Add( Entity entity, bool makeActive = false )
	{
		if ( !entity.IsValid() )
			return false;

		//if ( IsCarryingType( entity.GetType() ) )
		//	return false;

		//return base.Add( entity, makeActive );

		Host.AssertServer();

		//
		// Can't pickup if already owned
		//
		if ( entity.Owner != null )
			return false;

		//
		// Let the inventory reject the entity
		//
		//if ( !CanAdd( entity ) )
		//	return false;

		//
		// Let the entity reject the inventory
		//
		if ( !entity.CanCarry( Owner ) )
			return false;

		//
		// Passed!
		//

		entity.Parent = Owner;

		//
		// Let the item do shit
		//
		entity.OnCarryStart( Owner );

		if ( makeActive )
		{
			SetActive( entity );
		}

		return true;
	}

	public bool IsCarryingType( Type t )
	{
		return List.Any( x => x?.GetType() == t );
	}

	public override bool Drop( Entity ent )
	{
		if ( !Host.IsServer )
			return false;

		if ( !Contains( ent ) )
			return false;

		ent.OnCarryDrop( Owner );

		return ent.Parent == null;
	}
}
