using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using TheGreenMemoir.Unity.Audio;

namespace TheGreenMemoir.Unity.UI
{
    /// <summary>
    /// Tutorial Controller - Quản lý màn hình hướng dẫn
    /// Flexible: Không lỗi nếu thiếu components
    /// </summary>
    public class TutorialController : MonoBehaviour
    {
        [Header("UI References (Optional)")]
        [SerializeField] private GameObject tutorialPanel;
        [SerializeField] private TextMeshProUGUI tutorialText;
        [SerializeField] private Button nextButton;
        [SerializeField] private Button skipButton;

        [Header("Tutorial Content")]
        [SerializeField] private string[] tutorialSteps = new string[]
        {
            "Welcome to The Green Memoir!",
            "Use WASD to move around.",
            "Press E to interact with objects.",
            "Press I to open inventory.",
            "Press ESC to open pause menu."
        };

        [Header("Audio (Optional)")]
        [SerializeField] private AudioClip buttonClickSound;
        [SerializeField] private AudioClip nextStepSound;

        private int currentStep = 0;

        private void Start()
        {
            if (tutorialPanel != null)
                tutorialPanel.SetActive(true);

            if (nextButton != null)
                nextButton.onClick.AddListener(OnNextClicked);

            if (skipButton != null)
                skipButton.onClick.AddListener(OnSkipClicked);

            ShowCurrentStep();
        }

        private void ShowCurrentStep()
        {
            if (currentStep >= tutorialSteps.Length)
            {
                OnTutorialComplete();
                return;
            }

            if (tutorialText != null && currentStep < tutorialSteps.Length)
            {
                tutorialText.text = tutorialSteps[currentStep];
            }
        }

        private void OnNextClicked()
        {
            PlayButtonSound();
            currentStep++;
            ShowCurrentStep();
        }

        private void OnSkipClicked()
        {
            PlayButtonSound();
            OnTutorialComplete();
        }

        private void OnTutorialComplete()
        {
            if (tutorialPanel != null)
                tutorialPanel.SetActive(false);

            // Load Game scene
            var sceneLoader = Managers.SceneLoader.Instance;
            if (sceneLoader != null)
            {
                sceneLoader.LoadScene("Game");
            }
            else
            {
                SceneManager.LoadScene("Game");
            }
        }

        private void PlayButtonSound()
        {
            if (AudioManager.Instance != null)
            {
                if (buttonClickSound != null)
                {
                    AudioManager.Instance.PlaySFX(buttonClickSound, 0.5f);
                }
                else
                {
                    AudioManager.Instance.PlayButtonClick();
                }
            }
        }

        /// <summary>
        /// Show tutorial (public method để gọi từ bên ngoài)
        /// </summary>
        public void ShowTutorial()
        {
            if (tutorialPanel != null)
            {
                tutorialPanel.SetActive(true);
                currentStep = 0;
                ShowCurrentStep();
            }
        }

        /// <summary>
        /// Close tutorial
        /// </summary>
        public void CloseTutorial()
        {
            if (tutorialPanel != null)
                tutorialPanel.SetActive(false);
        }
    }
}

