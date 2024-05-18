using UnityEngine;

public class ParticleController : MonoBehaviour
{
    [SerializeField] protected ParticleSystem burstSystem;

    private void OnParticleSystemStopped()
    {
        Destroy(transform.parent.gameObject);
    }

    public void CreateBurst()
    {
        burstSystem.Play();
    }
}