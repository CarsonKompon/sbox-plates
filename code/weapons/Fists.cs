using Sandbox;
using System;

[Library("weapon_fists", Title = "Fists")]
	partial class Fists : Weapon
	{
	public override string ViewModelPath => "weapons/rust_flashlight/v_rust_flashlight.vmdl";
	public override float PrimaryRate => 1.0f;
	public override float SecondaryRate => 0.3f;
	//public override bool IsMelee => true;

	//public override int Bucket => 1;
	public virtual int MeleeDistance => 80;

	public override void Spawn()
	{
		base.Spawn();

		// TODO: EnableDrawing = false does not work.
		RenderAlpha = 0f;

		//SetModel("weapons/rust_boneknife/rust_boneknife.vmdl");
	}
	public virtual void MeleeStrike(float damage, float force)
	{
		var forward = Owner.EyeRot.Forward;
		forward = forward.Normal;
		var hit = true;
		foreach (var tr in TraceBullet(Owner.EyePos, Owner.EyePos + forward * MeleeDistance, 10f))
		{
			if (!tr.Entity.IsValid()){
				hit = false;
				continue;
			}
			tr.Surface.DoBulletImpact(tr);
			if (!IsServer) continue;
			using (Prediction.Off())
			{
				var damageInfo = DamageInfo.FromBullet(tr.EndPos, forward * 100 * force, damage)
					.UsingTraceResult(tr)
					.WithAttacker(Owner)
					.WithWeapon(this);
				tr.Entity.TakeDamage(damageInfo);
			}
		}
		if(hit) ViewModelEntity?.SetAnimBool( "attack_hit", true );
		else ViewModelEntity?.SetAnimBool( "attack", true );
		AnimEntity ply = Owner as AnimEntity;
		ply?.SetAnimBool("b_attack", true);
	}

	public override void AttackPrimary()
	{
		//ShootEffects();
		PlaySound("rust_boneknife.attack");
		MeleeStrike(1, 1.5f);

		if (IsLocalPawn)
		{
			new Sandbox.ScreenShake.Perlin();
		}

		ViewModelEntity?.SetAnimBool("fire", true);
		//CrosshairPanel?.OnEvent("fire");
	}

	public override void SimulateAnimator( PawnAnimator anim )
	{
		anim.SetParam( "holdtype", 4 ); // TODO this is shit
		anim.SetParam( "aimat_weight", 1.0f );
		anim.SetParam( "holdtype_post_hand", 0.07f );
	}


}