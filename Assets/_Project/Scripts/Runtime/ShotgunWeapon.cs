using UnityEngine;

namespace _Project.Scripts.Runtime
{
    public class ShotgunWeapon : Weapon
    {
        public override void Shoot()
        {
            var direction = GetBulletDirection();
            Shoot(direction);
        }

        protected override void Shoot(Vector3 direction)
        {
            var bulletRotation = Quaternion.LookRotation(direction);
            int bullets = Random.Range(5, 8);
            Debug.Log($"Bullets: {bullets}");
            for (int i = 0; i < bullets; i++)
            {
                // Vector3 positionOffset = Random.insideUnitSphere * 0.2f;
                Vector3 positionOffset = new Vector3(Random.Range(-0.15f, 0.15f), Random.Range(-0.15f, 0.15f), Random.Range(0f, 0.3f));
                var spawnPosition = _shootPoint.position + positionOffset;
                var bulletInstance = Instantiate(_bulletRigidbody, spawnPosition, bulletRotation);
                Vector3 randomDirection = Random.insideUnitSphere * 0.2f;
                Vector3 spreadDirection = (direction + randomDirection).normalized;
                bulletInstance.AddForce(spreadDirection * _bulletSpeed, ForceMode.Impulse);
            }
        }
    }
}