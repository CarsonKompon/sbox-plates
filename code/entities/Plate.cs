using Sandbox.Component;
using System;
using System.Collections.Generic;  

namespace Sandbox
{
	[Library( "ent_plate", Title = "Plate", Spawnable = true)]
    public partial class Plate : ModelEntity
	{

		[Net] public long owner {get;set;}
		[Net] public string ownerName {get;set;}
		[Net] public bool isDead {get;set;} = false;
		[Net] public DateTime deathTime {get;set;} 

		[Net] public float toScale {get;set;} = 1;
		[Net] public List<Entity> PlateEnts {get;set;} = new();

		private Glow glow;

		public Plate(){}

		public Plate(Vector3 pos, float size, long own, string name){
			Tags.Add("plate");
			Position = pos;
			owner = own;
			ownerName = name;
			Scale = size;
			toScale = size;

			glow = Components.GetOrCreate<Glow>();
			glow.Active = false;
			glow.RangeMin = 0;
			glow.RangeMax = 2000;
			glow.Color = Color.Blue;
		}

		public override void Spawn(){
			base.Spawn();

			SetModel("models/plate.vmdl");
			SetupPhysicsFromModel(PhysicsMotionType.Static);
			EnableAllCollisions = true;
			SetInteractsAs( CollisionLayer.Solid );

			RenderColor = Color.White;

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
					RenderColor = RenderColor.WithAlpha(1.0f-((float)(DateTime.Now - deathTime).TotalMilliseconds/7000.0f));
				}
			}
		}

		public void Kill(){
			if(!isDead){
				Sound.FromEntity("plates_death", this);
				isDead = true;
				RenderColor = Color.Red;
				deathTime = DateTime.Now;
				MoveTo(Position+Vector3.Up,8);
				DeleteAsync(7);
			}
		}

		public void SetGlow(bool visible, Color color = default)
		{
			if ( color == default )
				color = glow.Color;

			glow.Active = visible;
		}

	}
}
