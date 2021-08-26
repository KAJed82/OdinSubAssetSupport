using System;

namespace SubAssets
{
	[AttributeUsage( AttributeTargets.Class )]
	public class ShowSubObjectDeleteAttribute : Attribute
	{
		public bool ShowInInlineEditors { get; private set; }

		/// <summary>
		/// </summary>
		/// <param name="showInInlineEditors">Show the Delete button in Inline Editors.</param>
		public ShowSubObjectDeleteAttribute( bool showInInlineEditors = true )
		{
			ShowInInlineEditors = showInInlineEditors;
		}
	}
}