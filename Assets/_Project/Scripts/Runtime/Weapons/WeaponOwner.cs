using UnityEngine;

namespace _Project.Scripts.Runtime.Weapons
{
    public class WeaponOwner : MonoBehaviour
    {
        [SerializeField] private Transform _holder;
        [SerializeField] private Weapon _weapon;
        
        public Transform Holder => _holder;

        public void Shoot()
        {
            // Debug.Log($"Weapon type: {_weapon.GetType().Name}");
            _weapon?.Shoot();
        }

        public void StopShooting()
        {
            _weapon?.StopShooting();
        }

        public void Collect()
        {
            var screenCenterPoint = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
            var ray = Camera.main.ScreenPointToRay(screenCenterPoint);
            if (Physics.Raycast(ray, out var hit, 3f))
            {
                if (hit.collider.TryGetComponent<CollectibleItems>(out var collectibleItems))
                {
                    if (collectibleItems is Weapon newWeapon)
                    {
                        _weapon?.Detach();
                        _weapon = newWeapon;
                        _weapon.Attach(_holder);
                    }
                }
            }
        }
    }
}