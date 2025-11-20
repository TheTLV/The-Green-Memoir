using UnityEngine;
using System.Collections.Generic;

namespace TheGreenMemoir.Unity.Data
{
    /// <summary>
    /// Dialogue ScriptableObject - Định nghĩa dialogue/conversation
    /// </summary>
    [CreateAssetMenu(fileName = "Dialogue", menuName = "Game/Dialogue", order = 31)]
    public class DialogueSO : ScriptableObject
    {
        [Header("Basic Info")]
        public string dialogueId;
        public string npcId; // NPC nào nói dialogue này

        [Header("Dialogue Nodes")]
        public List<DialogueNode> nodes = new List<DialogueNode>();

        [System.Serializable]
        public class DialogueNode
        {
            public string nodeId;
            public string speakerName;
            [TextArea] public string text;
            public Sprite speakerPortrait;

            [Header("Choices")]
            public List<DialogueChoice> choices = new List<DialogueChoice>();

            [Header("Actions")]
            public List<DialogueAction> actions = new List<DialogueAction>();

            [Header("Next Node")]
            public string nextNodeId; // Nếu không có choices, tự động chuyển đến node này
        }

        [System.Serializable]
        public class DialogueChoice
        {
            public string choiceText;
            public string nextNodeId;
            public List<DialogueCondition> conditions = new List<DialogueCondition>(); // Điều kiện để hiển thị choice này
        }

        [System.Serializable]
        public class DialogueAction
        {
            public ActionType type;
            public string parameter; // Item ID, Quest ID, etc.

            public enum ActionType
            {
                GiveItem,
                TakeItem,
                StartQuest,
                CompleteQuest,
                ChangeNPCState,
                PlaySound,
                ShowAnimation,
                Teleport,
                Custom
            }
        }

        [System.Serializable]
        public class DialogueCondition
        {
            public ConditionType type;
            public string parameter;
            public int value;

            public enum ConditionType
            {
                HasItem,
                HasQuest,
                QuestCompleted,
                LevelReached,
                Custom
            }
        }
    }
}

