using System;
using System.Collections.Generic;
using UnityEngine;

public class FollowPath : MonoBehaviour
{
    // --- Constants ---
    private const float MIN_DISTANCE_TO_WAYPOINT = 0.1f;
    private const float MIN_ROTATION_DIFFERENCE = 0.001f;

    // --- Events ---
    public event Action<bool> OnReachedEnd;

    // --- Inspector Fields ---
    [Header("Path Settings")]
    [SerializeField] private GameObject waypointsParent; // Parent object containing all waypoint children
    [SerializeField] private Transform finalTarget; // Target to look at when waiting at the counter
    [SerializeField] private Vector3 forwardOffset = Vector3.zero; // Rotational offset

    [Header("Movement Settings")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float rotationSpeed = 5f;

    // --- State Variables ---
    private List<Transform> waypoints = new List<Transform>();
    private int currentWaypointIndex;
    private int currentDirection; // 1 for forward, -1 for backward
    private bool isMoving;

    void Awake() // Use Awake to ensure waypoints are ready before Start is called on other scripts
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
        if (isMoving)
        {
            MoveAlongPath();
        }
        else
        {
            if (currentWaypointIndex >= waypoints.Count - 1)
            {
                RotateToTarget(finalTarget);
            }
        }
    }
    public void ResetPath()
    {
        isMoving = false;
        currentWaypointIndex = 0;

        if (waypoints != null && waypoints.Count > 0)
        {
            // Instantly snap to the starting position to be ready for the next run
            transform.position = waypoints[0].position;
        }
    }
    public void Trigger(int direction)
    {
        currentDirection = direction;
        isMoving = true;
    }

    private void MoveAlongPath()
    {
        if (waypoints == null || waypoints.Count == 0) return;

        // Determine the target waypoint based on current progress
        Transform targetWaypoint = waypoints[currentWaypointIndex];

        // Move towards the target
        transform.position = Vector3.MoveTowards(transform.position, targetWaypoint.position, speed * Time.deltaTime);

        // Rotate towards the target
        Vector3 directionToTarget = targetWaypoint.position - transform.position;
        if (directionToTarget != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // Check if we've reached the waypoint
        if (Vector3.Distance(transform.position, targetWaypoint.position) < MIN_DISTANCE_TO_WAYPOINT)
        {
            AdvanceToNextWaypoint();
        }
    }

    private void AdvanceToNextWaypoint()
    {
        currentWaypointIndex += currentDirection;
        if (currentDirection > 0 && currentWaypointIndex >= waypoints.Count)
        {
            currentWaypointIndex = waypoints.Count - 1;
            isMoving = false;
            OnReachedEnd?.Invoke(true);
        }
        else if (currentDirection < 0 && currentWaypointIndex < 0)
        {
            isMoving = false;
            OnReachedEnd?.Invoke(false); 
            gameObject.SetActive(false);
        }
    }

    private void RotateToTarget(Transform target)
    {
        if (target == null) return;

        Vector3 direction = target.position - transform.position;
        direction.y = 0; // Ignore vertical difference for rotation

        if (direction.sqrMagnitude > MIN_ROTATION_DIFFERENCE)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            targetRotation *= Quaternion.Euler(forwardOffset);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
}