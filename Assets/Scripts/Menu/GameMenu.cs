using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Menu
{
    public class GameMenu : MonoBehaviour
    {
        private GameInputSchema _input;

        private void OnEnable()
        {
            _input = new GameInputSchema();

            _input.Menu.Reset.started += Reset;
            _input.Menu.Reset.Enable();

            _input.Menu.Exit.started += Exit;
            _input.Menu.Exit.Enable();
        }

        private void Awake()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private static void Reset(InputAction.CallbackContext ctx)
        {
            var activeScene = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(0);
            SceneManager.LoadScene(activeScene);
        }

        private static void Exit(InputAction.CallbackContext ctx)
        {
            SceneManager.LoadScene(0);
        }
    }
}