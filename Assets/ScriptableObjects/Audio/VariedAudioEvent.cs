using UnityEngine;

namespace ScriptableObjects.Audio
{
    [CreateAssetMenu(menuName = "Audio/VariedAudioEvent")]
    public class VariedAudioEvent : AudioEvent
    {
        public AudioClip[] clips;

        [Range(0.0f, 2.0f)]
        public float minVolume;
        [Range(0.0f, 2.0f)]
        public float maxVolume;

        [Range(0.0f, 2.0f)]
        public float minPitch;
        [Range(0.0f, 2.0f)]
        public float maxPitch;

        public override void Play(AudioSource source)
        {
            if (clips.Length == 0)
                return;

            source.clip = clips[Random.Range(0, clips.Length)];
            source.volume = Random.Range(minVolume, maxVolume);
            source.pitch = Random.Range(minPitch, maxPitch);

            source.Play();
        }
    }
}