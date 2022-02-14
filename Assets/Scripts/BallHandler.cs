using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BallHandler : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Since our finger may not be down of the frame it checks it needs to do a check first to see if our finger is even down first
        // So, if touchscreen is not pressed then do "return" but if pressed go to the next lines of code in the Update function
        if(!Touchscreen.current.primaryTouch.press.isPressed)
        {
            // Return says if we get here dont run anything underneath; In this instance jump out of this function(Update()) for this frame
            return;
        }


        // "current" is the touchscreen on the phone; "primaryTouch" is first finger that touches down; "ReadValue()" converts it into a Vector2 bc they use something
        // called Vector2Control which is samething just for mobile; Then we store this in a var "touchPosition"

        // This basically says every frame get our touch and give us the position for that touch and STORE IT IN THE VARIABLE
        Vector2 touchPosition = Touchscreen.current.primaryTouch.position.ReadValue();

        Debug.Log(touchPosition);
    }
}
