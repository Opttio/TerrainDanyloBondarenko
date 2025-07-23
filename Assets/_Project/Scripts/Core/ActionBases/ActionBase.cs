using UnityEngine;
using UnityEngine.Serialization;

namespace _Project.Scripts.Core.ActionBases
{
    public abstract class ActionBase : MonoBehaviour
    {
        [SerializeField] protected bool _executeOnStart;
        [SerializeField] protected bool _executeOnCollisionDestroy;
        [SerializeField] protected bool _executeOnCollisionCreate;
        [SerializeField] protected bool _executeOnCollisionChangeColorAnimation;

        private void Start()
        {
            if (_executeOnStart) 
                ExecuteInternalOnStart();
        }
        private void OnCollisionEnter(Collision collision)
        {
            if (_executeOnCollisionDestroy)
                OnCollisionEnterDestroy(collision);

            if (_executeOnCollisionCreate)
                OnCollisionEnterCreate(collision);
            
            if (_executeOnCollisionChangeColorAnimation)
                OnCollisionEnterGetDamage(collision);
                
        }

        protected abstract void OnCollisionEnterDestroy(Collision collision);
        protected abstract void OnCollisionEnterCreate(Collision collision);
        protected abstract void OnCollisionEnterGetDamage(Collision collision);
        protected abstract void ExecuteInternalOnStart();
    }
}