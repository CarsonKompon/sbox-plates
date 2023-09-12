using System;
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace Plates;

public partial class HeaderText : Panel
{
	public Label headerLabel;
    public Label subheaderLabel;
	public static string headerText = "";
    public static string subheaderText = "";

	public HeaderText(){
		headerLabel = Add.Label("", "header");
        subheaderLabel = Add.Label("", "subheader");
	}

	public override void Tick(){
		headerLabel.Text = $"{headerText}";
		subheaderLabel.Text = $"{subheaderText}";
	}

	[ClientRpc]
	public static void updateText(string _header = null, string _subheader = null){
		if(_header == null) _header = headerText;
		if(_subheader == null) _subheader = subheaderText;
        headerText = _header;
        subheaderText = _subheader;
	}
}