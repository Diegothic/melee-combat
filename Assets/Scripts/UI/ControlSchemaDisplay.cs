using System.Collections.Generic;
using Camera.Rotation;
using ScriptableObjects.Brains;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ControlSchemaDisplay : MonoBehaviour
    {
        private const string gamepadDevice = "Gamepad";
        private const string mouseKeyboardDevice = "MouseKeyboard";

        [SerializeField]
        private Image gamepadSchemaImage;
        [SerializeField]
        private Image mouseKeyboardSchemaImage;

        [SerializeField]
        private List<string> gamepadSchemaDevices;
        [SerializeField]
        private List<string> mouseKeyboardDevices;

        private string _currentDevice;

        private void Awake()
        {
            PlayerBrain.OnInputDeviceChanged += HandleDeviceChange;
            InputCameraRotator.OnInputDeviceChanged += HandleDeviceChange;
            gamepadSchemaImage.enabled = false;
            mouseKeyboardSchemaImage.enabled = false;
        }

        private void HandleDeviceChange(string deviceName)
        {
            var newDevice = "";
            if (gamepadSchemaDevices.Contains(deviceName))
            {
                newDevice = gamepadDevice;
            }
            else if (mouseKeyboardDevices.Contains(deviceName))
            {
                newDevice = mouseKeyboardDevice;
            }

            if (newDevice.Equals("") || newDevice == _currentDevice)
                return;

            _currentDevice = newDevice;
            gamepadSchemaImage.enabled = false;
            mouseKeyboardSchemaImage.enabled = false;

            if (_currentDevice == gamepadDevice)
            {
                gamepadSchemaImage.enabled = true;
            }
            else if (_currentDevice == mouseKeyboardDevice)
            {
                mouseKeyboardSchemaImage.enabled = true;
            }
        }
    }
}