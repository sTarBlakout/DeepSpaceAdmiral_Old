using UnityEngine;

namespace GameGlobal
{
    public class ParticleManager : MonoBehaviour
    {
        [SerializeField] private bool activateOnAwake = true;
        
        private ParticleSystem _particleSystem;

        private bool _activateDestroy;

        private void Awake()
        {
            _particleSystem = GetComponent<ParticleSystem>();
            if (activateOnAwake) ActivateParticle();
        }

        private void Update()
        {
            if (!_activateDestroy) return;
            
            if (!_particleSystem.isPlaying)
                Destroy(gameObject);
        }

        public void ActivateParticle()
        {
            transform.SetParent(AllData.Instance.ParticleContainer);
            _particleSystem.Play();
            _activateDestroy = true;
        }
    }
}
