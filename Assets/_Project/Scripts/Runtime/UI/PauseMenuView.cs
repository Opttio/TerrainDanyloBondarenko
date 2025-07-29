using System;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.Runtime.UI
{
    public class PauseMenuView : MonoBehaviour
    {
        [SerializeField] private Button _continueButton;
        [SerializeField] private Button _restartButton;
        [SerializeField] private Button _settingsButton;
        [SerializeField] private Button _exitButton;

        public Action OnContinue;
        public Action OnRestart;
        public Action OnSettings;
        public Action OnExit;

        private void OnEnable()
        {
            _continueButton.onClick.AddListener(() =>
            {
                UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
                OnContinue?.Invoke();
            });
            _restartButton.onClick.AddListener(() =>
            {
                UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
                OnRestart?.Invoke();
            });
            _settingsButton.onClick.AddListener(() =>
            {
                UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
                OnSettings?.Invoke();
            });
            _exitButton.onClick.AddListener(() =>
            {
                UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
                OnExit?.Invoke();
            });
        }

        private void OnDisable()
        {
            _continueButton.onClick.RemoveAllListeners();
            _restartButton.onClick.RemoveAllListeners();
            _settingsButton.onClick.RemoveAllListeners();
            _exitButton.onClick.RemoveAllListeners();
        }
    }
}