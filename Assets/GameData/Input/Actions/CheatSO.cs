using UnityEngine;
using TheGreenMemoir.Unity.Managers;
using TheGreenMemoir.Core.Domain.ValueObjects;

namespace TheGreenMemoir.Unity.Input.Actions
{
	/// <summary>
	/// Hệ thống cheat đa dạng cho nhiều thể loại game.
	/// Hỗ trợ: hack tiền, vàng, exp, level, NPC relationship, teleport, time, và nhiều thứ khác.
	/// </summary>
	[CreateAssetMenu(fileName = "CheatAction", menuName = "Game/Actions/Cheat", order = 24)]
	public class CheatSO : TheGreenMemoir.Unity.Input.ActionSOBase
	{
		public enum CheatType
		{
			AddMoney,           // Hack tiền
			AddGold,            // Hack vàng (nếu game có vàng riêng)
			AddExp,             // Hack kinh nghiệm
			AddLevel,           // Hack level
			AddEnergy,          // Hack năng lượng
			MaxEnergy,          // Max năng lượng
			Teleport,           // Teleport đến vị trí
			SetTime,             // Set thời gian (ngày/giờ)
			SkipDay,             // Bỏ qua ngày
			AddNPCFriendship,   // Hack độ thân mật với NPC
			SetNPCProgress,     // Hack tiến trình NPC (quest, event)
			AddSkillPoints,     // Hack điểm skill/attribute
			UnlockAll,          // Mở khóa tất cả
			GodMode,            // Chế độ bất tử
			Custom              // Cheat tùy chỉnh (dùng customAction)
		}

		[Header("Cheat Type")]
		[Tooltip("Loại cheat muốn thực hiện")]
		public CheatType cheatType = CheatType.AddMoney;

		[Header("Money & Resources")]
		[Tooltip("Số tiền thêm vào (cho AddMoney)")]
		public int moneyAmount = 1000;
		[Tooltip("Số vàng thêm vào (cho AddGold)")]
		public int goldAmount = 100;
		[Tooltip("Số exp thêm vào (cho AddExp)")]
		public int expAmount = 1000;

		[Header("Level & Stats")]
		[Tooltip("Level muốn set (cho AddLevel hoặc SetLevel)")]
		public int levelAmount = 1;
		[Tooltip("Số năng lượng thêm vào (cho AddEnergy)")]
		public int energyAmount = 100;
		[Tooltip("Số điểm skill/attribute thêm vào (cho AddSkillPoints)")]
		public int skillPointsAmount = 10;

		[Header("Teleport")]
		[Tooltip("Vị trí teleport (cho Teleport)")]
		public Vector3 teleportPosition = Vector3.zero;
		[Tooltip("Tên scene muốn teleport đến (để trống nếu cùng scene)")]
		public string teleportSceneName = "";

		[Header("Time")]
		[Tooltip("Số ngày bỏ qua (cho SkipDay)")]
		public int daysToSkip = 1;
		[Tooltip("Giờ muốn set (0-23, cho SetTime)")]
		public int hourToSet = 6;
		[Tooltip("Phút muốn set (0-59, cho SetTime)")]
		public int minuteToSet = 0;

		[Header("NPC")]
		[Tooltip("ID NPC (cho AddNPCFriendship, SetNPCProgress)")]
		public string npcId = "npc_001";
		[Tooltip("Số điểm thân mật thêm vào (cho AddNPCFriendship)")]
		public int friendshipPoints = 100;
		[Tooltip("Tiến trình muốn set (0-100, cho SetNPCProgress)")]
		public float npcProgress = 50f;

		[Header("Custom Action")]
		[Tooltip("Cheat tùy chỉnh - gọi method này nếu cheatType = Custom")]
		public UnityEngine.Events.UnityEvent customAction;

		public override void Execute(TheGreenMemoir.Unity.Input.ActionContext context)
		{
			var playerId = PlayerId.Default;

			switch (cheatType)
			{
				case CheatType.AddMoney:
					GameManager.PlayerService.AddMoney(playerId, new Money(moneyAmount));
					Debug.Log($"Cheat: +{moneyAmount} money");
					break;

				case CheatType.AddGold:
					// Giả sử có GoldService hoặc tương tự
					// GameManager.GoldService.AddGold(playerId, goldAmount);
					Debug.Log($"Cheat: +{goldAmount} gold (not implemented yet)");
					break;

				case CheatType.AddExp:
					// Giả sử có ExpService
					// GameManager.ExpService.AddExp(playerId, expAmount);
					Debug.Log($"Cheat: +{expAmount} exp (not implemented yet)");
					break;

				case CheatType.AddLevel:
					// GameManager.LevelService.AddLevel(playerId, levelAmount);
					Debug.Log($"Cheat: +{levelAmount} level (not implemented yet)");
					break;

				case CheatType.AddEnergy:
					var player = GameManager.PlayerService.GetPlayer(playerId);
					if (player != null && player.Energy != null)
					{
						// Energy là ValueObject, cần method Add hoặc tương tự
						// player.Energy.Add(energyAmount);
						Debug.Log($"Cheat: +{energyAmount} energy (check Energy implementation)");
					}
					break;

				case CheatType.MaxEnergy:
					// player.Energy.SetMax();
					Debug.Log($"Cheat: Max energy (not implemented yet)");
					break;

				case CheatType.Teleport:
					var playerObj = GameObject.FindGameObjectWithTag("Player");
					if (playerObj != null)
					{
						if (!string.IsNullOrEmpty(teleportSceneName))
						{
							// Lưu vị trí teleport
							PlayerPrefs.SetFloat("TeleportX", teleportPosition.x);
							PlayerPrefs.SetFloat("TeleportY", teleportPosition.y);
							PlayerPrefs.SetFloat("TeleportZ", teleportPosition.z);
							// Load scene và teleport sau khi load xong
							UnityEngine.SceneManagement.SceneManager.LoadScene(teleportSceneName);
							Debug.Log($"Cheat: Teleport to scene {teleportSceneName} at {teleportPosition}");
						}
						else
						{
							playerObj.transform.position = teleportPosition;
							Debug.Log($"Cheat: Teleport to {teleportPosition}");
						}
					}
					break;

				case CheatType.SetTime:
					// GameManager.TimeService.SetTime(hourToSet, minuteToSet);
					Debug.Log($"Cheat: Set time to {hourToSet}:{minuteToSet:00} (not implemented yet)");
					break;

				case CheatType.SkipDay:
					// GameManager.TimeService.SkipDays(daysToSkip);
					Debug.Log($"Cheat: Skip {daysToSkip} day(s) (not implemented yet)");
					break;

				case CheatType.AddNPCFriendship:
					// GameManager.NPCService.AddFriendship(npcId, friendshipPoints);
					Debug.Log($"Cheat: +{friendshipPoints} friendship with NPC {npcId} (not implemented yet)");
					break;

				case CheatType.SetNPCProgress:
					// GameManager.NPCService.SetProgress(npcId, npcProgress);
					Debug.Log($"Cheat: Set NPC {npcId} progress to {npcProgress}% (not implemented yet)");
					break;

				case CheatType.AddSkillPoints:
					// GameManager.SkillService.AddPoints(playerId, skillPointsAmount);
					Debug.Log($"Cheat: +{skillPointsAmount} skill points (not implemented yet)");
					break;

				case CheatType.UnlockAll:
					// GameManager.UnlockService.UnlockAll();
					Debug.Log($"Cheat: Unlock all (not implemented yet)");
					break;

				case CheatType.GodMode:
					// GameManager.PlayerService.SetGodMode(true);
					Debug.Log($"Cheat: God mode (not implemented yet)");
					break;

				case CheatType.Custom:
					if (customAction != null)
					{
						customAction.Invoke();
						Debug.Log($"Cheat: Custom action executed");
					}
					break;

				default:
					Debug.LogWarning($"Unknown cheat type: {cheatType}");
					break;
			}
		}
	}
}


