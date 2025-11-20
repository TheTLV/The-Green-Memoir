using UnityEngine;
using System.Collections.Generic;

namespace TheGreenMemoir.Unity.Data
{
    /// <summary>
    /// Game Event ScriptableObject - Định nghĩa event trong game
    /// </summary>
    [CreateAssetMenu(fileName = "GameEvent", menuName = "Game/Event", order = 32)]
    public class GameEventSO : ScriptableObject
    {
        [Header("Basic Info")]
        public string eventId;
        public string eventName;
        [TextArea] public string description;

        [Header("Trigger")]
        public TriggerType triggerType;
        public string triggerParameter; // Location, Item ID, etc.

        [Header("Actions")]
        public List<EventAction> actions = new List<EventAction>();

        [Header("Conditions")]
        public List<EventCondition> conditions = new List<EventCondition>(); // Điều kiện để event xảy ra

        [Header("Settings")]
        public bool isOneTimeOnly; // Chỉ xảy ra 1 lần
        public bool isActive = true;

        [System.Serializable]
        public enum TriggerType
        {
            OnEnterLocation,
            OnInteract,
            OnItemUse,
            OnQuestComplete,
            OnTime,
            OnCustom
        }

        [System.Serializable]
        public class EventAction
        {
            public ActionType type;
            public string parameter;
            public int value;

            public enum ActionType
            {
                GiveItem,
                TakeItem,
                StartQuest,
                CompleteQuest,
                ChangeNPCState,
                PlaySound,
                ShowDialogue,
                Teleport,
                ChangeScene,
                SpawnEnemy,
                Custom
            }
        }

        [System.Serializable]
        public class EventCondition
        {
            public ConditionType type;
            public string parameter;
            public int value;
            public bool mustBeTrue = true;

            public enum ConditionType
            {
                HasItem,
                HasQuest,
                QuestCompleted,
                LevelReached,
                TimeOfDay,
                Custom
            }
        }
    }
}

