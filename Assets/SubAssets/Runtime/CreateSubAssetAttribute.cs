using System;

namespace SubAssets
{
	[AttributeUsage( AttributeTargets.Class | AttributeTargets.Field | AttributeTargets.Property )]
	public class CreateSubAssetAttribute : Attribute
	{
		public string[] Trim { get; private set; }

		public CreateSubAssetAttribute()
		{
			Trim = null;
		}

		/// <summary>
		/// </summary>
		/// <param name="trim">Strings to trim off the start / end of the namespace qualified type name.</param>
		public CreateSubAssetAttribute( params string[] trim )
		{
			Trim = trim;
		}
	}
}