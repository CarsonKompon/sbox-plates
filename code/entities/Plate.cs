using System.Collections.Generic;  

namespace Sandbox
{
	[Library( "ent_plate", Title = "Plate", Spawnable = true)]
    public partial class Plate : Prop
	{

		[Net] public ulong owner {get;set;}
		[Net] public string ownerName {get;set;}

		[Net] public float toScale {get;set;} = 1;
		[Net] public List<Entity> PlateEnts {get;set;} = new();

		public Plate(){}

		public Plate(Vector3 pos, float size, ulong own, string name){
			Tags.Add("plate");
			Position = pos;
			owner = own;
			ownerName = name;
			Scale = size;
			toScale = size;
		}

		public override void Spawn(){
			base.Spawn();

			SetModel("models/plate.vmdl");
			SetupPhysicsFromModel(PhysicsMotionType.Static);

			// toPos = Position;
			// toScale = Scale;
		}

		[Event.Tick]
		public void Tick(){
			if(IsServer){
				Scale = MathC.Lerp(Scale,toScale,0.125f);
				//DebugOverlay.Box(Position+CollisionBounds.Mins, Position+CollisionBounds.Maxs);
				if(Scale <= 0) Kill();
			}
		}

		public void Kill(){
			MoveTo(Position+Vector3.Up,1);
			DeleteAsync(0.1f);
		}

	}
}