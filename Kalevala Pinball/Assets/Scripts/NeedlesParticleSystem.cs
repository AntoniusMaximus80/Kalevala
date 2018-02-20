using UnityEngine;
using System.Collections;

public class NeedlesParticleSystem : MonoBehaviour
{
    private float _particleSystemDuration,
        _destroyCounter;

    // Use this for initialization
    void Start()
    {
        _particleSystemDuration = GetComponent<ParticleSystem>().main.duration;
    }

    // Update is called once per frame
    void Update()
    {
        _destroyCounter += Time.deltaTime;
        if (_destroyCounter >= _particleSystemDuration)
        {
            Destroy(gameObject);
        }
    }
}
