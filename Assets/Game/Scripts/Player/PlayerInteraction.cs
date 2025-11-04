using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public TileMapReadController tileReadController;
    public FarmingManager farmingManager; 

    [Header("Current Tool State")]
    public ToolActionType currentTool = ToolActionType.None;

    public Vector2 facingDirection = new Vector2(0, -1);

    private void Start()
    {
        if (farmingManager == null)
            farmingManager = FarmingManager.Instance;
    }

    void Update()
    {
        HandleToolSelectionInput();

        Vector3 interactionWorldPos = transform.position + (Vector3)facingDirection.normalized * 1.0f;
        Vector3Int targetGridPos = tileReadController.GetGridPosition(interactionWorldPos, false);

        if (Input.GetMouseButtonDown(0))
        {
            PerformAction(targetGridPos, currentTool);
        }
    }

    private void HandleToolSelectionInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) { SetTool(ToolActionType.Plow); }  
        else if (Input.GetKeyDown(KeyCode.Alpha2)) { SetTool(ToolActionType.Harvest); } 
        else if (Input.GetKeyDown(KeyCode.Alpha3)) { SetTool(ToolActionType.Seed); } 
        else if (Input.GetKeyDown(KeyCode.Alpha4)) { SetTool(ToolActionType.Water); }  
    }

    public void SetTool(ToolActionType tool)
    {
        currentTool = tool;
        Debug.Log($"Tool selected: {tool.ToString()}");
    }


    private void PerformAction(Vector3Int pos, ToolActionType tool)
    {
        if (tool == ToolActionType.None) return;

        switch (tool)
        {
            case ToolActionType.Plow:
                farmingManager.PlowTile(pos);
                break;
            case ToolActionType.Water:
                farmingManager.WaterTile(pos);
                break;
            case ToolActionType.Harvest:
                
                break;
            case ToolActionType.Seed:
                
                break;
            default:
                break;
        }
    }
}