#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.OdinInspector.Editor.ValueResolvers;
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

		private static IEnumerable<ValueDropdownItem<Type>> GetNamedValidTypes( Type baseType, params string[] trims )
		{
			return SubAssetTypeCache.GetTypes( baseType )
			.Select( type =>
			{
				var typeName = type.FullName;
				if ( trims != null )
				{
					foreach ( var trim in trims )
						typeName = new Regex( $@"^{trim}|{trim}$" ).Replace( typeName, "" );
				}

				typeName = typeName.Replace( '.', '/' );
				typeName = ObjectNames.NicifyVariableName( typeName );
				return new ValueDropdownItem<Type>( typeName, type );
			} );
		}

		public static void ShowSelectorSimple<T>( Rect rect, InspectorProperty property, bool askForName = true, bool enableSingleClick = false, bool showNone = false, bool createIfSingle = false, string trim = null ) where T : ScriptableObject
		{
			if ( string.IsNullOrEmpty( trim ) )
			{
				ShowSelector<T>( rect, property, askForName, enableSingleClick, showNone, createIfSingle, ( string[])null );
			}
			else
			{
				var trimResolver = ValueResolver.Get<IEnumerable<string>>( property, trim, new string[] { trim } );
				if ( trimResolver.HasError )
				{
					Debug.LogError( trimResolver.ErrorMessage );
					ShowSelector<T>( rect, property, askForName, enableSingleClick, showNone, createIfSingle, (string[])null );
				}
				else
				{
					ShowSelector<T>( rect, property, askForName, enableSingleClick, showNone, createIfSingle, trimResolver.GetValue().ToArray() );
				}
			}
		}

		public static void ShowSelector<T>( Rect rect, InspectorProperty property, bool askForName = true, bool enableSingleClick = false, bool showNone = false, bool createIfSingle = false, params string[] trims ) where T : ScriptableObject
		{
			//ShowNone
			var items = GetNamedValidTypes( typeof( T ), trims ).ToList();

			if ( createIfSingle )
			{
				if ( items.Count == 0 ) // Add empty
				{
					AddToAsset( null, null, property );
					return;
				}
				else if ( items.Count == 1 ) // Add only option
				{
					var type = items[0].Value;
					if ( askForName )
						EditorApplication.delayCall += () => Rename( type, property );
					else
						AddToAsset( $"New {type.Name}", type, property );

					return;
				}
			}

			if ( showNone )
				items.Insert( 0, new ValueDropdownItem<Type>( "None", null ) );

			var gs = new GenericSelector<ValueDropdownItem<Type>>( items );
			if ( enableSingleClick )
				gs.EnableSingleClickToSelect();

			if ( askForName )
			{
				gs.SelectionConfirmed += results =>
				{
					var type = results.First().Value;
					if ( type == null )
						AddToAsset( null, null, property );
					else
						EditorApplication.delayCall += () => Rename( type, property );
				};
			}
			else
			{
				gs.SelectionConfirmed += results =>
				{
					var type = results.First().Value;
					if ( type == null )
						AddToAsset( null, null, property );
					else
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
				if ( type == null )
				{
					( property.ChildResolver as ICollectionResolver ).QueueAdd( null, i );
				}
				else
				{
					var so = ScriptableObject.CreateInstance( type );
					so.name = name;

					var root = (Object)property.SerializationRoot.ValueEntry.WeakValues[i];
					AssetDatabase.AddObjectToAsset( so, root );
					AssetDatabase.SaveAssets();

					( property.ChildResolver as ICollectionResolver ).QueueAdd( so, i );
				}
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