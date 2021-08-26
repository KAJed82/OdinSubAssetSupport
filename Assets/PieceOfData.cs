using System.Collections.Generic;
using Sirenix.OdinInspector;
using SubAssets;
using UnityEngine;

namespace Root.Game
{
	[ShowSubObjectDelete]
	[CreateAssetMenu]
	public class PieceOfData : ScriptableObject
	{
		public int id;

		[CreateSubAsset( nameof( GetTrimString ) )]
		[InlineEditor]
		public PieceOfData child;

		[CreateSubAsset( nameof( GetTrimString ) )]
		[SubAssetList( DeleteOnRemove = true, ConfirmDelete = true, Trim = nameof( GetTrimString ) )]
		public List<PieceOfData> children = new List<PieceOfData>();

		public IEnumerable<string> GetTrimString()
		{
			yield return "Root.";
			yield return "Game.";
		}
	}
}