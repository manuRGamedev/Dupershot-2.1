using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplaceParticlesByPlayerSpeed : MonoBehaviour
{
    ParticleSystem ps;
    ParticleSystem.MainModule main;
    PlayerController player;



    // Start is called before the first frame update
    void Start()
    {
        ps = GetComponent<ParticleSystem>();
        main = ps.main;
        var velocity = ps.velocityOverLifetime;
        velocity.enabled = true;

        player = PlayerController.instance;
    }

    // Update is called once per frame
    void Update()
    {
        int particleCount = ps.particleCount;
        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[particleCount];
        ps.GetParticles(particles);

        for (int i = 0; i < particles.Length; i++)
        {
            particles[i].velocity = particles[i].velocity + (Vector3)((-1) * player.movementSpeed * player.moveDir * Time.deltaTime);
        }

        ps.SetParticles(particles, particleCount);
    }
}
