using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
[ExecuteInEditMode]
public class KillOutOfBoundsParticles : MonoBehaviour
{
    ParticleSystem m_System;
    ParticleSystem.Particle[] m_Particles;
    public float minX,maxX;


    private void LateUpdate()
    {
        InitializeIfNeeded();

        int numParticlesAlive = m_System.GetParticles(m_Particles);

        // Change only the particles that are alive
        bool killNow = false;
        for (int i = 0; i < numParticlesAlive; i++)
        {
            if (m_Particles[i].position.x < minX || m_Particles[i].position.x > maxX)
            {
                m_Particles[i].color = new Color(0f, 0f, 0f, 0f);
                killNow = true;
            }
        }

        if (killNow)
            m_System.SetParticles(m_Particles, numParticlesAlive);
    }

    void InitializeIfNeeded()
    {
        if (m_System == null)
            m_System = GetComponent<ParticleSystem>();

        if (m_Particles == null || m_Particles.Length < m_System.maxParticles)
            m_Particles = new ParticleSystem.Particle[m_System.maxParticles];
    }
}
