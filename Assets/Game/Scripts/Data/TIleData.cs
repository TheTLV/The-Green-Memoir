using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Data/TileData")]

public class TileData : ScriptableObject
{
    // FIX: This list is necessary for TileMapReadController.cs to function
    public List<TileBase> tiles;

    public bool plowable;

    public bool ableToMow;

    public bool ableToSeed;

    public bool waterable;

    // public bool collectible;
}