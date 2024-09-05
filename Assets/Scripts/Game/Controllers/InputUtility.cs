using UnityEngine;

public class InputUtility
{

    public static bool TouchStarted()
    {
        if (Input.touchSupported)
        {
            return Input.touchCount >= 1 && Input.GetTouch(0).phase == TouchPhase.Began;
        }
        else
        {
            return Input.GetMouseButtonDown(0);
        }
    }


    public static bool TouchStopped()
    {
        if (Input.touchSupported)
        {
            return Input.touchCount >= 1 && Input.GetTouch(0).phase == TouchPhase.Ended;
        }
        else
        {
            return Input.GetMouseButtonUp(0);
        }
    }

    public static Vector2 GetTouchPosition()
    {
        if (Input.touchSupported)
        {
            return Input.GetTouch(0).position;
        }
        else
        {
            return Input.mousePosition;
        }
    }
}