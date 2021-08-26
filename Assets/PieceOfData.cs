using System.Collections.Generic;
using Sirenix.OdinInspector;
using SubAssets;
using UnityEngine;

[ShowSubObjectDelete]
[CreateAssetMenu]
public class PieceOfData : ScriptableObject
{
	public int id;

	[CreateSubAsset]
	[InlineEditor]
	public PieceOfData child;

	[SubAssetList( DeleteOnRemove = true, ConfirmDelete = true )]
	public List<PieceOfData> children = new List<PieceOfData>();
}
