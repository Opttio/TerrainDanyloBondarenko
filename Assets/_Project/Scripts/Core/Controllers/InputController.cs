
namespace _Project.Scripts.Core.Controllers
{
    public static class InputController
    {
        private static MyInputSystem _inputSystem = new MyInputSystem();
        private static MyUIInputSystem _uiInputSystem = new MyUIInputSystem();
        
        public static void EnableInputSystem() => _inputSystem.Enable();
        public static void DisableInputSystem() => _inputSystem.Disable();
        
        public static void EnableUIInputSystem() => _uiInputSystem.Enable();
        public static void DisableUIInputSystem() => _uiInputSystem.Disable();

        public static void SubscribeOnInput(MyInputSystem.IMainCharacterActions mainCharacterActions)
        {
            _inputSystem.MainCharacter.AddCallbacks(mainCharacterActions);
        }

        public static void SubscribeOnUIInput(MyUIInputSystem.IUIActions uiActions)
        {
            _uiInputSystem.UI.AddCallbacks(uiActions);
        }
        
        public static void UnsubscribeOnInput(MyInputSystem.IMainCharacterActions mainCharacterActions)
        {
            _inputSystem.MainCharacter.RemoveCallbacks(mainCharacterActions);
        }

        public static void UnsubscribeOnUIInput(MyUIInputSystem.IUIActions uiActions)
        {
            _uiInputSystem.UI.RemoveCallbacks(uiActions);
        }
    }
}