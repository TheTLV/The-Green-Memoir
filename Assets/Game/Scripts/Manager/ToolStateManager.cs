using UnityEngine;
using System.Collections.Generic;
using System;

// ToolState class (Must be defined here or outside)
public class ToolState
{
    public ToolData toolData;
    public int currentUses;
}

public class ToolStateManager : MonoBehaviour
{
    public static ToolStateManager Instance;

    // Dictionary to track mutable tool states (uses ToolData as key)
    private Dictionary<ToolData, ToolState> toolStates = new Dictionary<ToolData, ToolState>();

    // Event to notify QuickSlotUI when a tool's uses change
    public event Action OnToolStateChanged;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    /// <summary>
    /// Retrieves the current state of a tool, creating it if needed.
    /// </summary>
    public ToolState GetToolState(ToolData tool)
    {
        if (!toolStates.ContainsKey(tool))
        {
            ToolState newState = new ToolState
            {
                toolData = tool,
                currentUses = tool.maxUses
            };
            toolStates.Add(tool, newState);
            return newState;
        }
        return toolStates[tool];
    }

    /// <summary>
    /// Uses a charge from the tool.
    /// </summary>
    public bool UseToolCharge(ToolData tool)
    {
        ToolState state = GetToolState(tool);

        if (state != null && state.currentUses > 0)
        {
            state.currentUses--;
            OnToolStateChanged?.Invoke();
            return true;
        }
        return false;
    }

    /// <summary>
    /// Refills the tool to max capacity.
    /// </summary>
    public void RefillTool(ToolData tool)
    {
        ToolState state = GetToolState(tool);
        if (state != null)
        {
            state.currentUses = tool.maxUses;
            OnToolStateChanged?.Invoke();
        }
    }
}