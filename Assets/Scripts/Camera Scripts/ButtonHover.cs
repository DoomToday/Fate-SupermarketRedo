using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHover : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] private float targetAngle;
    [SerializeField] private GameObject secondButton;
    [SerializeField] private GameObject itemChoiceUI;
    [SerializeField] private GameObject buyerSendCallUI;
    [SerializeField] private CameraRotation camera;
    [SerializeField] private bool showUI;
    public void OnPointerEnter(PointerEventData eventData)
    {
        camera.SetTargetAngle(targetAngle);
        secondButton.SetActive(true);
        gameObject.SetActive(false);
        itemChoiceUI.SetActive(showUI ? true : false);
        buyerSendCallUI.SetActive(showUI ? false : true);
    }
}
