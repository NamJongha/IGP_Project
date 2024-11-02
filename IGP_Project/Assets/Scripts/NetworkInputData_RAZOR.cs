using Fusion;
//using Fusion.Editor;
using UnityEngine;


public enum InputButton
{
    LEFT = 1 << 0,
    RIGHT = 1 << 1
}
public struct NetworkInputData_tutorial : INetworkInput
{
    public NetworkButtons Buttons;

    public bool GetButton(InputButton button)
    {
        return Buttons.IsSet(button);
    }

    public NetworkButtons GetButtonPressed(NetworkButtons prev)
    {
        return Buttons.GetPressed(prev);
    }

    public bool AxisPressed()
    {
        return GetButton(InputButton.LEFT) || GetButton(InputButton.RIGHT);
    }
}
