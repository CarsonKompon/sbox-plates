using System;
using System.Collections.Generic;  

namespace Sandbox
{
	[Library( "ent_plate", Title = "Plate", Spawnable = true)]
    public partial class Plate : ModelEntity
	{

		[Net] public ulong owner {get;set;}
		[Net] public string ownerName {get;set;}
		[Net] public bool isDead {get;set;} = false;
		[Net] public DateTime deathTime {get;set;} 

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
			EnableAllCollisions = true;
			SetInteractsAs( CollisionLayer.Solid );

			// toPos = Position;
			// toScale = Scale;
		}

		[Event.Tick]
		public void Tick(){
			if(IsServer){
				Scale = MathC.Lerp(Scale,toScale,0.125f);
				//DebugOverlay.Box(Position+CollisionBounds.Mins, Position+CollisionBounds.Maxs);
				if(isDead){
					//if(RenderAlpha > 0) RenderAlpha -= 1.0f/(7*60.0f);
				}else if(Scale <= 0) Kill();
			}
			if(IsClient){
				if(isDead){
					RenderAlpha = 1.0f-((float)(DateTime.Now - deathTime).TotalMilliseconds/7000.0f);
					//RenderAlpha -= 1.0f/(7*60.0f);
					Log.Info(RenderAlpha);
				}
			}
		}

		public void Kill(){
			isDead = true;
			RenderColor = Color.Red;
			deathTime = DateTime.Now;
			MoveTo(Position+Vector3.Up,1);
			DeleteAsync(7);
		}

	}
}