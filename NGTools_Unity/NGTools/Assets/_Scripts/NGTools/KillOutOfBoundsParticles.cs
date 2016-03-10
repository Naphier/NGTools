using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(ParticleSystem))]
[ExecuteInEditMode]
public class KillOutOfBoundsParticles : MonoBehaviour
{
    ParticleSystem m_System;
    ParticleSystem.Particle[] m_Particles;
    public Vector3 min, max;
    public new Collider collider;

    private void LateUpdate()
    {
        InitializeIfNeeded();

        int numParticlesAlive = m_System.GetParticles(m_Particles);

        // Change only the particles that are alive
        bool killNow = false;
        for (int i = 0; i < numParticlesAlive; i++)
        {
            if (GetIsOutOfBounds(m_Particles[i].position))
            {
                m_Particles[i].lifetime = 0;
                killNow = true;
            }
        }

        if (killNow)
            m_System.SetParticles(m_Particles, numParticlesAlive);
    }

    float r;

    bool GetIsOutOfBounds(Vector3 position)
    {
        if (collider == null)
        {
            if (position.x < min.x || position.x > max.x ||
                position.y < min.y || position.y > max.y ||
                position.z < min.z || position.z > max.z)
                return true;
        }
        else
        {
            if (collider.GetType() == typeof(SphereCollider))
            {
                float radius = ((SphereCollider)collider).radius;
                r = radius;
                Vector3 center = collider.bounds.center;
                float dist = Vector3.Distance(center, position);
                if (dist > radius)
                    return true;
            }
            else if (collider.GetType() == typeof(BoxCollider))
            {
                if (!collider.bounds.Contains(position))
                    return true;
            }
        }


        return false;
    }

    void InitializeIfNeeded()
    {
        if (m_System == null)
            m_System = GetComponent<ParticleSystem>();

        if (m_Particles == null || m_Particles.Length < m_System.maxParticles)
            m_Particles = new ParticleSystem.Particle[m_System.maxParticles];
    }


    void OnGUI()
    {
        GUI.Label(new Rect(0, 0, 100, 100), "r: " + r.ToString());
    }
}
