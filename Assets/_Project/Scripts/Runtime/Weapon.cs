using UnityEngine;

namespace _Project.Scripts.Runtime
{
    public class Weapon : MonoBehaviour
    {
        [SerializeField] private Rigidbody _bulletRigidbody;
        [SerializeField] private Transform _shootPoint;
        [SerializeField] private float _bulletSpeed;

        public void Shoot()
        {
            var screenCenterPont = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
            var ray = Camera.main.ScreenPointToRay(screenCenterPont);
            if (Physics.Raycast(ray, out var hit, 30f))
            {
                var bulletDirection = (hit.point - _shootPoint.position).normalized;
                var bulletRotation = Quaternion.LookRotation(bulletDirection);
                var bulletInstance = Instantiate(_bulletRigidbody, _shootPoint.position, bulletRotation);
                bulletInstance.AddForce(bulletDirection * _bulletSpeed, ForceMode.Impulse);
            }
        }
    }
}