using Sandbox.UI;

namespace Plates;

public partial class Crosshair : Panel
{
	public static Crosshair Current;

	public Crosshair()
	{
		Current = this;
		StyleSheet.Load( "/ui/screen/crosshair.scss" );
	}
}