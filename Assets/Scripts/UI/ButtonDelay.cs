using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class ButtonDelay : MonoBehaviour
{
    [SerializeField] private Button nextBuyerButton;
    [SerializeField] private Button sendBackButton;
    [SerializeField] private SupermarketManager manager;
    [SerializeField] private float delay = 7f;

    private bool canPress = true;

    public void OnNextBuyerPressed()
    {
        if (!canPress) return;

        manager.CallNextBuyer();

        StartCoroutine(ButtonCooldown());
    }

    public void OnSendBackPressed()
    {
        if (!canPress) return;

        manager.FinishCurrentBuyer();

        StartCoroutine(ButtonCooldown());
    }

    private IEnumerator ButtonCooldown()
    {
        canPress = false;
        nextBuyerButton.interactable = false;
        sendBackButton.interactable = false;

        yield return new WaitForSeconds(delay);

        canPress = true;
        nextBuyerButton.interactable = true;
        sendBackButton.interactable = true;
    }
}