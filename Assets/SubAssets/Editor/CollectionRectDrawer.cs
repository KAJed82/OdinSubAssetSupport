using Sirenix.OdinInspector.Editor;
using UnityEngine;

namespace SubAssets.Editor
{
	[DrawerPriority( 0, 0, 0.9001 )]
	public class CollectionRectDrawer : OdinDrawer
	{
		public override bool CanDrawProperty( InspectorProperty property )
		{
			return property.ChildResolver is ICollectionResolver;
		}

		internal Rect Rect { get; private set; }
		protected override void DrawPropertyLayout( GUIContent label )
		{
			Rect = GUILayoutUtility.GetRect( 0, 0, GUILayout.ExpandWidth( true ) );

			CallNextDrawer( label );
		}
	}
}