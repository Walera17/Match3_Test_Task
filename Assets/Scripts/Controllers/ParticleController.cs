using Gems;
using UnityEngine;
using UnityEngine.Events;

namespace Controllers
{
    public class ParticleController : MonoBehaviour
    {
        public static event UnityAction<Gem> OnGemDestroy;

        [SerializeField] protected ParticleSystem burstSystem;
        [SerializeField] private Gem gem;

        private void OnParticleSystemStopped()
        {
            OnGemDestroy?.Invoke(gem);
            //transform.parent.gameObject.SetActive(false);
        }

        public void CreateBurst()
        {
            burstSystem.Play();
        }
    }
}