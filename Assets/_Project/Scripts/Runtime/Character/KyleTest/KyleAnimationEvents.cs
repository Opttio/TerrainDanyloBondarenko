using UnityEngine;

namespace _Project.Scripts.Runtime.Character.KyleTest
{
    public class KyleAnimationEvents : MonoBehaviour
    {
        [SerializeField] private ParticleSystem _landingParticles;
        public void PlayLandingParticles()
        {
            if (_landingParticles != null && !_landingParticles.IsAlive())
                _landingParticles.Play();
        }
    }
}