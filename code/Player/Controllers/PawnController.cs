using Sandbox;
using System.Collections.Generic;

namespace Plates
{
	public class PawnController : EntityComponent<Player>
	{
		internal HashSet<string> Events;
		internal HashSet<string> Tags;

		public Vector3 Position { get; set; }
		public Rotation Rotation { get; set; }
		public Vector3 Velocity { get; set; }
		public Rotation EyeRotation { get; set; }
		public Vector3 EyeLocalPosition { get; set; }
		public Vector3 BaseVelocity { get; set; }
		public Entity GroundEntity { get; set; }
		public Vector3 GroundNormal { get; set; }

		public Vector3 WishVelocity { get; set; }
		public virtual bool HasAnimations => true;
		public virtual bool HasRotation => true;

		public void UpdateFromEntity()
		{
			Position = Entity.Position;
			Rotation = Entity.Rotation;
			Velocity = Entity.Velocity;

			EyeRotation = Entity.EyeRotation;
			EyeLocalPosition = Entity.EyeLocalPosition;

			BaseVelocity = Entity.BaseVelocity;
			GroundEntity = Entity.GroundEntity;
			WishVelocity = Entity.Velocity;
		}

		public void UpdateEntity( )
		{
			Entity.Position = Position;
			Entity.Velocity = Velocity;
			Entity.Rotation = Rotation;
			Entity.GroundEntity = GroundEntity;
			Entity.BaseVelocity = BaseVelocity;

			Entity.EyeLocalPosition = EyeLocalPosition;
			Entity.EyeRotation = EyeRotation;
		}

		/// <summary>
		/// This is what your logic should be going in
		/// </summary>
		public virtual void Simulate()
		{
			// Nothing
		}

		/// <summary>
		/// This is called every frame on the client only
		/// </summary>
		public virtual void FrameSimulate()
		{
			Game.AssertClient();
		}

		/// <summary>
		/// Call OnEvent for each event
		/// </summary>
		public virtual void RunEvents( PawnController additionalController )
		{
			if ( Events == null ) return;

			foreach ( var e in Events )
			{
				OnEvent( e );
				additionalController?.OnEvent( e );
			}
		}

		/// <summary>
		/// An event has been triggered - maybe handle it
		/// </summary>
		public virtual void OnEvent( string name )
		{

		}

		/// <summary>
		/// Returns true if we have this event
		/// </summary>
		public bool HasEvent( string eventName )
		{
			if ( Events == null ) return false;
			return Events.Contains( eventName );
		}

		/// <summary>
		/// </summary>
		public bool HasTag( string tagName )
		{
			if ( Tags == null ) return false;
			return Tags.Contains( tagName );
		}


		/// <summary>
		/// Allows the controller to pass events to other systems
		/// while staying abstracted.
		/// For example, it could pass a "jump" event, which could then
		/// be picked up by the playeranimator to trigger a jump animation,
		/// and picked up by the player to play a jump sound.
		/// </summary>
		public void AddEvent( string eventName )
		{
			// TODO - shall we allow passing data with the event?

			if ( Events == null ) Events = new HashSet<string>();

			if ( Events.Contains( eventName ) )
				return;

			Events.Add( eventName );
		}


		/// <summary>
		/// </summary>
		public void SetTag( string tagName )
		{
			// TODO - shall we allow passing data with the event?

			Tags ??= new HashSet<string>();

			if ( Tags.Contains( tagName ) )
				return;

			Tags.Add( tagName );
		}

		/// <summary>
		/// Allow the controller to tweak input. Empty by default.
		/// </summary>
		public virtual void BuildInput()
		{

		}

		public void Simulate( IClient client, Entity pawn )
		{
			Events?.Clear();
			Tags?.Clear();

			UpdateFromEntity();

			Simulate();

			UpdateEntity();
		}
		
		public void FrameSimulate( IClient client, Entity pawn )
		{
			UpdateFromEntity();

			FrameSimulate();

			UpdateEntity();
		}
	}
}
