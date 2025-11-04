using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class FarmingManager : MonoBehaviour
{
    // Static reference for easy access (Singleton pattern)
    public static FarmingManager Instance;

    [Header("Tilemap & Tile References")]
    [SerializeField] private Tilemap groundTilemap;
    [SerializeField] private Tilemap cropTilemap;
    [SerializeField] private TileBase plowedDirtTile; // Dirt Tile Asset after plowing
    [SerializeField] private TileBase normalDirtTile; // The basic dirt tile (for harvest reset)
    [SerializeField] private TileBase wateredDirtTile; // Tile for wet dirt (visual feedback)

    // Dictionary to track crop states: GridPosition -> CropState Data
    private Dictionary<Vector3Int, CropState> cropStates = new Dictionary<Vector3Int, CropState>();

    // --- Inner Class: CropState (Data structure to hold a crop's current condition) ---
    public class CropState
    {
        public SeedData seedData;
        public int currentGrowthStage = 0; // 0 = Seed, 1..N = Stage
        public int daysWatered = 0; // Days the crop has been successfully watered
        public int daysSinceWatered = 0; // Used for wilting logic
        public bool isWateredToday = false;
        public bool isWilted = false;
    }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    // --- CORE TILE INTERACTION METHODS ---

    public bool PlowTile(Vector3Int gridPos)
    {
        TileBase tile = groundTilemap.GetTile(gridPos);

        // Assumption: TileMapReadController confirms tile is plowable
        if (tile != null && tile != plowedDirtTile)
        {
            groundTilemap.SetTile(gridPos, plowedDirtTile);
            Debug.Log($"Plowed tile at {gridPos}");
            return true;
        }
        return false;
    }

    public bool WaterTile(Vector3Int gridPos)
    {
        // 1. Check if there is a crop planted at this position
        if (cropStates.ContainsKey(gridPos))
        {
            CropState state = cropStates[gridPos];

            if (!state.isWateredToday)
            {
                state.isWateredToday = true;
                state.daysSinceWatered = 0; // Reset days since last water

                // Set the visual tile to wet dirt
                groundTilemap.SetTile(gridPos, wateredDirtTile);

                Debug.Log($"Watered crop at {gridPos}");
                return true;
            }
        }
        // If there's no crop, still allow watering the plowed dirt for visual feedback
        else if (IsGroundPlowed(gridPos))
        {
            groundTilemap.SetTile(gridPos, wateredDirtTile);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Plants a seed on the given grid position. (Called after selecting seed from UI)
    /// </summary>
    public void SeedTile(Vector3Int gridPos, SeedData seed)
    {
        // Requirement: Ground must be plowed and empty
        if (IsGroundPlowed(gridPos) && !IsCropPlanted(gridPos))
        {
            // 1. Add new Crop State
            CropState newState = new CropState { seedData = seed };
            cropStates.Add(gridPos, newState);

            // 2. Draw the initial seed Tile onto CropTilemap
            // We use growthTiles[0] which should be the seed Tile
            cropTilemap.SetTile(gridPos, seed.growthTiles[0]);

            // 3. TODO: Decrease the amount of seed in InventoryManager
            Debug.Log($"Planted {seed.cropName} at {gridPos}");
        }
    }

    /// <summary>
    /// Performs the Harvest action on the target grid position. (Used by the Sickle)
    /// </summary>
    public bool HarvestTile(Vector3Int gridPos)
    {
        if (cropStates.ContainsKey(gridPos))
        {
            CropState state = cropStates[gridPos];
            // Check if the crop is fully grown
            if (state.currentGrowthStage == state.seedData.daysToGrow && !state.isWilted)
            {
                // 1. TODO: Add harvested item to InventoryManager
                // InventoryManager.Instance.AddItem(state.seedData.harvestItem, state.seedData.harvestYield);
                Debug.Log($"Harvested {state.seedData.cropName}.");

                // 2. Clear the crop state and Tile
                cropStates.Remove(gridPos);
                cropTilemap.SetTile(gridPos, null);
                groundTilemap.SetTile(gridPos, normalDirtTile); // Reset back to normal dirt (or keep plowedDirtTile)
                return true;
            }
            else if (state.isWilted)
            {
                // Clear the wilted crop (no item yield)
                cropStates.Remove(gridPos);
                cropTilemap.SetTile(gridPos, null);
                groundTilemap.SetTile(gridPos, normalDirtTile);
                Debug.Log("Removed wilted crop.");
                return true;
            }
        }
        return false;
    }

    // --- UTILITY CHECKS ---
    public bool IsCropPlanted(Vector3Int gridPos)
    {
        return cropStates.ContainsKey(gridPos);
    }

    public bool IsGroundPlowed(Vector3Int gridPos)
    {
        return groundTilemap.GetTile(gridPos) == plowedDirtTile || groundTilemap.GetTile(gridPos) == wateredDirtTile;
    }

    // --- TIME MANAGEMENT & GROWTH LOGIC ---

    /// <summary>
    /// This is called once per game day (e.g., by a TimeManager script).
    /// Handles growth and wilting logic.
    /// </summary>
    public void EndOfDay()
    {
        List<Vector3Int> wiltedCrops = new List<Vector3Int>();

        foreach (var pair in cropStates)
        {
            Vector3Int pos = pair.Key;
            CropState state = pair.Value;

            // 1. Check for Wilting/Dying
            if (!state.isWateredToday)
            {
                state.daysSinceWatered++;
                // If not watered for 2 days, it wilts
                if (state.daysSinceWatered >= 2 && !state.isWilted)
                {
                    state.isWilted = true;
                    cropTilemap.SetTile(pos, state.seedData.witheredTile);
                    Debug.Log($"{state.seedData.cropName} at {pos} wilted!");
                }
            }

            // 2. Handle Growth (Only if watered and not wilted)
            if (state.isWateredToday && !state.isWilted)
            {
                // Increase growth stage if not fully mature
                if (state.currentGrowthStage < state.seedData.daysToGrow)
                {
                    state.currentGrowthStage++;
                    // Update Tile to the next growth stage
                    cropTilemap.SetTile(pos, state.seedData.growthTiles[state.currentGrowthStage]);
                    Debug.Log($"{state.seedData.cropName} grew to stage {state.currentGrowthStage}");
                }
            }

            // 3. Reset visual dirt tile and watering state for the new day
            // Change wet dirt back to plowed dirt
            if (groundTilemap.GetTile(pos) == wateredDirtTile)
            {
                groundTilemap.SetTile(pos, plowedDirtTile);
            }
            state.isWateredToday = false;
        }
    }
}