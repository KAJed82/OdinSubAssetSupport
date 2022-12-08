using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using Object = UnityEngine.Object;

namespace SubAssets.Editor
{
	public class ShowSubObjectDeleteAttributePropertyProcessor<T> : OdinPropertyProcessor<T, ShowSubObjectDeleteAttribute>
	where T : Object
	{
		public override bool CanProcessForProperty( InspectorProperty property )
		{
			for ( int i = 0; i < property.ValueEntry.ValueCount; ++i )
			{
				T asset = property.ValueEntry.WeakValues[i] as T;
				if ( asset != null && ( !EditorUtility.IsPersistent( asset ) || !AssetDatabase.IsSubAsset( asset ) ) )
					return false;
			}

			return true;
		}

		public override void ProcessMemberProperties( List<InspectorPropertyInfo> infos )
		{
			var attribute = Property.GetAttribute<ShowSubObjectDeleteAttribute>();
			var attributes = attribute.ShowInInlineEditors
				? new Attribute[] { new DelayedPropertyAttribute(), new PropertyOrderAttribute( float.NegativeInfinity ) }
				: new Attribute[] { new DelayedPropertyAttribute(), new PropertyOrderAttribute( float.NegativeInfinity ), new HideInInlineEditorsAttribute() };

			infos.AddDelegate(
				"deleteSubObject",
				() =>
				{
					if ( !EditorUtility.DisplayDialog( "Delete", "Delete sub object?", "Ok", "Cancel" ) )
						return;

					for ( int i = 0; i < Property.ValueEntry.ValueCount; ++i )
					{
						T asset = Property.ValueEntry.WeakValues[i] as T;
						if ( asset != null )
							Object.DestroyImmediate( asset, true );
					}

					AssetDatabase.SaveAssets();
				},
				attributes
			);
		}
	}
}