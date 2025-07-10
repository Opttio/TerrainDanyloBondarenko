using UnityEngine;

namespace _Project.Scripts.Core.ActionBases
{
    public abstract class BulletActionBase : MonoBehaviour
    {
        [SerializeField] protected bool _executeOnStart;
        [SerializeField] protected bool _executeOnCollision;
        [SerializeField] protected bool _executeOnCollisionCreate;

        private void Start()
        {
            if (_executeOnStart) 
                ExecuteInternalOnStart();
        }
        private void OnCollisionEnter(Collision collision)
        {
            if (_executeOnCollision)
                OnCollisionEnterDestroy(collision);

            if (_executeOnCollisionCreate)
                OnCollisionEnterCreate(collision);
        }

        protected abstract void OnCollisionEnterDestroy(Collision collision);
        protected abstract void OnCollisionEnterCreate(Collision collision);
        protected abstract void ExecuteInternalOnStart();
    }
}