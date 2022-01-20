using UnityEngine;
using UnityEngine.SceneManagement;

namespace Menu
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField]
        private string pvcSceneName;
        [SerializeField]
        private string cvcSceneName;

        private void Awake()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        public void StartPVC()
        {
            SceneManager.LoadScene(pvcSceneName);
        }

        public void StartCVC()
        {
            SceneManager.LoadScene(cvcSceneName);
        }

        public void Exit()
        {
            Debug.Log("Exiting...");
            Application.Quit();
        }
    }
}