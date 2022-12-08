using System.Linq;
using Sirenix.OdinInspector.Editor;
using Sirenix.OdinInspector.Editor.ActionResolvers;
using Sirenix.OdinInspector.Editor.ValueResolvers;
using UnityEngine;
using ActionNamedValue = Sirenix.OdinInspector.Editor.ActionResolvers.NamedValue;
using ValueNamedValue = Sirenix.OdinInspector.Editor.ValueResolvers.NamedValue;

[assembly: RegisterDefaultActionResolver( typeof( SubAssets.Editor.CollectionRectActionResolverCreateor ), 10000 )]
[assembly: RegisterDefaultValueResolverCreator( typeof( SubAssets.Editor.CollectionRectValueResolverCreateor ), 10000 )]

namespace SubAssets.Editor
{
	public class CollectionRectValueResolverCreateor : ValueResolverCreator
	{
		public override string GetPossibleMatchesString( ref ValueResolverContext context )
		{
			return null;
		}
		public override ValueResolverFunc<TResult> TryCreateResolverFunc<TResult>( ref ValueResolverContext context )
		{
			if ( !( context.Property.ChildResolver is ICollectionResolver ) )
				return null;

			context.NamedValues.Add(
				new ValueNamedValue(
					"rect",
					typeof( Rect ),
					( ref ValueResolverContext c, int selectionIndex ) =>
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

	public class CollectionRectActionResolverCreateor : ActionResolverCreator
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
				new ActionNamedValue(
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