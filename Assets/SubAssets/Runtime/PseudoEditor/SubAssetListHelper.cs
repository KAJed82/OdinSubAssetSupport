#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SubAssets.Editor
{
	public static class SubAssetListHelper
	{
		public static string ToGenericTypeString( this Type t )
		{
			if ( !t.IsGenericType )
				return t.Name;

			string genericTypeName = t.GetGenericTypeDefinition().Name;
			genericTypeName = genericTypeName.Substring( 0, genericTypeName.IndexOf( '`' ) );
			string genericArgs = string.Join( ",", t.GetGenericArguments().Select( ta => ToGenericTypeString( ta ) ).ToArray() );
			return genericTypeName + "<" + genericArgs + ">";

		}

		public static void ShowSelector<T>( Rect rect, InspectorProperty property, bool askForName = true, bool enableSingleClick = false ) where T : ScriptableObject
		{
			var gs = new GenericSelector<Type>(
				SubAssetTypeCache.GetTypes( typeof( T ) )
			);
			if ( enableSingleClick )
				gs.EnableSingleClickToSelect();

			if ( askForName )
			{
				gs.SelectionConfirmed += results => EditorApplication.delayCall += () => Rename( results.First(), property );
			}
			else
			{
				gs.SelectionConfirmed += results =>
				{
					var type = results.First();
					AddToAsset( $"New {type.Name}", type, property );
				};
			}

			gs.ShowInPopup( rect );
		}

		private static void Rename( Type type, InspectorProperty property )
		{
			TextWindow.Open( "New SubAsset Name", $"New {type.Name}", name => AddToAsset( name, type, property ) );
		}

		private static void AddToAsset( string name, Type type, InspectorProperty property )
		{
			for ( int i = 0; i < property.ValueEntry.ValueCount; ++i )
			{
				var so = ScriptableObject.CreateInstance( type );
				so.name = name;

				var root = (Object)property.SerializationRoot.ValueEntry.WeakValues[i];
				AssetDatabase.AddObjectToAsset( so, root );
				AssetDatabase.SaveAssets();

				( property.ChildResolver as ICollectionResolver ).QueueAdd( so, i );
			}
		}

		public static void RemoveElement<TList, T>( InspectorProperty property, int index, bool askFirst = true )
			where TList : IList<T>
			where T : ScriptableObject
		{
			var element = property.Children[$"${index}"];
			for ( int i = 0; i < property.ValueEntry.ValueCount; ++i )
			{
				var elementValue = element.ValueEntry.WeakValues[i] as Object;
				if ( elementValue == null )
				{
					( property.ChildResolver as BaseOrderedCollectionResolver<TList> ).QueueRemoveAt( index, i );
				}
				else
				{
					if ( !AssetDatabase.IsSubAsset( elementValue ) )
					{
						( property.ChildResolver as BaseOrderedCollectionResolver<TList> ).QueueRemoveAt( index, i );
						continue;
					}

					if ( askFirst )
					{
						int result = EditorUtility.DisplayDialogComplex( "Are you sure?", $"Removing / Deleting this object cannot be undone.\n{elementValue}", "Yes", "No", "Cancel" );
						switch ( result )
						{
							case 0:
								break;

							case 1:
								continue;

							case 2:
								return;
						}
					}

					( property.ChildResolver as ICollectionResolver ).QueueRemove( elementValue, i );
					Object.DestroyImmediate( elementValue, true );

					AssetDatabase.SaveAssets();
				}
			}
		}
	}
}
#endif