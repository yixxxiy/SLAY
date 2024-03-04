using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugJoystick : MonoBehaviour
{
    private Joystick _joystick;

    // Start is called before the first frame update
    void Start()
    {
        _joystick = GetComponent<Joystick>();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("direction:" + _joystick.Direction.ToString());
    }
}
