using UnityEngine;

/// <summary>
/// Simple script to set the playback speed of a particle system on application start
/// Allows you to also lock that speed so it can't be changed by another script.
/// </summary>

[RequireComponent(typeof(ParticleSystem))]
public class ParticleSystemSpeed : MonoBehaviour
{
    public float speed = 1f;
    public bool lockSpeed = false;

    ParticleSystem psys;
    void Start()
    {
        psys = gameObject.GetComponent<ParticleSystem>();
		ParticleSystem.MainModule main = psys.main;
        main.simulationSpeed= speed;
    }


    void LateUpdate()
    {
        if (!lockSpeed)
            return;

        if (!Mathf.Approximately(psys.main.simulationSpeed, speed))
        {
			ParticleSystem.MainModule main = psys.main;
			main.simulationSpeed = speed;
		}
    }
}