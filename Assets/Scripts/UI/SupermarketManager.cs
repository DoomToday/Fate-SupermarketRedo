using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class SupermarketManager : MonoBehaviour
{
    private const int NO_CHOSEN_ITEM = -1;
    private const int FORWARD_DIRECTION = 1;
    private const int BACKWARD_DIRECTION = -1;

    private List<GameObject> allBuyersList = new List<GameObject>();
    private List<Outline> allGlowsList = new List<Outline>();
    private List<GameObject> availableBuyers = new List<GameObject>();
    private GameObject activeBuyer;

    [SerializeField] int timeForBuyer = 60;
    [SerializeField] int timeBeforeNewBuyer = 2;
    [SerializeField] int scoreScale = 100;

    private int score = 0;
    private float countdown;

    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] GameObject sellButton;

    private int chosenItemIndex = -1;

    [SerializeField] private GameObject allBuyers;
    [SerializeField] private GameObject allGlows;

    [SerializeField] private Dialogue dialogue;
    [SerializeField] private AudioSource CashRegisterSound;

    private Coroutine timerCoroutine;

    public static event Action gameFinished;

    void Start()
    {
        PauseMenu.gameUnpaused += TrySpeak;
        countdown = timeForBuyer;

        if (allBuyers != null)
        {
            allBuyersList.Clear();
            foreach (Transform buyer in allBuyers.transform)
            {
                allBuyersList.Add(buyer.gameObject);
                buyer.gameObject.SetActive(false);
            }
        }

        if (allGlows != null)
        {
            allGlowsList.Clear();
            foreach (Transform glow in allGlows.transform)
            {
                Debug.Log(glow);
                allGlowsList.Add(glow.GetComponent<Outline>());
            }

            foreach (var glow in allGlowsList)
            {
                glow.enabled = false;
            }
        }
    }

    public void CallNextBuyer()
    {
        if (availableBuyers.Count == 0)
        {
            Debug.Log("All buyers have been used, resetting...");
            ResetBuyerPool();
            return;
        }

        // Pick a random buyer
        int randomIndex = UnityEngine.Random.Range(0, availableBuyers.Count);
        activeBuyer = availableBuyers[randomIndex];
        availableBuyers.RemoveAt(randomIndex);

        activeBuyer.GetComponent<FollowPath>().ResetPath();
        activeBuyer.SetActive(true);
        activeBuyer.GetComponent<FollowPath>().OnReachedEnd += OnReachedEndTrigger;
        activeBuyer.GetComponent<FollowPath>().Trigger(FORWARD_DIRECTION);
    }

    private void ResetBuyerPool()
    {
        availableBuyers = new List<GameObject>(allBuyersList);
        ShuffleList(availableBuyers);
        activeBuyer = null;

        foreach (var buyer in allBuyersList)
            buyer.SetActive(false);

        CallNextBuyer();
    }

    private void ShuffleList<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int rand = UnityEngine.Random.Range(i, list.Count);
            (list[i], list[rand]) = (list[rand], list[i]);
        }
    }

    public void FinishCurrentBuyer()
    {
        if (activeBuyer == null)
        {
            Debug.Log("No active buyer to finish.");
            return;
        }

        if (activeBuyer.GetComponent<CharacterData>().requiredItems.Contains(chosenItemIndex))
        {
            BuyerSpeak((int)Constants.PhraseType.Satisfied);
            score += scoreScale;
        }
        else if (chosenItemIndex == NO_CHOSEN_ITEM)
        {
            BuyerSpeak((int)Constants.PhraseType.NoItem);
            score -= scoreScale;
        }
        else
        {
            BuyerSpeak((int)Constants.PhraseType.Disappointed);
            score -= scoreScale;
        }

        UpdateScore();
        StopTimer();

        CashRegisterSound.Play();

        activeBuyer.GetComponent<FollowPath>().Trigger(BACKWARD_DIRECTION);
        Debug.Log(activeBuyer);
        chosenItemIndex = NO_CHOSEN_ITEM;
        DisableAllGlows();
    }

    private void OnReachedEndTrigger(bool forward)
    {
        if (forward)
        {
            BuyerSpeak((int)Constants.PhraseType.Buying);
            StartTimer();
            sellButton.SetActive(true);
        }
        else
        {
            dialogue.Clear();
            activeBuyer.GetComponent<FollowPath>().OnReachedEnd -= OnReachedEndTrigger;
            activeBuyer.SetActive(false);
            activeBuyer = null;
            StartCoroutine(WaitBeforeNewBuyer(timeBeforeNewBuyer));
        }
    }

    public void TrySpeak()
    {
        if (activeBuyer != null)
        {
            BuyerSpeak((int)Constants.PhraseType.Buying);
        }
    }

    public void BuyerSpeak(int phraseID)
    {
        dialogue.Say(
            activeBuyer.GetComponent<CharacterData>().phrases.ElementAt(phraseID),
            activeBuyer.GetComponent<CharacterData>().phrases.ElementAt(Constants.NAME)
        );
    }

    public void SetChosenItem(int index)
    {
        chosenItemIndex = index;
    }

    public void DisableAllGlows()
    {
        foreach (var glow in allGlowsList)
        {
            glow.enabled = false;
        }
    }

    public void UpdateScore()
    {
        scoreText.text = "Score: " + score;
    }

    public void StartTimer()
    {
        if (timerCoroutine != null)
            StopCoroutine(timerCoroutine);

        countdown = timeForBuyer;
        timerCoroutine = StartCoroutine(TimerRoutine());
    }

    public void StopTimer()
    {
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
            timerText.text = "";
            timerCoroutine = null;
        }
    }

    private IEnumerator TimerRoutine()
    {
        while (countdown > 0)
        {
            countdown -= Time.deltaTime;
            UpdateTimerUI(countdown);
            yield return null;
        }

        countdown = 0;
        UpdateTimerUI(countdown);
        chosenItemIndex = NO_CHOSEN_ITEM;
        FinishCurrentBuyer();
    }

    private IEnumerator WaitBeforeNewBuyer(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        CallNextBuyer();
    }

    private void UpdateTimerUI(float time)
    {
        timerText.text = $"Time: {Mathf.FloorToInt(time % 60):00}";
    }

    public void EndGame()
    {
        gameFinished?.Invoke();
    }
}
