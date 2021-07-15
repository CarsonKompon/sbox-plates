using System;
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

public partial class EventHud : Panel
{
	public Label label;
	public static string txt = "";

	public EventHud(){
		label = Add.Label("100", "value");
	}

	public override void Tick(){
		label.Text = $"{txt}";
	}

	[ClientRpc]
	public static void updateText(string text){
		txt = text;
	}
}

public partial class EventSub : Panel
{
	public Label label;
	public static string txt = "";

	public EventSub(){
		label = Add.Label("100", "value");
	}

	public override void Tick(){
		label.Text = $"{txt}";
	}

	[ClientRpc]
	public static void updateText(string text){
		txt = text;
	}
}