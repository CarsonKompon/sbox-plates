using Sandbox;
using Sandbox.UI;
using System;
using System.Linq;
using System.Collections.Generic;
using Sandbox.UI.Construct;

// TODO: Implement

// [PlatesEvent]
// public class PlayerVirusEvent : PlatesEventAttribute
// {
//     public PlayerVirusEvent(){
//         name = "player_virus";
//         text = " player(s) will get the virus in ";
//         type = EventType.Player;
//     }

//     public override void OnEvent(Entity ent){
//         //(ent as PlatesPlayer).RenderColor = Color.Green;
//         new PlyVirusEnt(ent);
//         var plate = Entity.All.OfType<Plate>().OrderBy(x => Rand.Double()).ToArray()[0];
//         new VirusCureEnt(plate as Entity, ent as PlatesPlayer);
//     }
// }

// public class PlyVirusEnt : Entity
// {
//     public Entity ent;

//     public PlyVirusEnt() {}
//     public PlyVirusEnt(Entity e){
//         ent = e;
//         PlatesGame.AddEntity(this);
//     }

//     [Event.Tick]
//     public void Tick(){
//         if(ent.IsValid()){
//             var part = Particles.Create("particles/virus.vpcf");
//             part.SetPosition(0,ent.Position + Vector3.Up*40);
//             if(IsServer){
//                 ent.TakeDamage(DamageInfo.Generic( 0.001f ));
//             }
//         }else Delete();
//     }

// }

// public partial class VirusCureEnt : Prop
// {   

//     [Net] public long owner {get;set;}
// 	[Net] public string ownerName {get;set;}

//     VirusNameTag nameTag = null;

//     public VirusCureEnt(){}
//     public VirusCureEnt(Entity e, PlatesPlayer p){
//         SetModel("models/teslacoil.vmdl");
// 		SetupPhysicsFromModel(PhysicsMotionType.Dynamic);
//         RenderColor = Color.Magenta;
//         Position = e.Position + Vector3.Up*5;
//         Scale = 0.1f;
//         var client = p.Client;
//         owner = client.PlayerId;
//         ownerName = client.Name;
//         //SetParent(e);
//         PlatesGame.AddEntity(this);
//     }

//     [Event.Tick]
//     private void Tick()
//     {
//         if(nameTag != null)
//         {
//             nameTag = new VirusNameTag(this);
//         }
//     }

//     protected override void OnPhysicsCollision( CollisionEventData eventData )
//     {
//         foreach(var virus in Entity.All.OfType<PlyVirusEnt>()){
//             if(virus.ent == eventData.This.Entity){
//                 virus.Delete();
//                 Delete();
//             }
//         }
//         base.OnPhysicsCollision( eventData );
//     }

// }


// //UI STUFF
// public class VirusNameTag : WorldPanel
// {
//     public Label NameLabel;
//     //public Image Avatar;

//     VirusCureEnt virus;

//     public VirusNameTag( VirusCureEnt virus )
//     {
//         StyleSheet.Load("/events/playerevents/ui/virustags.scss");
//         this.virus = virus;

//         NameLabel = Add.Label( $"{virus.ownerName}'s Cure" );
//         //Avatar = Add.Image( $"avatar:{plate.owner}" );
//     }

//     [Event.Tick]
//     public override void Tick()
//     {
//         base.Tick();

//         Position = virus.Position + Vector3.Up * 50f;
//     }
// }