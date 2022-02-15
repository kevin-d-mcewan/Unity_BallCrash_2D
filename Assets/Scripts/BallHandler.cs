using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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
        if (!Touchscreen.current.primaryTouch.press.isPressed)
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


        // "current" is the touchscreen on the phone; "primaryTouch" is first finger that touches down; "ReadValue()" converts it into a Vector2 bc they use something
        // called Vector2Control which is samething just for mobile; Then we store this in a var "touchPosition"

        // This basically says every frame get our touch and give us the position for that touch and STORE IT IN THE VARIABLE
        Vector2 touchPosition = Touchscreen.current.primaryTouch.position.ReadValue();

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
