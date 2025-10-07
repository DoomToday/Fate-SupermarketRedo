using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHover : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] private float targetAngle;
    [SerializeField] private GameObject secondButton;
    [SerializeField] private GameObject itemChoiceUI;
    [SerializeField] private CameraRotation camera;
    [SerializeField] private bool showUI;
    public void OnPointerEnter(PointerEventData eventData)
    {
        camera.SetTargetAngle(targetAngle);
        secondButton.SetActive(true);
        gameObject.SetActive(false);
        if (showUI)
        {
            itemChoiceUI.SetActive(true);
        }
        else
        {
            itemChoiceUI.SetActive(false);
        }
    }
}
