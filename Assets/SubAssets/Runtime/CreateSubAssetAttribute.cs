using System;

namespace SubAssets
{
	[AttributeUsage( AttributeTargets.Class | AttributeTargets.Field | AttributeTargets.Property )]
	public class CreateSubAssetAttribute : Attribute
	{
		public string Trim { get; private set; }

		public CreateSubAssetAttribute()
		{
			Trim = null;
		}

		/// <summary>
		/// </summary>
		/// <param name="trim">Function to call for strings that will be trimmed (ValueResolver)
		public CreateSubAssetAttribute( string trim )
		{
			Trim = trim;
		}
	}
}