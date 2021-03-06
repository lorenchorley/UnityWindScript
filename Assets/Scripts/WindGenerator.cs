﻿using System.Collections.Generic;
using UnityEngine;

/*
 * Written by Loren Chorley (https://github.com/lorenchorley)
 * Usage: Attach to any object. Any object that is to be affected by the wind should be added to the AffectedObjects list.
 */
public class WindGenerator : MonoBehaviour {

    // Time to transition between states
    public float transitionTime = 1;

    // Maximum random purturbation
    public float maxTurbulence = 1;

    // Min and max values of speed and direction change
    // as well as the wait period before the next update
    public float minDirectionWait = 2;
    public float maxDirectionWait = 10;
    public float minDirectionChange = 0;
    public float maxDirectionChange = 1;

    public float minSpeed = 1;
    public float maxSpeed = 2;
    public float minSpeedWait = 1;
    public float maxSpeedWait = 5;

    // Transitioning values
    private float? startedDirectionChange = null;
    private Vector3 InitialDirection;
    private Vector3 TargetDirection;

    private float? startedSpeedChange = null;
    private float InitialSpeed;
    private float TargetSpeed;

    // Final usable values
    public Vector3 windDirection = Vector3.forward;
    public float windSpeed;
    public Vector3 windVelocity;

    // Count down timers (in seconds)
    private float directionTimer;
    private float speedTimer;

    public List<Rigidbody> AffectedObjects;

    void Update() {

        // Change direction or speed if time is up
        if (directionTimer <= 0)
            ChangeDirection();
        if (speedTimer <= 0)
            ChangeSpeed();

        // Transition between old and new values of direction and speed
        if (startedSpeedChange.HasValue) {
            float speedProgress = (Time.time - startedSpeedChange.Value) / transitionTime;
            if (speedProgress >= 1) {
                startedSpeedChange = null;
                windSpeed = TargetSpeed;
            } else {
                windSpeed = Mathf.SmoothStep(InitialSpeed, TargetSpeed, speedProgress);
            }
            windVelocity = windDirection * windSpeed;
        }

        if (startedDirectionChange.HasValue) {
            float directionProgress = (Time.time - startedDirectionChange.Value) / transitionTime;
            if (directionProgress >= 1) {
                startedDirectionChange = null;
                windDirection = TargetDirection;
            } else {
                windDirection = Vector3.Lerp(InitialDirection, TargetDirection, directionProgress);
            }
            windVelocity = windDirection * windSpeed;
        }

        // Decrement counters
        directionTimer -= 1 * Time.deltaTime;
        speedTimer -= 1 * Time.deltaTime;

        // Apply impulses to all affected objects
        foreach (Rigidbody rb in AffectedObjects)
            rb.AddForce(GenerateImpulseForce(), ForceMode.Impulse);

    }

    public Vector3 GenerateImpulseForce() {
        return windVelocity + Random.insideUnitSphere * windSpeed * maxTurbulence;
    }

    void ChangeDirection() {

        // Generate random adjustment in euler angles
        float directionAdjustX = Random.Range(minDirectionChange, maxDirectionChange);
        float directionAdjustY = Random.Range(minDirectionChange, maxDirectionChange);
        float directionAdjustZ = Random.Range(minDirectionChange, maxDirectionChange);

        // Start transition
        InitialDirection = windDirection;
        TargetDirection = Quaternion.Euler(directionAdjustX, directionAdjustY, directionAdjustZ) * windDirection;

        // Reset timer
        directionTimer = Random.Range(minDirectionWait, maxDirectionWait);

    }

    void ChangeSpeed() {

        // Generate random wind speed and start transition
        InitialSpeed = windSpeed;
        TargetSpeed = Random.Range(minSpeed, maxSpeed);
        startedSpeedChange = Time.time;

        // Reset timer
        speedTimer = Random.Range(minSpeedWait, maxSpeedWait);

    }

}
