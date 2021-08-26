using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using Object = UnityEngine.Object;

namespace SubAssets.Editor
{
	public class ShowSubObjectNameAttributePropertyProcessor<T> : OdinPropertyProcessor<T, ShowSubObjectNameAttribute>
		where T : Object
	{
		public override void ProcessMemberProperties( List<InspectorPropertyInfo> infos )
		{
			var attribute = Property.GetAttribute<ShowSubObjectNameAttribute>();
			var attributes = attribute.ShowInInlineEditors
				? new Attribute[] { new DelayedPropertyAttribute(), new PropertyOrderAttribute( float.MinValue ) }
				: new Attribute[] { new DelayedPropertyAttribute(), new PropertyOrderAttribute( float.MinValue ), new HideInInlineEditorsAttribute() };

			infos.AddValue(
				"m_Name",
				( ref T asset ) => asset.name,
				( ref T asset, string name ) =>
				{
					asset.name = name;
					AssetDatabase.SaveAssets();
				},
				attributes
			);
		}
	}
}
