using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TankMania
{
    public class TankExplosionBehavior : MonoBehaviour
    {
        public AudioClip[] AudioClips;

        public event EventHandler<EventArgs> Finished;

        private AudioSource _audioSource;

        public void Start()
        {
            _audioSource = GetComponentInParent<AudioSource>();

            _audioSource.clip = AudioClips[Random.Range(0, AudioClips.Length)];
            _audioSource.Play();
        }

        public void FinishExplosion()
        {
            if (Finished != null)
                Finished(this, EventArgs.Empty);
        }
    }
}