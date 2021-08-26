#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.Utilities;

namespace SubAssets.Editor
{
	public static class SubAssetTypeCache
	{
		private static Dictionary<Type, Type[]> s_Types = new Dictionary<Type, Type[]>();

		public static Type[] GetTypes( Type baseType )
		{
			if ( !s_Types.TryGetValue( baseType, out Type[] types ) )
			{
				s_Types[baseType] = types = AssemblyUtilities.GetTypes( AssemblyTypeFlags.All )
						.Where( x => baseType.IsAssignableFrom( x ) )
						.Where( x => !x.IsAbstract )
						.Where( x => !x.IsGenericTypeDefinition )
						.ToArray();
			}

			return types;
		}
	}
}
#endif