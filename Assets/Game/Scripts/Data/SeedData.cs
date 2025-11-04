using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;


[CreateAssetMenu(fileName = "New Seed Data", menuName = "Farming/Seed Data")]
public class SeedData : ScriptableObject
{
    public string cropName = "New Crop";

    [Header("Thời gian và Trạng thái")]
    public int daysToGrow = 4; 
    public int harvestYield = 1; 
    public ItemData harvestItem; 

    [Header("Tile Trạng thái")]
    public TileBase[] growthTiles;
    public TileBase witheredTile; 
}