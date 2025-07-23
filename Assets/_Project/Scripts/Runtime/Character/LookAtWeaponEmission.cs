using _Project.Scripts.Runtime.Weapons;
using UnityEngine;

namespace _Project.Scripts.Runtime.Character
{
    public class LookAtWeaponEmission : MonoBehaviour
    {
        [SerializeField] private float _maxRayDistance = 3f;

        private Weapon _currentWeapon;

        private void Update()
        {
            DetectAndEmissionWeapon();
        }

        private void DetectAndEmissionWeapon()
        {
            var screenCenterPoint = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
            var ray = Camera.main.ScreenPointToRay(screenCenterPoint);
            if (Physics.Raycast(ray, out var hit, _maxRayDistance))
            {
                if (hit.transform.TryGetComponent<Weapon>(out var weapon))
                {
                    if (_currentWeapon != weapon)
                    {
                        ClearCurrent();
                        _currentWeapon = weapon;
                        _currentWeapon.SetEmission(true);
                    }
                    return;
                }
            }
            ClearCurrent();
        }
        
        private void ClearCurrent()
        {
            if (_currentWeapon)
            {
                _currentWeapon.SetEmission(false);
                _currentWeapon = null;
            }
        }
    }
}