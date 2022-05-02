using UI;
using UnityEngine;

namespace Menu
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField]
        private string pvcSceneName;
        [SerializeField]
        private string cvcSceneName;

        [SerializeField]
        private SceneTransition sceneTransition;

        private void Awake()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        public void StartPVC()
        {
            if (sceneTransition == null)
            {
                return;
            }

            sceneTransition.FadeToScene(pvcSceneName, false);
        }

        public void StartCVC()
        {
            if (sceneTransition == null)
            {
                return;
            }

            sceneTransition.FadeToScene(cvcSceneName, false);
        }

        public void Exit()
        {
            Debug.Log("Exiting...");
            Application.Quit();
        }
    }
}