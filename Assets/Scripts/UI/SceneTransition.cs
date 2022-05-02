using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class SceneTransition : MonoBehaviour
    {
        private static readonly int GameOver = Animator.StringToHash("GameOver");
        private static readonly int FadeOut = Animator.StringToHash("FadeOut");
        private Animator _animator;

        private string _sceneToLoadName;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        public void FadeToScene(string sceneToLoadName, bool gameOver)
        {
            _sceneToLoadName = sceneToLoadName;
            _animator.SetTrigger(gameOver ? GameOver : FadeOut);
        }

        public void OnFadeComplete()
        {
            SceneManager.LoadScene(_sceneToLoadName);
        }
    }
}