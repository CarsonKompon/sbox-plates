using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;

public class Vitals : Panel
{
	public Panel VitalsPanel;
	public Panel InfoPanel;

	public Label HP;
	public Label Info;
	public Vitals(){
		VitalsPanel = Add.Panel("vitals");
		InfoPanel = Add.Panel("info");
		HP = VitalsPanel.Add.Label("HP: 100", "value");
		Info = InfoPanel.Add.Label("Speed: 16\nJump: 50\nPlate Size: 92\"", "value");
	}

	public override void Tick(){
		var player = Local.Pawn as PlatesPlayer;

		var plateSize = "N/A";
		var pl = player.CurrentPlate;
		if(pl.IsValid()) plateSize = MathF.Ceiling(pl.toScale*92) + "\""; 

		if(player.Controller is PlatesWalkController){
			var walkController = ((player as PlatesPlayer).Controller as PlatesWalkController);
			if(player == null) return;

			HP.Text = $"HP: {player.Health.CeilToInt()}";
			Info.Text = $"Speed: {String.Format("{0:0.00}", walkController.Speed)}x\nJump: {String.Format("{0:0.00}", walkController.JumpPower)}x\nPlate Size: {plateSize}";
		}
		/*
		else if(player.Controller is MarbleController){
			var walkController = ((player as PlatesPlayer).Controller as MarbleController);
			if(player == null) return;

			HP.Text = $"HP: {player.Health.CeilToInt()}";
			Info.Text = $"Speed: 1.00x\nJump: 0.00x\nPlate Size: {plateSize}";
		}
		*/

		SetClass( "blur", (Local.Pawn as PlatesPlayer).BlurFX );
	}
}