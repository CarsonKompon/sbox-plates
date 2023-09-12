
using System;
using Sandbox;

namespace Plates
{
    [Library]
	public partial class WalkController : PawnController
    {
        [Net] public float Speed { get; set; } = 1.0f;
        [Net] public float JumpPower { get; set; } = 1;
        [Net] public float InputMultiplier { get; set; } = 1.0f;
        [Net] public float ForwardInput { get; set; } = 0.0f;
        [Net] public float SidewaysInput { get; set; } = 0.0f;
        [Net] public float Height { get; set; } = 1.0f;

        [Net] public float SprintSpeed { get; set; } = 400.0f;
        [Net] public float WalkSpeed { get; set; } = 80.0f;
        [Net] public float DefaultSpeed { get; set; } = 250.0f;
        [Net] public float Acceleration { get; set; } = 10.0f;
        [Net] public float AirAcceleration { get; set; } = 50.0f;
        [Net] public float GroundFriction { get; set; } = 4.0f;
        [Net] public float StopSpeed { get; set; } = 100.0f;
        [Net] public float GroundAngle { get; set; } = 46.0f;
        [Net] public float StepSize { get; set; } = 18.0f;
        [Net] public float MaxNonJumpVelocity { get; set; } = 140.0f;
        [Net] public float BodyGirth { get; set; } = 32.0f;
        [Net] public float BodyHeight { get; set; } = 72.0f;
        [Net] public float EyeHeight { get; set; } = 64.0f;
        [Net] public float Gravity { get; set; } = 800.0f;
        [Net] public float AirControl { get; set; } = 30.0f;
        public bool Swimming { get; set; } = false;
        [Net] public bool AutoJump { get; set; } = false;

        public Duck Duck;
        public Unstuck Unstuck;


        public WalkController()
        {
            Duck = new Duck(this);
            Unstuck = new Unstuck(this);
        }

        // Duck body height 32
        // Eye Height 64
        // Duck Eye Height 28

        protected Vector3 mins;
        protected Vector3 maxs;

        public virtual void SetBBox(Vector3 mins, Vector3 maxs)
        {
            if (this.mins == mins && this.maxs == maxs)
                return;

            this.mins = mins;
            this.maxs = maxs;
        }

        /// <summary>
        /// Update the size of the bbox. We should really trigger some shit if this changes.
        /// </summary>
        public virtual void UpdateBBox()
        {
            var girth = BodyGirth * 0.5f;

            var mins = new Vector3(-girth, -girth, 0) * Entity.Scale;
            var maxs = new Vector3(+girth, +girth, BodyHeight * Height) * Entity.Scale;

            Duck.UpdateBBox(ref mins, ref maxs, Entity.Scale);

            SetBBox(mins, maxs);
        }

        protected float SurfaceFriction;


        public override void FrameSimulate()
        {
            base.FrameSimulate();

			EyeRotation = Entity.ViewAngles.ToRotation();
		}

        public override void Simulate()
        {
			EyeLocalPosition = Vector3.Up * (EyeHeight * Entity.Scale);
            EyeLocalPosition *= Height;

			UpdateBBox();

			// If we're a bot, spin us around 180 degrees.
			if ( Entity.Client.IsBot )
				EyeRotation = Entity.ViewAngles.WithYaw( Entity.ViewAngles.yaw + 180f ).ToRotation();
			else
				EyeRotation = Entity.ViewAngles.ToRotation();

			RestoreGroundPos();

            if (Unstuck.TestAndFix())
                return;

            CheckLadder();
            Swimming = Entity.GetWaterLevel() > 0.5f;

            // Gravity - First Half
            if (!Swimming && !IsTouchingLadder)
            {
                Velocity -= new Vector3(0, 0, Gravity * 0.5f) * Time.Delta;
                Velocity += new Vector3(0, 0, BaseVelocity.z) * Time.Delta;

                BaseVelocity = BaseVelocity.WithZ(0);
            }

            if (AutoJump ? Input.Down("jump") : Input.Pressed("jump"))
            {
                CheckJumpButton();
            }

            bool bStartOnGround = GroundEntity != null;

            if (bStartOnGround)
            {
                Velocity = Velocity.WithZ(0);

                if (GroundEntity != null)
                {
                    ApplyFriction(GroundFriction * SurfaceFriction);
                }
            }

            // Wish Velocity
            WishVelocity = new Vector3( (Entity.InputDirection.x + ForwardInput).Clamp( -1f, 1f ), (Entity.InputDirection.y + SidewaysInput).Clamp( -1f, 1f ), 0) * InputMultiplier;
            var inSpeed = WishVelocity.Length.Clamp(0, 1);
            WishVelocity *= Entity.ViewAngles.WithPitch(0).ToRotation();

            if (!Swimming && !IsTouchingLadder)
            {
                WishVelocity = WishVelocity.WithZ(0);
            }

            WishVelocity = WishVelocity.Normal * inSpeed;
            WishVelocity *= GetWishSpeed();

            Duck.PreTick();

            bool bStayOnGround = false;
            if (Swimming)
            {
                ApplyFriction(1);
                WaterMove();
            }
            else if (IsTouchingLadder)
            {
                SetTag("climbing");
                LadderMove();
            }
            else if (GroundEntity != null)
            {
                bStayOnGround = true;
                WalkMove();
            }
            else
            {
                AirMove();
            }

            CategorizePosition(bStayOnGround);

            // Gravity - Second Half
            if (!Swimming && !IsTouchingLadder)
            {
                Velocity -= new Vector3(0, 0, Gravity * 0.5f) * Time.Delta;
            }

            if (GroundEntity != null)
            {
                Velocity = Velocity.WithZ(0);
            }

            SaveGroundPos();

        }

        public virtual float GetWishSpeed()
        {
            var ws = Duck.GetWishSpeed();
            if (ws >= 0) return ws * Speed;

            if (Input.Down("run")) return SprintSpeed * Speed;
            if (Input.Down("walk")) return WalkSpeed * Speed;

            return DefaultSpeed * Speed;
        }

        public virtual void WalkMove()
        {
            var wishdir = WishVelocity.Normal;
            var wishspeed = WishVelocity.Length;

            WishVelocity = WishVelocity.WithZ(0);
            WishVelocity = WishVelocity.Normal * wishspeed;

            Velocity = Velocity.WithZ(0);
            Accelerate(wishdir, wishspeed, 0, Acceleration);
            Velocity = Velocity.WithZ(0);

            //   Player.SetAnimParam( "forward", Input.Forward );
            //   Player.SetAnimParam( "sideward", Input.Right );
            //   Player.SetAnimParam( "wishspeed", wishspeed );
            //    Player.SetAnimParam( "walkspeed_scale", 2.0f / 190.0f );
            //   Player.SetAnimParam( "runspeed_scale", 2.0f / 320.0f );

            //  DebugOverlay.Text( 0, Pos + Vector3.Up * 100, $"forward: {Input.Forward}\nsideward: {Input.Right}" );

            // Add in any base velocity to the current velocity.
            Velocity += BaseVelocity;

            try
            {
                if (Velocity.Length < 1.0f)
                {
                    Velocity = Vector3.Zero;
                    return;
                }

                // first try just moving to the destination
                var dest = (Position + Velocity * Time.Delta).WithZ(Position.z);

                var pm = TraceBBox(Position, dest);

                if (pm.Fraction == 1)
                {
                    Position = pm.EndPosition;
                    StayOnGround();
                    return;
                }

                StepMove();
            }
            finally
            {

                // Now pull the base velocity back out.   Base velocity is set if you are on a moving object, like a conveyor (or maybe another monster?)
                Velocity -= BaseVelocity;
            }

			StayOnGround();

			Velocity = Velocity.Normal * MathF.Min( Velocity.Length, GetWishSpeed() );
		}

        public virtual void StepMove()
        {
            MoveHelper mover = new MoveHelper(Position, Velocity);
            mover.Trace = mover.Trace.Size(mins, maxs).Ignore(Entity);
            mover.MaxStandableAngle = GroundAngle;

            mover.TryMoveWithStep(Time.Delta, StepSize);

            Position = mover.Position;
            Velocity = mover.Velocity;
        }

        public virtual void Move()
        {
            MoveHelper mover = new MoveHelper(Position, Velocity);
            mover.Trace = mover.Trace.Size(mins, maxs).Ignore(Entity);
            mover.MaxStandableAngle = GroundAngle;

            mover.TryMove(Time.Delta);

            Position = mover.Position;
            Velocity = mover.Velocity;
        }

        /// <summary>
        /// Add our wish direction and speed onto our velocity
        /// </summary>
        public virtual void Accelerate(Vector3 wishdir, float wishspeed, float speedLimit, float acceleration)
        {
            // This gets overridden because some games (CSPort) want to allow dead (observer) players
            // to be able to move around.
            // if ( !CanAccelerate() )
            //     return;

            if (speedLimit > 0 && wishspeed > speedLimit)
                wishspeed = speedLimit;

            // See if we are changing direction a bit
            var currentspeed = Velocity.Dot(wishdir);

            // Reduce wishspeed by the amount of veer.
            var addspeed = wishspeed - currentspeed;

            // If not going to add any speed, done.
            if (addspeed <= 0)
                return;

            // Determine amount of acceleration.
            var accelspeed = acceleration * Time.Delta * wishspeed * SurfaceFriction;

            // Cap at addspeed
            if (accelspeed > addspeed)
                accelspeed = addspeed;

            Velocity += wishdir * accelspeed;
        }

        /// <summary>
        /// Remove ground friction from velocity
        /// </summary>
        public virtual void ApplyFriction(float frictionAmount = 1.0f)
        {
            // If we are in water jump cycle, don't apply friction
            //if ( player->m_flWaterJumpTime )
            //   return;

            // Not on ground - no friction


            // Calculate speed
            var speed = Velocity.Length;
            if (speed < 0.1f) return;

            // Bleed off some speed, but if we have less than the bleed
            //  threshold, bleed the threshold amount.
            float control = (speed < StopSpeed) ? StopSpeed : speed;

            // Add the amount to the drop amount.
            var drop = control * Time.Delta * frictionAmount;

            // scale the velocity
            float newspeed = speed - drop;
            if (newspeed < 0) newspeed = 0;

            if (newspeed != speed)
            {
                newspeed /= speed;
                Velocity *= newspeed;
            }

            // mv->m_outWishVel -= (1.f-newspeed) * mv->m_vecVelocity;
        }

        public virtual void CheckJumpButton()
        {
            // If we are in the water most of the way...
            if (Swimming)
            {
                ClearGroundEntity();

                Velocity = Velocity.WithZ(100);

                return;
            }

            if (GroundEntity == null)
                return;

            ClearGroundEntity();
            float flGroundFactor = 1.0f;
            float flMul = 268.3281572999747f * 1.2f * JumpPower;
            float startz = Velocity.z;

            if (Duck.IsActive)
                flMul *= 0.8f;

            Velocity = Velocity.WithZ(startz + flMul * flGroundFactor);

            Velocity -= new Vector3(0, 0, Gravity * 0.5f) * Time.Delta;

            AddEvent("jump");

        }

        public virtual BBox GetBBox(float crouch = -1f, float liftFeet = -1f)
	{
		if(crouch == -1f) crouch = 1;
		if(liftFeet == -1f) liftFeet = 1;

		var girth = BodyGirth * 0.5f;
		var height = BodyHeight * Height;
		var mins = new Vector3(-girth, -girth, 0) * Entity.Scale;
		var maxs = new Vector3(+girth, +girth, height) * Entity.Scale;

		if( crouch > 0f )
		{
			maxs.WithZ( 36 * Entity.Scale * Height);
		}
		if ( liftFeet > 0 )
		{
			maxs = maxs.WithZ( maxs.z - liftFeet );
		}

		return new BBox(mins, maxs);
	}

	/// <summary>
	/// Traces the bbox and returns the trace result.
	/// LiftFeet will move the start position up by this amount, while keeping the top of the bbox at the same 
	/// position. This is good when tracing down because you won't be tracing through the ceiling above.
	/// </summary>
	public virtual TraceResult TraceBBox( Vector3 start, Vector3 end, float crouch = -1f, float liftFeet = -1f )
	{
		if(crouch == -1f) crouch = 1;
		if(liftFeet == -1f) liftFeet = 1;

		var bounds = GetBBox(crouch, liftFeet);

		if ( liftFeet > 0 )
		{
			start += Vector3.Up * liftFeet;
		}

		var tr = Trace.Ray( start, end )
					.Size( bounds.Mins, bounds.Maxs )
					.WithAnyTags( "solid", "playerclip", "passbullets", "player" )
					.Ignore( Entity )
					.Run();
		return tr;
	}

        public virtual void AirMove()
        {
            var wishdir = WishVelocity.Normal;
            var wishspeed = WishVelocity.Length;

            Accelerate(wishdir, wishspeed, AirControl, AirAcceleration);

            Velocity += BaseVelocity;

            Move();

            Velocity -= BaseVelocity;
        }

        public virtual void WaterMove()
        {
            var wishdir = WishVelocity.Normal;
            var wishspeed = WishVelocity.Length;

            wishspeed *= 0.8f;

            Accelerate(wishdir, wishspeed, 100, Acceleration);

            Velocity += BaseVelocity;

            Move();

            Velocity -= BaseVelocity;
        }

        protected bool IsTouchingLadder = false;
        protected Vector3 LadderNormal;

        public virtual void CheckLadder()
        {
            var wishvel = new Vector3( Entity.InputDirection.x.Clamp( -1f, 1f ), Entity.InputDirection.y.Clamp( -1f, 1f ), 0);
            wishvel *= Entity.ViewAngles.WithPitch(0).ToRotation();
            wishvel = wishvel.Normal;

            if (IsTouchingLadder)
            {
                if (Input.Pressed("jump"))
                {
                    Velocity = LadderNormal * 100.0f;
                    IsTouchingLadder = false;

                    return;

                }
                else if (GroundEntity != null && LadderNormal.Dot(wishvel) > 0)
                {
                    IsTouchingLadder = false;

                    return;
                }
            }

            const float ladderDistance = 1.0f;
            var start = Position;
            Vector3 end = start + (IsTouchingLadder ? (LadderNormal * -1.0f) : wishvel) * ladderDistance;

            var pm = Trace.Ray(start, end)
                        .Size(mins, maxs)
                        .WithTag("ladder")
                        .Ignore(Entity)
                        .Run();

            IsTouchingLadder = false;

            if (pm.Hit)
            {
                IsTouchingLadder = true;
                LadderNormal = pm.Normal;
            }
        }

        public virtual void LadderMove()
        {
            var velocity = WishVelocity;
            float normalDot = velocity.Dot(LadderNormal);
            var cross = LadderNormal * normalDot;
            Velocity = (velocity - cross) + (-normalDot * LadderNormal.Cross(Vector3.Up.Cross(LadderNormal).Normal));

            Move();
        }


        public virtual void CategorizePosition(bool bStayOnGround)
        {
            SurfaceFriction = 1.0f;

            // Doing this before we move may introduce a potential latency in water detection, but
            // doing it after can get us stuck on the bottom in water if the amount we move up
            // is less than the 1 pixel 'threshold' we're about to snap to.	Also, we'll call
            // this several times per frame, so we really need to avoid sticking to the bottom of
            // water on each call, and the converse case will correct itself if called twice.
            //CheckWater();

            var point = Position - Vector3.Up * 2;
            var vBumpOrigin = Position;

            //
            //  Shooting up really fast.  Definitely not on ground trimed until ladder shit
            //
            bool bMovingUpRapidly = Velocity.z > MaxNonJumpVelocity;
            bool bMovingUp = Velocity.z > 0;

            bool bMoveToEndPos = false;

            if (GroundEntity != null) // and not underwater
            {
                bMoveToEndPos = true;
                point.z -= StepSize;
            }
            else if (bStayOnGround)
            {
                bMoveToEndPos = true;
                point.z -= StepSize;
            }

            if (bMovingUpRapidly || Swimming) // or ladder and moving up
            {
                ClearGroundEntity();
                return;
            }

            var pm = TraceBBox(vBumpOrigin, point, 4.0f);

            if (pm.Entity == null || Vector3.GetAngle(Vector3.Up, pm.Normal) > GroundAngle)
            {
                ClearGroundEntity();
                bMoveToEndPos = false;

                if (Velocity.z > 0)
                    SurfaceFriction = 0.25f;
            }
            else
            {
                UpdateGroundEntity(pm);
            }

            if (bMoveToEndPos && !pm.StartedSolid && pm.Fraction > 0.0f && pm.Fraction < 1.0f)
            {
                Position = pm.EndPosition;
            }

        }

        /// <summary>
        /// We have a new ground entity
        /// </summary>
        public virtual void UpdateGroundEntity(TraceResult tr)
        {
            GroundNormal = tr.Normal;

            // VALVE HACKHACK: Scale this to fudge the relationship between vphysics friction values and player friction values.
            // A value of 0.8f feels pretty normal for vphysics, whereas 1.0f is normal for players.
            // This scaling trivially makes them equivalent.  REVISIT if this affects low friction surfaces too much.
            SurfaceFriction = tr.Surface.Friction * 1.25f;
            if (SurfaceFriction > 1) SurfaceFriction = 1;

            //if ( tr.Entity == GroundEntity ) return;

            Vector3 oldGroundVelocity = default;
            if (GroundEntity != null) oldGroundVelocity = GroundEntity.Velocity;

            bool wasOffGround = GroundEntity == null;

            GroundEntity = tr.Entity;

            if (GroundEntity != null)
            {
                BaseVelocity = GroundEntity.Velocity;
            }
        }

        /// <summary>
        /// We're no longer on the ground, remove it
        /// </summary>
        public virtual void ClearGroundEntity()
        {
            if (GroundEntity == null) return;

            GroundEntity = null;
            GroundNormal = Vector3.Up;
            SurfaceFriction = 1.0f;
        }

        /// <summary>
        /// Try to keep a walking player on the ground when running down slopes etc
        /// </summary>
        public virtual void StayOnGround()
        {
            var start = Position + Vector3.Up * 2;
            var end = Position + Vector3.Down * StepSize;

            // See how far up we can go without getting stuck
            var trace = TraceBBox(Position, start);
            start = trace.EndPosition;

            // Now trace down from a known safe position
            trace = TraceBBox(start, end);

            if (trace.Fraction <= 0) return;
            if (trace.Fraction >= 1) return;
            if (trace.StartedSolid) return;
            if (Vector3.GetAngle(Vector3.Up, trace.Normal) > GroundAngle) return;

            // This is incredibly hacky. The real problem is that trace returning that strange value we can't network over.
            // float flDelta = fabs( mv->GetAbsOrigin().z - trace.m_vEndPos.z );
            // if ( flDelta > 0.5f * DIST_EPSILON )

            Position = trace.EndPosition;
        }

        protected void RestoreGroundPos()
        {
            if (GroundEntity == null || GroundEntity.IsWorld)
                return;

            //var Position = GroundEntity.Transform.ToWorld( GroundTransform );
            //Pos = Position.Position;
        }

        protected void SaveGroundPos()
        {
            if (GroundEntity == null || GroundEntity.IsWorld)
                return;

            //GroundTransform = GroundEntity.Transform.ToLocal( new Transform( Pos, Rot ) );
        }

    }
}
