using System.Linq;
using Sirenix.OdinInspector.Editor;
using Sirenix.OdinInspector.Editor.ActionResolvers;
using UnityEngine;

[assembly: RegisterDefaultActionResolver( typeof( SubAssets.Editor.CollectionRectResolverCreateor ), 10000 )]

namespace SubAssets.Editor
{
	public class CollectionRectResolverCreateor : ActionResolverCreator
	{
		public override string GetPossibleMatchesString( ref ActionResolverContext context )
		{
			return null;
		}

		public override ResolvedAction TryCreateAction( ref ActionResolverContext context )
		{
			if ( !( context.Property.ChildResolver is ICollectionResolver ) )
				return null;

			context.NamedValues.Add(
				new NamedValue(
					"rect",
					typeof( Rect ),
					( ref ActionResolverContext c, int selectionIndex ) =>
					{
						var drawer = c.Property.GetActiveDrawerChain().BakedDrawerArray.FirstOrDefault( x => typeof( CollectionRectDrawer ).IsAssignableFrom( x.GetType() ) ) as CollectionRectDrawer;
						if ( drawer == null )
							return new Rect( 0, 0, 10, 10 );

						return new Rect( drawer.Rect ) { y = drawer.Rect.y + 20, width = drawer.Rect.width - 22 };
					}
				)
			);

			return null;
		}
	}
}