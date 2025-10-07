using UnityEngine;

public class CameraRotation : MonoBehaviour
{
    float r;
    [SerializeField] private float targetAngle;

    private void Update()
    {
        RotateAngle();
    }
    private void RotateAngle()
    {
        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref r, 0.3f);
        transform.rotation = Quaternion.Euler(0, angle, 0);
    }
    public void SetTargetAngle(float angle)
    {
        targetAngle = angle;
    }
}
