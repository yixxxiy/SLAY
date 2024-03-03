using UnityEngine;
public class AutoDestroyParticle : MonoBehaviour
{
    void Update()
    {
        if (!GetComponent<ParticleSystem>().IsAlive())
        {
            Destroy(gameObject);
        }
    }
}
