using _Project.Scripts.Core.ActionBases;
using UnityEngine;

namespace _Project.Scripts.Runtime
{
    public class Bullet : BulletActionBase
    {
        [SerializeField] private float _bulletDelay;
        [SerializeField] private float _bulletCollisionDelay;
        [SerializeField] private GameObject _bulletTracePrefab;

        protected override void ExecuteInternalOnStart()
        {
            Destroy(gameObject, _bulletDelay);
        }

        protected override void OnCollisionEnterDestroy(Collision collision)
        {
            Destroy(gameObject, _bulletCollisionDelay);
        }

        protected override void OnCollisionEnterCreate(Collision collision)
        {
            var contactPoint = collision.GetContact(0);
            var rotation = Quaternion.LookRotation(contactPoint.normal);
            var instance = Instantiate(_bulletTracePrefab, contactPoint.point, rotation);
        }
    }
}