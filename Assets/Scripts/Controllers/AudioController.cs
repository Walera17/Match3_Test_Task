using BoardCode;
using Commons;
using UnityEngine;

namespace Controllers
{
    public class AudioController : MonoBehaviour
    {
        [SerializeField] AudioSource m_AudioSource;
        [SerializeField] AudioSource gemAudioSource;
        [SerializeField] AudioSource bombAudioSource;
        [SerializeField] AudioSource stoneAudioSource;
        [SerializeField] private AudioClip bombClip;
        [SerializeField] private AudioClip stoneClip;
        [SerializeField] private AudioClip gemClip;
        [SerializeField] private AudioClip levelCompleteClip;
        [SerializeField] private AudioClip mainMenuClip;
        [SerializeField] private AudioClip[] levelClips;

        private void OnEnable()
        {
            Board.OnAudioEffect += Board_OnAudioEffect;
            RoundController.OnLevelCompleted += RoundController_OnLevelCompleted;
            RoundController.OnActiveMenu += RoundController_OnActiveMenu;
        }

        private void OnDisable()
        {
            Board.OnAudioEffect -= Board_OnAudioEffect;
            RoundController.OnLevelCompleted -= RoundController_OnLevelCompleted;
            RoundController.OnActiveMenu -= RoundController_OnActiveMenu;
        }

        private void RoundController_OnLevelCompleted()
        {
            m_AudioSource.loop = false;
            m_AudioSource.clip = levelCompleteClip;
            m_AudioSource.Play();
        }

        private void RoundController_OnActiveMenu(bool isMenu)
        {
            m_AudioSource.loop = true;
            m_AudioSource.clip = isMenu ? mainMenuClip : levelClips[Random.Range(0, levelClips.Length)];
            m_AudioSource.Play();
        }

        private void Board_OnAudioEffect(GemType type)
        {
            switch (type)
            {
                case GemType.Blue:
                case GemType.Green:
                case GemType.Red:
                case GemType.Yellow:
                case GemType.Purple:
                    if (!gemAudioSource.isPlaying)
                    {
                        gemAudioSource.clip = gemClip;
                        gemAudioSource.Play();
                    }
                    break;
                case GemType.Bomb:
                    if (!bombAudioSource.isPlaying)
                    {
                        bombAudioSource.clip = bombClip;
                        bombAudioSource.Play();
                    }
                    break;
                case GemType.Stone:
                    if (!stoneAudioSource.isPlaying)
                    {
                        stoneAudioSource.clip = stoneClip;
                        stoneAudioSource.Play();
                    }
                    break;
            }
        }
    }
}