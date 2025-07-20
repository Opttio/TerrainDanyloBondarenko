using UnityEngine;

namespace _Project.Scripts.Runtime
{
    public class Weapon : CollectibleItems
    {
        [Header("Bullet Information")]
        [SerializeField] protected Rigidbody _bulletRigidbody;
        [SerializeField] protected Transform _shootPoint;
        [SerializeField] protected float _bulletSpeed;
        [Space] [Header("Weapon Emission")]
        [SerializeField] private Renderer _renderer;
        
        private Material _material;

        public virtual void Shoot()
        {
            var direction = GetBulletDirection();
            Shoot(direction);
        }

        public virtual void StopShooting() { }

        protected virtual void Shoot(Vector3 direction)
        {
            var bulletRotation = Quaternion.LookRotation(direction);
            var bulletInstance = Instantiate(_bulletRigidbody, _shootPoint.position, bulletRotation);
            bulletInstance.AddForce(direction * _bulletSpeed, ForceMode.Impulse);
        }

        protected virtual Vector3 GetBulletDirection()
        {
            var screenCenterPoint = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
            var ray = Camera.main.ScreenPointToRay(screenCenterPoint);
            if (Physics.Raycast(ray, out var hit, 30f))
            {
                var bulletDirection = (hit.point - _shootPoint.position).normalized;
                return bulletDirection;
            }
            return (ray.GetPoint(30f) - _shootPoint.position).normalized;
        }
        
        public void SetEmission(bool enable)
        {
            if (!_renderer) return;
            Material material = _renderer.material;

            if (enable)
            {
                material.EnableKeyword("_EMISSION");
            }

            else
            {
                material.DisableKeyword("_EMISSION");
            }
        }
    }
}