using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "New Seed Data", menuName = "Farming/Seed Data")]
public class SeedData : ScriptableObject
{
    public string cropName = "New Crop";

    [Header("Harvest & Growth Duration")]
    [Tooltip("Total days from planting to full maturity.")]
    public int daysToGrow = 4;
    public int harvestYield = 1;
    public ItemData harvestItem;

    [Header("Wilting & Watering")]
    [Tooltip("Number of days without water before the crop wilts and dies.")]
    public int daysToWilt = 2; 

    [Header("Tile Stages")]
    [Tooltip("Tiles for each growth stage when the crop is NOT watered.")]
    public TileBase[] dryGrowthTiles;


    [Tooltip("Tiles for each growth stage when the crop IS watered.")]
    public TileBase[] wetGrowthTiles;

    public TileBase witheredTile;
}