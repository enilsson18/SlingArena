using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ParticleSystem))]
public class ParticleAttraction : MonoBehaviour
{
    public Transform Target;

    public float speed = 1.0f;

    private ParticleSystem system;

    private static ParticleSystem.Particle[] particles = new ParticleSystem.Particle[1000];

    private void Start()
    {
        
    }

    void Update()
    {
        if (Target != null)
        {
            if (system == null) system = GetComponent<ParticleSystem>();

            var count = system.GetParticles(particles);

            for (int i = 0; i < count; i++)
            {
                var particle = particles[i];

                float distance = Vector3.Distance(Target.position, particle.position);

                if (distance > 0.25f)
                {
                    //acceleration method vs direct position control

                    //accel method increases acceleration so it has stronger acceleration as it gets closer
                    particle.velocity += (Target.position - particle.position) * speed * 1/(distance*distance);

                    //normal pos method
                    //particle.position = Vector3.Lerp(particle.position, Target.position, Time.deltaTime * speed);
                }
                else
                {
                    hitAction(ref particle);
                }

                particles[i] = particle;
            }

            system.SetParticles(particles, count);
        }
    }

    //i know this should be a callback function but I am too lazy so deal with it
    //increment progress bar
    void hitAction(ref ParticleSystem.Particle particle)
    {
        ProgressBar progressBar;
        progressBar = GameObject.FindGameObjectWithTag("progress_bar").GetComponent<ProgressBar>();

        progressBar.increment();

        particle.remainingLifetime = 0;
    }
}