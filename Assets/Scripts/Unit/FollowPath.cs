using System;
using System.Collections.Generic;
using UnityEngine;

public class FollowPath : MonoBehaviour
{
    private const float MIN_DISTANCE_DIFFERENCE = 0.1f;
    private const float MIN_ROTATION_DIFFERENCE = 0.001f;

    private List<Transform> waypoints = new List<Transform>();

    [SerializeField] private GameObject waypointsParent; // Parent object containing all waypoint children

    private int currentWaypointIndex = 0;
    private int currentDirection = 1; // 1 for forward, -1 for backward

    private float speed = 5f;
    private float rotationSpeed = 5f;

    private bool waitingForTrigger = true;
    private bool passedOnce = false;

    public event Action OnReachedEnd;

    [SerializeField] private Transform finalTarget;

    [SerializeField] private Vector3 forwardOffset = new Vector3(0, 0, 0); // Offset in case problems with facing direction

    void Start()
    {
        if (waypointsParent != null)
        {
            waypoints.Clear();
            foreach (Transform child in waypointsParent.transform)
            {
                waypoints.Add(child);
            }
        }


    }

    void Update()
    {
        if (!waitingForTrigger)
        {
            MoveAlongPath();
        }
        else
        {
            RotateToTarget(finalTarget);
        }
    }

    private void MoveAlongPath()
    {
        if (waypoints == null || waypoints.Count == 0) return;

        Transform targetWaypoint = waypoints[currentWaypointIndex];

        // Move towards target waypoint
        transform.position = Vector3.MoveTowards(transform.position, targetWaypoint.position, speed * Time.deltaTime);

        // Rotate smoothly towards target
        Vector3 direction = targetWaypoint.position - transform.position;
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        if (Vector3.Distance(transform.position, targetWaypoint.position) < MIN_DISTANCE_DIFFERENCE)
        {
            currentWaypointIndex += currentDirection;
            if (currentDirection == 1 && currentWaypointIndex >= waypoints.Count)
            {
                currentWaypointIndex = waypoints.Count - 1;
                passedOnce = true;
                waitingForTrigger = true; // Wait for external trigger
                OnReachedEnd?.Invoke();
            }
            if (currentDirection == -1 && currentWaypointIndex < 0 && passedOnce)
            {
                currentWaypointIndex = 0;
                enabled = false;
                transform.gameObject.SetActive(false);
            }
        }
    }
    public void Trigger(int dir)
    {
        if (waitingForTrigger)
        {
            waitingForTrigger = false;
            currentDirection = dir;
        }
    }

    private void RotateToTarget(Transform target)
    {
        if (target == null) return;

        Vector3 direction = target.position - transform.position;
        direction.y = 0; // ignore vertical difference
        if (direction.sqrMagnitude > MIN_ROTATION_DIFFERENCE)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            targetRotation *= Quaternion.Euler(forwardOffset); // Apply offset if needed
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
}
