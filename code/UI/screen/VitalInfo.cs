using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;

namespace Plates;

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
	
	public VitalInfo()
	{
		Disclaimer = Add.Label("Plates of Fate is in an unfinished state. Please report bugs on the github issues page.", "disclaimer");

		VitalsPanel = Add.Panel("vitals");
		InfoPanel = Add.Panel("info");
		HealthBarContainer = Add.Panel("healthbar");
		HealthBar = HealthBarContainer.Add.Panel("bar");
		HP = VitalsPanel.Add.Label("HP: 100", "hp");
		Money = VitalsPanel.Add.Label("$10,000", "money");
		Info = InfoPanel.Add.Label("Speed: 16\nJump: 50\nPlate Size: 92\"", "value");
		//RefreshAvatar();
		var avatarPanel = Add.Panel("avatar");
		avatarPanel.AddChild<AvatarHud>();
	}

	public override void Tick()
	{
		if(Game.LocalPawn is Player player)
		{
			var plateSize = "N/A";
			if(player.CurrentPlate.IsValid())
			{
				plateSize = MathF.Ceiling(player.CurrentPlate.GetSize()*92) + "\"";
			}
			InfoPanel.SetClass("open", player.InGame);

			if(player.Controller is WalkController){
				var walkController = ((player as Player).Controller as WalkController);
				if(player == null) return;

				HealthBar.Style.Width = Length.Percent(player.Health);

				HP.Text = $"HP: {player.Health.CeilToInt()}";
				Money.Text = "$" + String.Format("{0:n0}", PlayerDataManager.LocalData.Money);
				Info.Text = $"Speed: {String.Format("{0:0.00}", walkController.Speed)}x\nJump: {String.Format("{0:0.00}", walkController.JumpPower)}x\nPlate Size: {plateSize}";
			}

			TickAvatar();
		}
	}
}