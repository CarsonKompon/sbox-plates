using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;

public partial class VitalInfo : Panel
{
	public Panel VitalsPanel;
	public Panel InfoPanel;
	private Panel HealthBarContainer;
	private Panel HealthBar;

	public Label HP;
	public Label Money;
	public Label Info;
	public Label Disclaimer;
	
	public VitalInfo(){
		Disclaimer = Add.Label("Plates of Fate is currently in a WIP state. Please report bugs on the github issues page.", "disclaimer");

		VitalsPanel = Add.Panel("vitals");
		InfoPanel = Add.Panel("info");
		HealthBarContainer = Add.Panel("healthbar");
		HealthBar = HealthBarContainer.Add.Panel("bar");
		HP = VitalsPanel.Add.Label("HP: 100", "hp");
		Money = VitalsPanel.Add.Label("$10,000", "money");
		Info = InfoPanel.Add.Label("Speed: 16\nJump: 50\nPlate Size: 92\"", "value");
		RefreshAvatar();
	}

	public override void Tick(){
		var player = Local.Pawn as PlatesPlayer;

		var plateSize = "N/A";
		var pl = player.CurrentPlate;
		if(pl.IsValid()) plateSize = MathF.Ceiling(pl.toScale.x*92) + "\""; 
		InfoPanel.SetClass("open", player.InGame);

		if(player.Controller is PlatesWalkController){
			var walkController = ((player as PlatesPlayer).Controller as PlatesWalkController);
			if(player == null) return;

			HealthBar.Style.Width = Length.Percent(player.Health);

			HP.Text = $"HP: {player.Health.CeilToInt()}";
			Money.Text = "$" + String.Format("{0:n0}", PlatesPlayerData.GetLocalData().Money);
			Info.Text = $"Speed: {String.Format("{0:0.00}", walkController.Speed)}x\nJump: {String.Format("{0:0.00}", walkController.JumpPower)}x\nPlate Size: {plateSize}";
		}

		TickAvatar();
		/*
		else if(player.Controller is MarbleController){
			var walkController = ((player as PlatesPlayer).Controller as MarbleController);
			if(player == null) return;

			HP.Text = $"HP: {player.Health.CeilToInt()}";
			Info.Text = $"Speed: 1.00x\nJump: 0.00x\nPlate Size: {plateSize}";
		}
		*/
	}
}