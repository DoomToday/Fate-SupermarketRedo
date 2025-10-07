using UnityEngine;

public class MovementChecker : MonoBehaviour
{
    [SerializeField] private Animator animator;
    private Vector3 lastPosition;
    public float movementThreshold = 0.001f;
    public bool isMoving { get; private set; }

    void Start()
    {
        lastPosition = transform.position;
    }

    void Update()
    {
        float distance = Vector3.Distance(transform.position, lastPosition);
        isMoving = distance > movementThreshold;
        lastPosition = transform.position;
        if (isMoving)
        {
            animator.SetBool("isWalking", true);
        }
        else
        {
            animator.SetBool("isWalking", false);
        }
    }
}
