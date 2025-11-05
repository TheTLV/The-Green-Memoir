using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System.Linq;
using System;

public class FarmingManager : MonoBehaviour
{
    public static FarmingManager Instance;

    [Header("Tilemap & Tile References")]
    [SerializeField] private Tilemap groundTilemap;
    [SerializeField] private Tilemap cropTilemap;
    [SerializeField] private TileBase plowedDirtTile;
    [SerializeField] private TileBase normalDirtTile;
    [SerializeField] private TileBase wateredDirtTile;

    private Dictionary<Vector3Int, CropState> cropStates = new Dictionary<Vector3Int, CropState>();

    public class CropState
    {
        public SeedData seedData;
        public int currentGrowthStage = 0;
        public int daysWatered = 0;
        public int daysSinceWatered = 0;
        public bool isWateredToday = false;
        public bool isWilted = false;
    }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.OnDayEnd += EndOfDay;
        }
    }

    private void OnDestroy()
    {
        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.OnDayEnd -= EndOfDay;
        }
    }

    // --- CORE TILE INTERACTION METHODS ---

    public bool PlowTile(Vector3Int gridPos)
    {
        TileBase tile = groundTilemap.GetTile(gridPos);

        if (tile != plowedDirtTile && tile != wateredDirtTile)
        {
            groundTilemap.SetTile(gridPos, plowedDirtTile);
            return true;
        }
        return false;
    }


    public bool WaterTile(Vector3Int gridPos)
    {
        if (cropStates.ContainsKey(gridPos))
        {
            CropState state = cropStates[gridPos];

            if (!state.isWateredToday)
            {
                state.isWateredToday = true;
                state.daysSinceWatered = 0;

                if (state.currentGrowthStage < state.seedData.wetGrowthTiles.Length)
                {
                    cropTilemap.SetTile(gridPos, state.seedData.wetGrowthTiles[state.currentGrowthStage]);
                }

                groundTilemap.SetTile(gridPos, wateredDirtTile);
                return true;
            }
        }
        else if (IsGroundPlowed(gridPos) && groundTilemap.GetTile(gridPos) != wateredDirtTile)
        {
            groundTilemap.SetTile(gridPos, wateredDirtTile);
            return true;
        }
        return false;
    }

    public void SeedTile(Vector3Int gridPos, SeedData seed)
    {
        if (IsGroundPlowed(gridPos) && !IsCropPlanted(gridPos))
        {
            CropState newState = new CropState { seedData = seed };
            cropStates.Add(gridPos, newState);

            if (seed.dryGrowthTiles.Length > 0)
            {
                cropTilemap.SetTile(gridPos, seed.dryGrowthTiles[0]);
            }
        }
    }

    public bool HarvestTile(Vector3Int gridPos)
    {
        if (cropStates.ContainsKey(gridPos))
        {
            CropState state = cropStates[gridPos];

            if (state.isWilted)
            {
                ClearCrop(gridPos);
                Debug.Log("Removed wilted crop (no yield).");
                return true;
            }
            else if (state.currentGrowthStage == state.seedData.daysToGrow)
            {
                InventoryManager.Instance.AddItem(state.seedData.harvestItem, state.seedData.harvestYield);
                ClearCrop(gridPos);
                Debug.Log($"Harvested {state.seedData.cropName}.");
                return true;
            }
        }
        return false;
    }

    public void RefillWater(Vector3Int gridPos, ToolData waterCanData)
    {
        ToolStateManager.Instance.RefillTool(waterCanData);
        Debug.Log($"Refilled {waterCanData.toolName} to max capacity!");
    }

    // --- UTILITY & CLEANUP ---

    private void ClearCrop(Vector3Int gridPos)
    {
        cropStates.Remove(gridPos);
        cropTilemap.SetTile(gridPos, null);
        groundTilemap.SetTile(gridPos, normalDirtTile);
    }

    public bool IsGroundPlowed(Vector3Int gridPos)
    {
        return groundTilemap.GetTile(gridPos) == plowedDirtTile || groundTilemap.GetTile(gridPos) == wateredDirtTile;
    }

    public bool IsCropPlanted(Vector3Int gridPos)
    {
        return cropStates.ContainsKey(gridPos);
    }

    // --- TIME MANAGEMENT & GROWTH LOGIC ---

    public void EndOfDay()
    {
        foreach (var pos in cropStates.Keys.ToList())
        {
            CropState state = cropStates[pos];

            // 1. Check for Wilting/Dying
            if (!state.isWateredToday)
            {
                state.daysSinceWatered++;

                if (state.daysSinceWatered >= state.seedData.daysToWilt && !state.isWilted)
                {
                    state.isWilted = true;
                    if (state.seedData.witheredTile != null)
                    {
                        cropTilemap.SetTile(pos, state.seedData.witheredTile);
                    }
                }
            }

            // 2. Handle Growth (Only if watered and not wilted)
            if (state.isWateredToday && !state.isWilted)
            {
                state.daysWatered++;

                if (state.currentGrowthStage < state.seedData.daysToGrow)
                {
                    state.currentGrowthStage++;
                }
            }

            // 3. Reset visual dirt tile and watering state for the new day
            if (!state.isWilted)
            {
                int stage = state.currentGrowthStage;
                if (stage < state.seedData.dryGrowthTiles.Length)
                {
                    cropTilemap.SetTile(pos, state.seedData.dryGrowthTiles[stage]);
                }
            }

            if (groundTilemap.GetTile(pos) == wateredDirtTile)
            {
                groundTilemap.SetTile(pos, plowedDirtTile);
            }
            state.isWateredToday = false;
        }
    }
}