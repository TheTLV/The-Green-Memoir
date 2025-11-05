using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Manager References")]
    // Tham chiếu đến các Manager cần thiết
    [SerializeField] private TileMapReadController tileReadController;
    [SerializeField] private FarmingManager farmingManager;
    [SerializeField] private QuickSlotManager quickSlotManager; 
    [SerializeField] private ToolStateManager toolStateManager;  

    [Header("Interaction Settings")]
    public Vector2 facingDirection = new Vector2(0, -1);

    private ToolActionType currentTool = ToolActionType.None;

    private void Start()
    {
        if (farmingManager == null)
            farmingManager = FarmingManager.Instance;

        if (quickSlotManager == null)
            quickSlotManager = QuickSlotManager.Instance;

        if (toolStateManager == null)
            toolStateManager = ToolStateManager.Instance;
    }

    void Update()
    {

        currentTool = quickSlotManager != null ? quickSlotManager.GetSelectedToolData().actionType : ToolActionType.None;

        Vector3 interactionWorldPos = transform.position + (Vector3)facingDirection.normalized * 1.0f;
        Vector3Int targetGridPos = tileReadController.GetGridPosition(interactionWorldPos, false);

        if (Input.GetMouseButtonDown(0))
        {
            ToolData currentToolData = quickSlotManager.GetSelectedToolData();

            if (currentToolData == null) return;

            PerformAction(targetGridPos, currentToolData); 
        }
    }

    // --- TOOL SELECTION  ---


    public void SetTool(ToolActionType tool)
    {
        // currentTool = tool; // Đây là cách làm cũ
        Debug.Log($"Tool selected: {tool.ToString()}");
        // QuickSlotManager.Instance.SetToolByIndex(index);
    }


    public void PerformAction(Vector3Int pos, ToolData toolData)
    {
        ToolActionType tool = toolData.actionType;

        if (tool == ToolActionType.None) return;

        switch (tool)
        {
            case ToolActionType.Plow:
                farmingManager.PlowTile(pos);
                break;

            case ToolActionType.Water:
                if (toolStateManager.GetToolState(toolData).currentUses > 0)
                {
                    if (farmingManager.WaterTile(pos))
                    {
                        toolStateManager.UseToolCharge(toolData);
                    }
                }
                else
                {
                    Debug.Log("Water can is empty! Needs refill.");
                }
                break;

            case ToolActionType.Refill:
                farmingManager.RefillWater(pos, toolData);
                break;

            case ToolActionType.Harvest:
            case ToolActionType.Seed:

                if (farmingManager.IsCropPlanted(pos))
                {
                    farmingManager.HarvestTile(pos);
                }
                else if (farmingManager.IsGroundPlowed(pos))
                {
                    SeedSelectionUI.Instance.OpenSelectionUI(pos);
                }
                break;

            case ToolActionType.Interact:
                Debug.Log("Interacting with world object.");
                break;

            default:
                break;
        }
    }
}