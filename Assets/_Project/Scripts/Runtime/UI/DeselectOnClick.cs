using UnityEngine;
using UnityEngine.EventSystems;

namespace _Project.Scripts.Runtime.UI
{
    public class DeselectOnClick : MonoBehaviour
    {
        public void Deselect()
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }
}