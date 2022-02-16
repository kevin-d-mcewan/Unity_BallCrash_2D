using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class BallHandler : MonoBehaviour
{

    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private Rigidbody2D pivot;
    [SerializeField] private float detachDelay;
    [SerializeField] private float respawnDelay;


    private Rigidbody2D currentBallRigidBody;
    private SpringJoint2D currentBallSpringJoint;
    
    private Camera mainCamera;
    private bool isDragging;



    // Start is called before the first frame update
    void Start()
    {
        // Find and get ref to the main camera in unity
        mainCamera = Camera.main;
        // Spawn in a new ball to start the game
        SpawnNewBall();
    }

    // OnEnable() & OnDisable() used for multi-touch
    private void OnEnable()
    {
        EnhancedTouchSupport.Enable();
    }

    private void OnDisable()
    {
        EnhancedTouchSupport.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        // This states if we DONT have ball don't run any of the code anymore
        if (currentBallRigidBody == null)
        {
            return;
        }

        // Since our finger may not be down of the frame it checks it needs to do a check first to see if our finger is even down first
        // So, if touchscreen is not pressed then do "return" but if pressed go to the next lines of code in the Update function
        
        // if (!Touchscreen.current.primaryTouch.press.isPressed) ** Way to go if only using SINGLE TOUCH
        
        if(Touch.activeTouches.Count == 0) //This says if nothing is being touched do what it says in {..} curly braces
        {
            if (isDragging)
            {
                LaunchBall();
            }

            isDragging = false;

            //currentBallRigidBody.isKinematic = false;
            // Return says if we get here dont run anything underneath; In this instance jump out of this function(Update()) for this frame
            return;
        }

        isDragging = true;
        // Sets so that physics is not being applied to it
        currentBallRigidBody.isKinematic = true;


        // Initialize a Vector2 touchPositioN as a new Vector2 which is (0,0) bc thats what all default vector2's start as
        Vector2 touchPosition = new Vector2();


        // The "foreach" is adding all the touches together and finding the middle point
        foreach(Touch touch in Touch.activeTouches)
        {
            // This is just adding each touchPosition. So if there is only 1 touch then we just have that touchPosition if we have 3 we will have 
            // each touchPosition for all 3 touches. aka 3 Vector2 coordinates 1 for each touch
            touchPosition += touch.screenPosition;
        }

        // Then we will take all the touches and divide them to get the center position between them to give us our position
        touchPosition /= Touch.activeTouches.Count;


        // "current" is the touchscreen on the phone; "primaryTouch" is first finger that touches down; "ReadValue()" converts it into a Vector2 bc they use something
        // called Vector2Control which is samething just for mobile; Then we store this in a var "touchPosition"

        // This basically says every frame get our touch and give us the position for that touch and STORE IT IN THE VARIABLE
        /* The code the line below is for SINGLE TOUCH POSITIONS */
        // Vector2 touchPosition = Touchscreen.current.primaryTouch.position.ReadValue();


        /*-------------------------------------- MULTI-TOUCH ---------------------------------- */
        // Can use the following code for "touches" but it is an array and has to be an array of 10. So here is an example of how you would start that
        // "Touchscreen.current.touches..." then you would finish filling in the rest of the line but instead Unity Has a BETTER way called ENHANCED TOUCH

        /* ENHANCED TOUCH must be turned on and off by us. So will use the "OnEnable() & OnDisable()" fx right below start */


        // convert from touch to world space. Unity has a built in system of this thru camera. So get ref to camera to use; Pass in touch position which is a vec 2 which
        // still works bc just add 0 to z. Then create a vec3 var "worldPosition" bc thats what "ScreenToWOrldPoint" is
        Vector3 worldPostion = mainCamera.ScreenToWorldPoint(touchPosition);


        currentBallRigidBody.position = worldPostion;

    }



     private void SpawnNewBall()
    {
        // We are Instantiating the "ballPrefab" @ the pivot but we need its position and then Quaternion.identity just means no rotation
        // Instantiate returns a GameObj (the ballPrefab) so we set it equal to a var
        GameObject ballInstance = Instantiate(ballPrefab, pivot.position, Quaternion.identity);

        // Next we need to get the Rigidbody and SpringJoint for the ball prefab so we set them to the ballInstances.GetComponent<ComponentNeeded>();
        currentBallRigidBody = ballInstance.GetComponent<Rigidbody2D>();
        currentBallSpringJoint = ballInstance.GetComponent<SpringJoint2D>();

        // Now we need to set the SpringJoints 'connectedBody' by code instead of in the Inspector this time by...
        currentBallSpringJoint.connectedBody = pivot;
    }

    
    
    


    private void LaunchBall()
    {
        // Make the ball react to Physics
        currentBallRigidBody.isKinematic = false;
        // This will clear the rigidbody (When ball is launched) so we can't reset the rigidbody if we touch the screen again
        currentBallRigidBody = null;


        /* NEED A DELAY BETWEEN THESE 2 PORTIONS */
        Invoke("DetachBall", detachDelay);    // Can also call it like "Invoke(nameOf(DetachBall), delayDuration);

        
    }

    private void DetachBall()
    {
        // Turn of the spring joint so it wont be pulled anymore
        currentBallSpringJoint.enabled = false;
        // Clearing the SpringJoint so that it wont re-enable
        currentBallSpringJoint = null;

        Invoke(nameof(SpawnNewBall), respawnDelay);
    }
}
