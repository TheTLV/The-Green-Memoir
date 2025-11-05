using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class TileMapReadController : MonoBehaviour
{
    // C?n ??m b?o các bi?n này ???c kéo vào Inspector
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private List<TileData> tileDatas;
    private Dictionary<TileBase, TileData> dataFromTiles;

    private void Start()
    {
        dataFromTiles = new Dictionary<TileBase, TileData>();

        foreach (TileData tileData in tileDatas)
        {
            foreach (TileBase tile in tileData.tiles)
            {
                dataFromTiles.Add(tile, tileData);
            }
        }
    }

    public Vector3Int GetGridPosition(Vector2 position, bool mousePosition)
    {
        Vector3 worldPosition;

        if (mousePosition)
        {
            worldPosition = Camera.main.ScreenToWorldPoint(position);
        }
        else
        {
            worldPosition = position;
        }

        Vector3Int gridPosition = tilemap.WorldToCell(worldPosition);

        return gridPosition;
    }


    public TileBase GetTileBase(Vector3Int gridPosition)
    {
        TileBase tile = tilemap.GetTile(gridPosition);

        return tile;
    }

    public TileData GetTileData(TileBase tilebase)
    {
        if (dataFromTiles.ContainsKey(tilebase))
        {
            return dataFromTiles[tilebase];
        }
        return null;
    }
}