using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

public class Logo : Panel
{
	public Label Label;

	public Logo(){
		Label = Add.Label("Pl&tes - Docs available on github.com/cklidify/sbox-plates", "value");
	}
}