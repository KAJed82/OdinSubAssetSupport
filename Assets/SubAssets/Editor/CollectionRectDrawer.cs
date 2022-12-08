using Sirenix.OdinInspector.Editor;
using UnityEngine;
using System.Linq;

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

	public static class CollectionRectHelperExtensions
	{
		public static Rect GetCollectionRect( this BakedDrawerChain bakedDrawerChain )
		{
			var drawer = bakedDrawerChain.BakedDrawerArray.FirstOrDefault( x => typeof( CollectionRectDrawer ).IsAssignableFrom( x.GetType() ) ) as CollectionRectDrawer;
			if ( drawer == null )
				return new Rect( 0, 0, 10, 10 );

			return new Rect( drawer.Rect ) { y = drawer.Rect.y + 20, width = drawer.Rect.width - 22 };
		}
	}
}
