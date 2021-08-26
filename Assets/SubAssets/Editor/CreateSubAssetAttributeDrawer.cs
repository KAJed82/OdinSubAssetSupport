using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SubAssets.Editor
{
	[DrawerPriority( 0, 1, 0 )]
	public class CreateSubAssetAttributeDrawer<T> : OdinAttributeDrawer<CreateSubAssetAttribute, T>
	 where T : Object
	{
		protected override bool CanDrawAttributeValueProperty( InspectorProperty property )
		{
			if ( property.ValueEntry.ValueCount > 1 )
				return false;

			if ( !typeof( Object ).IsAssignableFrom( property.SerializationRoot.ValueEntry.BaseValueType ) )
				return false;

			for ( int i = 0; i < property.SerializationRoot.ValueEntry.ValueCount; ++i )
			{
				var o = (Object)property.SerializationRoot.ValueEntry.WeakValues[i];
				if ( !EditorUtility.IsPersistent( o ) )
					return false;
			}

			return true;
		}

		private List<ValueDropdownItem<Type>> m_NamedValidTypes;
		private List<ValueDropdownItem<Type>> NamedValidTypes
		{
			get
			{
				if ( m_NamedValidTypes == null || m_NamedValidTypes.Count == 0 )
				{
					m_NamedValidTypes = SubAssetTypeCache.GetTypes( ValueEntry.BaseValueType )
					.Select( type =>
					{
						var typeName = type.FullName;
						if ( Attribute.Trim != null )
						{
							foreach ( var trim in Attribute.Trim )
								typeName = new Regex( $@"^{trim}|{trim}$" ).Replace( typeName, "" );
						}

						typeName = typeName.Replace( '.', '/' );
						typeName = ObjectNames.NicifyVariableName( typeName );
						return new ValueDropdownItem<Type>( typeName, type );
					} )
					.ToList();
				}

				return m_NamedValidTypes;
			}
		}

		protected override void DrawPropertyLayout( GUIContent label )
		{
			if ( ValueEntry.SmartValue != null )
			{
				CallNextDrawer( label );
				return;
			}

			GUILayout.Space( -2 );

			EditorGUILayout.BeginHorizontal();

			Rect lastRect;
			using ( var vg = new EditorGUILayout.VerticalScope() )
			{
				CallNextDrawer( label );

				lastRect = vg.rect;
			}

			if ( SirenixEditorGUI.IconButton( EditorIcons.File, tooltip: "Create as sub asset" ) )
			{
				var typeName = ValueEntry.BaseValueType.Name;
				if ( Attribute.Trim != null )
				{
					foreach ( var trim in Attribute.Trim )
						typeName = new Regex( $@"^{trim}|{trim}$" ).Replace( typeName, "" );
				}
				typeName = ObjectNames.NicifyVariableName( typeName );

				var gs = new GenericSelector<ValueDropdownItem<Type>>( $"Create {typeName}", NamedValidTypes );
				lastRect.position += new Vector2( 20, 0 );
				gs.ShowInPopup( lastRect );
				gs.SelectionConfirmed += results =>
				{
					if ( results != null && results.Any() )
					{
						var type = results.First().Value;
						var so = ScriptableObject.CreateInstance( type );
						so.name = $"New {type.Name}";

						var root = (Object)Property.SerializationRoot.ValueEntry.WeakSmartValue;
						AssetDatabase.AddObjectToAsset( so, root );
						AssetDatabase.SaveAssets();

						ValueEntry.WeakSmartValue = so;
					}
				};
			}

			EditorGUILayout.EndHorizontal();
		}
	}
}