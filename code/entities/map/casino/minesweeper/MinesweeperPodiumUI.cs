using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;



public partial class MinesweeperPodiumUI : WorldPanel
{
	Panel uiCard { get; set; }
	public MinesweeperPodium podium;

	public MinesweeperPodiumUI()
	{
	}
	public MinesweeperPodiumUI( float scale, MinesweeperPodium podi ) : this()
	{
		podium = podi;

		// UI setup
		uiCard = Add.Panel( "ui-card" );
		uiCard.Add.Label( "Plate Sweeper", "title" );
		StyleSheet.Load( "/entities/map/casino/minesweeper/MinesweeperPodiumUI.scss" );
		AddClass( "minesweeper-podium-ui" );
		var width = 40 * 7 * scale;
		var height = 30 * 7 * scale;
		PanelBounds = new Rect( -width * .5f, -height * .5f, width, height );
		Rotation = Rotation + (Rotation.FromPitch( -100 ));

	}

	// }
	public override void Tick()
	{
		SetClass( "show-ui", Local.Client.Pawn.Position.Distance( podium.Position ) < podium.screen.activeRadius );
	}




}