using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoystickSingleton : Joystick
{
    public static JoystickSingleton instance { get; private set; }

    private void Awake()
    {
        instance = this;
    }
}
