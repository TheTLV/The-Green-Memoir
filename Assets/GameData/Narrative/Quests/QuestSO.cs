using UnityEngine;
using System.Collections.Generic;

namespace TheGreenMemoir.Unity.Data
{
    /// <summary>
    /// Quest ScriptableObject - Định nghĩa quest
    /// </summary>
    [CreateAssetMenu(fileName = "Quest", menuName = "Game/Quest", order = 30)]
    public class QuestSO : ScriptableObject
    {
        [Header("Basic Info")]
        public string questId;
        public string questName;
        [TextArea] public string description;

        [Header("Objectives")]
        public List<QuestObjective> objectives = new List<QuestObjective>();

        [Header("Rewards")]
        public int moneyReward;
        public int expReward;
        public List<QuestRewardItem> itemRewards = new List<QuestRewardItem>();

        [Header("Settings")]
        public bool isMainQuest;
        public bool isRepeatable;
        public QuestPrerequisite prerequisite;

        [System.Serializable]
        public class QuestObjective
        {
            public string objectiveId;
            public string description;
            public ObjectiveType type;
            public string targetId; // Item ID, NPC ID, etc.
            public int targetCount;
            public bool isCompleted;

            public enum ObjectiveType
            {
                CollectItem,
                KillEnemy,
                TalkToNPC,
                ReachLocation,
                CompleteQuest,
                Custom
            }
        }

        [System.Serializable]
        public class QuestRewardItem
        {
            public string itemId;
            public int quantity;
        }

        [System.Serializable]
        public class QuestPrerequisite
        {
            public string requiredQuestId;
            public int requiredLevel;
            public List<string> requiredItemIds = new List<string>();
        }
    }
}

