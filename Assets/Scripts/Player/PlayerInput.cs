using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    float h, v;
    Vector2 axisInput;
    public bool isMoving;
    public Vector2 GetAxis() {
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");

        isMoving = Mathf.Abs(h) + Mathf.Abs(v) != 0;

        axisInput = new Vector2(h, v);

        return axisInput;
    }

    public bool GetJumpInput() {
        return Input.GetKeyDown(KeyCode.Space);
    }
    
    public bool GetRollInput() {
        return Input.GetKeyDown(KeyCode.LeftShift);
    }

    public int GetNumberInput()
    {
        for (int i = 0; i < 9; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha0 + i))
            {
                return i;
            }
        }

        return -1;
    }

    public Vector2 GetAxisStored() {
        return axisInput;
    }
}
