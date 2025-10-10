using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class ButtonDelay : MonoBehaviour
{
    [SerializeField] private Button sellItemButton;
    [SerializeField] private Button finishButton;
    [SerializeField] private SupermarketManager manager;
    [SerializeField] private float delay = 7f;

    private bool canPress = true;

    public void OnStartShiftPressed()
    {
        if (!canPress) return;

        manager.CallNextBuyer();

        StartCoroutine(ButtonCooldown());
    }

    public void OnSellItemPressed()
    {
        if (!canPress) return;

        manager.FinishCurrentBuyer();

        StartCoroutine(ButtonCooldown());
    }

    private IEnumerator ButtonCooldown()
    {
        canPress = false;
        sellItemButton.interactable = false;
        finishButton.interactable = false;
        yield return new WaitForSeconds(delay);

        canPress = true;
        sellItemButton.interactable = true;
        finishButton.interactable = true;
    }
}