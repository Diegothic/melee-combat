using ScriptableObjects.Audio;
using UnityEngine;

namespace Character.Audio
{
    public class HumanoidAudio : MonoBehaviour
    {
        private AudioSource source;

        [SerializeField]
        private AudioEvent footstepAudioEvent;

        [SerializeField]
        private AudioEvent swipeAudioEvent;

        [SerializeField]
        private AudioEvent getHitAudioEvent;

        [SerializeField]
        private AudioEvent deathAudioEvent;

        [SerializeField]
        private AudioEvent swordsHitAudioEvent;

        [SerializeField]
        private AudioEvent parryAudioEvent;

        [SerializeField]
        private AudioEvent counterAudioEvent;

        [SerializeField]
        private AudioEvent dodgeAudioEvent;

        [SerializeField]
        private AudioEvent blockAudioEvent;

        [SerializeField]
        private AudioEvent startBlockingAudioEvent;

        private void Awake()
        {
            source ??= GetComponent<AudioSource>();
        }

        public void AudioOnFootstep()
        {
            footstepAudioEvent.Play(source);
        }

        public void AudioOnAttack()
        {
            swipeAudioEvent.Play(source);
        }

        public void AudioOnGetHit()
        {
            getHitAudioEvent.Play(source);
        }

        public void AudioOnDeath()
        {
            deathAudioEvent.Play(source);
        }

        public void AudioOnSwordsHit()
        {
            swordsHitAudioEvent.Play(source);
        }

        public void AudioOnParry()
        {
            parryAudioEvent.Play(source);
        }

        public void AudioOnCounter()
        {
            counterAudioEvent.Play(source);
        }

        public void AudioOnDodge()
        {
            dodgeAudioEvent.Play(source);
        }

        public void AudioOnBlock()
        {
            blockAudioEvent.Play(source);
        }

        public void AudioOnStartBlocking()
        {
            startBlockingAudioEvent.Play(source);
        }
    }
}