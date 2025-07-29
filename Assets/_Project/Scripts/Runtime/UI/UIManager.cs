using System;
using _Project.Scripts.Core.Controllers;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace _Project.Scripts.Runtime.UI
{
    public class UIManager : MonoBehaviour, MyUIInputSystem.IUIActions
    {
        [SerializeField] private PauseMenuView _pauseMenuView;

        private void Awake()
        {
            InputController.SubscribeOnUIInput(this);
            Cursor.lockState = CursorLockMode.Locked;
            _pauseMenuView.OnContinue += TogglePause;
            _pauseMenuView.OnRestart += RestartGame;
            _pauseMenuView.OnSettings += OpenSettings;
            _pauseMenuView.OnExit += ExitToMainMenu;
        }

        private void OnEnable()
        {
            InputController.EnableUIInputSystem();
        }

        private void OnDisable()
        {
            InputController.DisableUIInputSystem();
        }

        private void OnDestroy()
        {
            InputController.UnsubscribeOnUIInput(this);
        }

        public void OnPauseToggle(InputAction.CallbackContext context)
        {
            if (context.phase != InputActionPhase.Performed)
                return;
            TogglePause();
        }

        private void TogglePause()
        {
            var isPauseActive = !_pauseMenuView.gameObject.activeSelf;
            _pauseMenuView.gameObject.SetActive(isPauseActive);

            if (isPauseActive)
            {
                InputController.DisableInputSystem();
                Cursor.lockState = CursorLockMode.Confined;
            }
            else
            { 
                InputController.EnableInputSystem();
                Cursor.lockState = CursorLockMode.Locked;
            }
        }

        private void RestartGame()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        private void OpenSettings()
        {
        }

        private void ExitToMainMenu()
        {
            Application.Quit();
        }
    }
}