using UnityEngine;

namespace _Project.Scripts.Runtime.Weapons
{
    public class CollectibleItems : MonoBehaviour
    {
        [Header("Collectible Items")]
        [SerializeField] private Transform _attachedTo;
        [SerializeField] private float _dropForce;
        [Space]
        [SerializeField] protected Rigidbody _collectibleRb;
        [SerializeField] protected Collider[] _colliders;
        

        public bool IsAttached => _attachedTo != null;

        private void Awake()
        {
            if (IsAttached)
                Attach(_attachedTo);
        }

        public void Attach(Transform attachedTo)
        {
            _attachedTo = attachedTo;
            transform.SetParent(attachedTo);
            _collectibleRb.isKinematic = true;
            foreach (Collider collider in _colliders)
                collider.enabled = false;
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }

        public void Detach()
        {
            _attachedTo = null;
            transform.parent = null;
            _collectibleRb.isKinematic = false;
            foreach (Collider collider in _colliders) 
                collider.enabled = true;
            _collectibleRb.AddForce(transform.forward * _dropForce, ForceMode.Impulse);
        }
        
        

    }
}