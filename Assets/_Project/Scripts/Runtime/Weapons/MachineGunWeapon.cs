using UnityEngine;
using System.Collections;
using _Project.Scripts.Runtime.Weapons;

namespace _Project.Scripts.Runtime
{
    public class MachineGunWeapon : Weapon
    {
        private Coroutine _autoFireCoroutine;
        
        public override void Shoot()
        {
            if (_autoFireCoroutine == null)
                _autoFireCoroutine = StartCoroutine(ShootAuto());
        }
        
        public override void StopShooting()
        {
            if (_autoFireCoroutine != null)
            {
                StopCoroutine(_autoFireCoroutine);
                _autoFireCoroutine = null;
            }
        }
        
        private IEnumerator ShootAuto()
        {
            float fireRate = 0.2f;
            while (true)
            {
                var direction = GetBulletDirection();
                Shoot(direction);
                yield return new WaitForSeconds(fireRate);
            }
        }
    }
}