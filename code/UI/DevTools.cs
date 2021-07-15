using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

public class DevTools : Panel
{
	public Label Label;

	public DevTools(){
		Label = Add.Label("DEV TOOLS", "value");
	}
}