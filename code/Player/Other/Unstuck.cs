using Sandbox;

namespace Plates;

public class Unstuck
{
	public WalkController Controller;

	public bool IsActive; // replicate

	internal int StuckTries = 0;

	public Unstuck( WalkController controller )
	{
		Controller = controller;
	}

	public virtual bool TestAndFix()
	{
		var result = Controller.TraceBBox( Controller.Position, Controller.Position );

		// Not stuck, we cool
		if ( !result.StartedSolid )
		{
			StuckTries = 0;
			return false;
		}

		//
		// Client can't jiggle its way out, needs to wait for
		// server correction to come
		//
		if ( Game.IsClient )
			return true;

		int AttemptsPerTick = 20;

		for ( int i=0; i< AttemptsPerTick; i++ )
		{
			var pos = Controller.Position + Vector3.Random.Normal * (((float)StuckTries) / 2.0f);

			// First try the up direction for moving platforms
			if ( i == 0 )
			{
				pos = Controller.Position + Vector3.Up * 5;
			}

			result = Controller.TraceBBox( pos, pos );

			if ( !result.StartedSolid )
			{
				Controller.Position = pos;
				return false;
			}
		}

		StuckTries++;

		return true;
	}
}