using Character;
using UI;
using UnityEngine;

namespace Manager
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField]
        private SceneTransition sceneTransition;

        [SerializeField]
        private string mainMenuSceneName;

        private void Awake()
        {
            HumanoidController.OnCharacterDeath += GameOver;
        }

        private void GameOver()
        {
            if (sceneTransition == null)
            {
                return;
            }

            sceneTransition.FadeToScene(mainMenuSceneName, true);
        }
    }
}