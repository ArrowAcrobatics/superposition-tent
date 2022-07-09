using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;

[System.Serializable]
public class PrimaryButtonEvent : UnityEvent<bool> { }

public class PrimaryButtonWatcher : MonoBehaviour
{
    public PrimaryButtonEvent primaryButtonPress;

    private bool lastButtonState = false;
    private List<InputDevice> devicesWithPrimaryButton;

    private void Awake() {
        if(primaryButtonPress == null) {
            primaryButtonPress = new PrimaryButtonEvent();
        }

        devicesWithPrimaryButton = new List<InputDevice>();
    }

    void OnEnable() {
        List<InputDevice> allDevices = new List<InputDevice>();
        InputDevices.GetDevices(allDevices);
        foreach(InputDevice device in allDevices)
            InputDevices_deviceConnected(device);

        InputDevices.deviceConnected += InputDevices_deviceConnected;
        InputDevices.deviceDisconnected += InputDevices_deviceDisconnected;
    }

    private void OnDisable() {
        InputDevices.deviceConnected -= InputDevices_deviceConnected;
        InputDevices.deviceDisconnected -= InputDevices_deviceDisconnected;
        devicesWithPrimaryButton.Clear();
    }

    private void InputDevices_deviceConnected(InputDevice device) {
        bool discardedValue;
        if(device.TryGetFeatureValue(CommonUsages.primaryButton, out discardedValue)) {
            Debug.Log("found device with primaryButton" + device.ToString());
            devicesWithPrimaryButton.Add(device); // Add any devices that have a primary button.
        }
    }

    private void InputDevices_deviceDisconnected(InputDevice device) {
        if(devicesWithPrimaryButton.Contains(device))
            devicesWithPrimaryButton.Remove(device);
    }

    void Update() {
        bool tempState = false;

        foreach(var device in devicesWithPrimaryButton) {
            bool primaryButtonState = false;
            bool gotValue = device.TryGetFeatureValue(CommonUsages.primaryButton, out primaryButtonState);

            if(!gotValue) {
                Debug.Log("failed to get value from primary button");
            }

            tempState =  gotValue // did get a value
                        && primaryButtonState // the value we got
                        || tempState; // cumulative result from other controllers
        }

        if(tempState != lastButtonState) // Button state changed since last frame
        {
            Debug.Log("button state changed" + tempState.ToString());
            primaryButtonPress.Invoke(tempState);
            lastButtonState = tempState;
        }
    }
}
