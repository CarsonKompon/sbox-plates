using Sandbox.UI.Construct;


namespace Sandbox.UI
{
	public partial class PlatesChatEntry : Panel
	{
		public Label NameLabel { get; internal set; }
		public Label Message { get; internal set; }
		public Image Avatar { get; internal set; }

		public RealTimeSince TimeSinceBorn = 0;

		public PlatesChatEntry()
		{
			Avatar = Add.Image();
			NameLabel = Add.Label( "Name", "name" );
			Message = Add.Label( "Message", "message" );
		}

		public override void Tick() 
		{
			base.Tick();

			if ( TimeSinceBorn > 12 ) 
			{ 
				Delete();
			}
		}
	}
}