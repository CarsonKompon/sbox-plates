﻿using Sandbox;
using Sandbox.UI;

/* 
 * Weapon base for sniper based zooming
*/

namespace SWB_Base
{
    public partial class WeaponBaseSniper : WeaponBase
    {
        public virtual string LensTexture => "/swb_base/textures/scopes/swb_lens_hunter.png"; // Path to the lens texture
        public virtual string ScopeTexture => "/swb_base/textures/scopes/swb_scope_hunter.png"; // Path to the scope texture
        public virtual string ZoomInSound => "swb_sniper.zoom_in"; // Sound to play when zooming in
        public virtual string ZoomOutSound => ""; // Sound to play when zooming out
        public virtual float ZoomAmount => 20f; // The amount to zoom in ( lower is more )
        public virtual bool UseRenderTarget => false; // EXPERIMENTAL - Use a render target instead of a full screen texture zoom

        private Panel SniperScopePanel;
        private bool switchBackToThirdP = false;
        private float lerpZoomAmount = 0;

        public override void ActiveStart( Entity ent )
        {
            base.ActiveStart( ent );
        }

        public override void ActiveEnd( Entity ent, bool dropped )
        {
            base.ActiveEnd( ent, dropped );

            SniperScopePanel?.Delete();
        }

        public virtual void OnScopedStart()
        {
            Primary.Spread /= 1000;

            var owner = Client;
            if ( owner.Camera is ThirdPersonCamera )
            {
                switchBackToThirdP = true;
                owner.Camera = new FirstPersonCamera();
            }

            if ( IsLocalPawn )
            {
                ViewModelEntity.RenderColor = ViewModelEntity.RenderColor.WithAlpha(0f);

                if ( !string.IsNullOrEmpty( ZoomInSound ) )
                    PlaySound( ZoomInSound );
            }
        }

        public virtual void OnScopedEnd()
        {
            Primary.Spread *= 1000;
            lerpZoomAmount = 0;

            var owner = Client;
            if ( switchBackToThirdP )
            {
                switchBackToThirdP = false;
                owner.Camera = new ThirdPersonCamera();
            }

            if ( IsLocalPawn && !(owner.Camera is ThirdPersonCamera) )
            {
                ViewModelEntity.RenderColor = ViewModelEntity.RenderColor.WithAlpha(1f);

                if ( !string.IsNullOrEmpty( ZoomOutSound ) )
                    PlaySound( ZoomOutSound );
            }
        }

        public override void Simulate( Client owner )
        {
            base.Simulate( owner );

            if ( Input.Pressed( InputButton.Attack2 ) && !IsReloading )
            {
                OnScopedStart();
            }

            if ( Input.Released( InputButton.Attack2 ) )
            {
                OnScopedEnd();
            }
        }
        public override void CreateHudElements()
        {
            base.CreateHudElements();

            if ( Local.Hud == null ) return;

            if ( UseRenderTarget )
            {
                SniperScopePanel = new SniperScopeRT( LensTexture, ScopeTexture );
                SniperScopePanel.Parent = Local.Hud;
            }
            else
            {
                SniperScopePanel = new SniperScope( LensTexture, ScopeTexture );
                SniperScopePanel.Parent = Local.Hud;
            }
        }

        public override void PostCameraSetup( ref CameraSetup camSetup )
        {
            base.PostCameraSetup( ref camSetup );

            if ( IsZooming )
            {
                if ( lerpZoomAmount == 0 )
                    lerpZoomAmount = camSetup.FieldOfView;

                lerpZoomAmount = MathUtil.FILerp( lerpZoomAmount, ZoomAmount, 10f );
                camSetup.FieldOfView = lerpZoomAmount;
            }
        }
    }
}
