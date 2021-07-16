﻿using Sandbox;
using SWB_Base;
using System;
using System.Collections.Generic;

namespace SWB_CSS
{
	[Library( "swb_css_m249_he", Title = "HE Grenade" )]
	public class M249HE : WeaponBaseEntity
	{
		public override int Bucket => 4;
		public override HoldType HoldType => HoldType.Rifle;
		public override string ViewModelPath => "weapons/css_m249/css_v_mach_m249para.vmdl";
		public override string WorldModelPath => "weapons/css_m249/css_w_mach_m249para.vmdl";
		public override string Icon => "/swb_css/textures/ui/css_icon_m249he.png";
		public override int FOV => 75;
		public override int ZoomFOV => 40;
		public override float WalkAnimationSpeedMod => 0.7f;

		public override Func<ClipInfo, bool, FiredEntity> CreateEntity => CreateGrenadeEntity;
		public override string EntityModel => "weapons/css_grenade_he/css_w_grenade_he_thrown.vmdl";
		public override Vector3 EntityVelocity => new Vector3( 0, 0, 100 );
		public override Vector3 EntitySpawnOffset => new Vector3( 10, 10, 10 );
		public override float PrimaryEntitySpeed => 17;

		public M249HE()
		{
			Primary = new ClipInfo
			{
				Ammo = 100,
				AmmoType = AmmoType.Grenade,
				ClipSize = 100,
				ReloadTime = 5.7f,

				BulletSize = 5f,
				Damage = 15f,
				Force = 4f,
				Spread = 0.2f,
				Recoil = 0.7f,
				RPM = 800,
				FiringType = FiringType.auto,
				ScreenShake = new ScreenShake
				{
					Length = 0.5f,
					Speed = 4.0f,
					Size = 1.0f,
					Rotation = 0.5f
				},

				DryFireSound = "swb_lmg.empty",
				ShootSound = "css_m249.fire",

				BulletEjectParticle = "particles/pistol_ejectbrass.vpcf",
				MuzzleFlashParticle = "particles/pistol_muzzleflash.vpcf",

				InfiniteAmmo = InfiniteAmmoType.reserve
			};

			ZoomAnimData = new AngPos
			{
				Angle = new Angles( 1f, 0f, 0 ),
				Pos = new Vector3( -4.425f, 2.45f, 2 )
			};

			RunAnimData = new AngPos
			{
				Angle = new Angles( 10, 50, 0 ),
				Pos = new Vector3( 5, 0, 2 )
			};
		}

		private FiredEntity CreateGrenadeEntity( ClipInfo clipInfo, bool isPrimary )
		{
			var grenade = new Grenade();
			grenade.Weapon = this;
			grenade.ExplosionDelay = 3f;
			grenade.ExplosionRadius = 300f;
			grenade.ExplosionDamage = 200f;
			grenade.ExplosionForce = 350f;
			grenade.BounceSound = "css_grenade_he.bounce";
			grenade.ExplosionSounds = new List<string>
			{
				"css_grenade_he.explode"
			};
			grenade.ExplosionEffect = "weapons/css_grenade_he/particles/grenade_he_explosion.vpcf";
			grenade.ExplosionShake = new ScreenShake
			{
				Length = 1f,
				Speed = 5f,
				Size = 5f,
				Rotation = 2f,
			};

			return grenade;
		}
	}
}
