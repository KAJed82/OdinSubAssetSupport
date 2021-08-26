using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEngine;

namespace SubAssets.Editor
{
	public class SubAssetListAttributeProcessor<TList, T> : OdinAttributeProcessor<TList>
		where TList : IList<T>
		where T : ScriptableObject
	{
		public override bool CanProcessSelfAttributes( InspectorProperty property )
		{
			return property.GetAttribute<SubAssetListAttribute>() != null;
		}

		public override void ProcessSelfAttributes( InspectorProperty property, List<Attribute> attributes )
		{
			var subAssetListAttribute = property.GetAttribute<SubAssetListAttribute>();
			var listDrawerSettings = attributes.GetAttribute<ListDrawerSettingsAttribute>();
			if ( listDrawerSettings != null )
			{
				if ( !string.IsNullOrEmpty( listDrawerSettings.CustomAddFunction ) )
				{
					Debug.LogError( "Cannot mix SubAssetListAttribute with another CustomAddFunction" );
					return;
				}

				if ( subAssetListAttribute.DeleteOnRemove && ( !string.IsNullOrEmpty( listDrawerSettings.CustomRemoveElementFunction ) || !string.IsNullOrEmpty( listDrawerSettings.CustomRemoveIndexFunction ) ) )
				{
					Debug.LogError( "Cannot mix SubAssetListAttribute with another CustomRemoveElementFunction" );
					return;
				}
			}

			if ( listDrawerSettings == null )
				attributes.Add( listDrawerSettings = new ListDrawerSettingsAttribute() );

			//listDrawerSettings.CustomAddFunction = $"@SubAssets.Editor.SubAssetListHelper.ShowSelector<{typeof( T ).Name}>( $rect, $property, {( subAssetListAttribute.AskForName ? "true" : "false" )}, {( subAssetListAttribute.EnableSingleClick ? "true" : "false" )}, {( string.IsNullOrEmpty( subAssetListAttribute.Trim ) ? "null" : subAssetListAttribute.Trim )} )";
			listDrawerSettings.CustomAddFunction = $"@SubAssets.Editor.SubAssetListHelper.ShowSelector<{typeof( T ).Name}>( $rect, $property, {( subAssetListAttribute.AskForName ? "true" : "false" )}, {( subAssetListAttribute.EnableSingleClick ? "true" : "false" )}, {( string.IsNullOrEmpty( subAssetListAttribute.Trim ) ? "null" : $"\"{subAssetListAttribute.Trim}\"" )} )";

			if ( subAssetListAttribute.DeleteOnRemove )
				listDrawerSettings.CustomRemoveIndexFunction = $"@SubAssets.Editor.SubAssetListHelper.RemoveElement<{typeof( TList ).ToGenericTypeString()},{typeof( T ).Name}>( $property, $index, {( subAssetListAttribute.ConfirmDelete ? "true" : "false" )} )";
		}
	}
}