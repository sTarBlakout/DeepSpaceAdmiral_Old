using UnityEngine;

namespace GameGlobal
{
    public class ParticleManager : MonoBehaviour
    {
        private ParticleSystem _particleSystem;

        private bool _activateDestroy;

        private void Awake()
        {
            _particleSystem = GetComponent<ParticleSystem>();
        }

        private void Update()
        {
            if (!_activateDestroy) return;
            
            if (!_particleSystem.isPlaying)
                Destroy(gameObject);
        }

        public void ActivateParticle()
        {
            transform.SetParent(null);
            _particleSystem.Play();
            _activateDestroy = true;
        }
    }
}
