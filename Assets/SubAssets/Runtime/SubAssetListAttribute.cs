namespace SubAssets
{
	public class SubAssetListAttribute : System.Attribute
	{
		public bool AskForName { get; set; }
		public bool EnableSingleClick { get; set; }

		public bool DeleteOnRemove { get; set; }
		public bool ConfirmDelete { get; set; }

		public string Trim { get; set; }

		public SubAssetListAttribute( bool askForName = true, bool enableSingleClick = false )
		{
			AskForName = askForName;
			EnableSingleClick = enableSingleClick;
		}
	}
}