using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class SupermarketManager : MonoBehaviour
{
    private const int NO_CHOSEN_ITEM = -1;
    private const int forwardDirection = 1;
    private const int backwardDirection = -1;
    private List<GameObject> allBuyersList = new List<GameObject>();
    private List<GameObject> allFramesList = new List<GameObject>();
    private Queue<GameObject> buyerQueue = new Queue<GameObject>();
    private GameObject activeBuyer;
    [SerializeField] int timeForBuyer = 60;
    [SerializeField] int scoreScale = 100;

    private int score = 0;
    private float countdown;

    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI timerText;

    private int chosenItemIndex = -1;

    [SerializeField] private GameObject allBuyers;
    [SerializeField] private GameObject allFrames;

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
            foreach (Transform child in allBuyers.transform)
            {
                Debug.Log(child);
                allBuyersList.Add(child.gameObject);
            }
            foreach (var buyer in allBuyersList)
            {
                buyer.SetActive(false);
                buyerQueue.Enqueue(buyer);
            }
        }
        if (allFrames != null)
        {
            allFramesList.Clear();
            foreach (Transform child in allFrames.transform)
            {
                Debug.Log(child);
                allFramesList.Add(child.gameObject);
            }
            foreach (var frame in allFramesList)
            {
                frame.SetActive(false);
            }
        }

    }
    public void CallNextBuyer()
    {
        if (activeBuyer != null)
        {
            Debug.Log("Current buyer is still active!");
            return;
        }

        if (buyerQueue.Count == 0)
        {
            Debug.Log("No more buyers in the queue.");
            gameFinished?.Invoke();
            return;
        }

        activeBuyer = buyerQueue.Dequeue();
        activeBuyer.SetActive(true);

        activeBuyer.GetComponent<FollowPath>().OnReachedEnd += OnReachedEndTrigger;

        activeBuyer.GetComponent<FollowPath>().Trigger(forwardDirection);
    }

    public void FinishCurrentBuyer()
    {
        if (activeBuyer == null)
        {
            Debug.Log("No active buyer to finish.");
            return;
        }

        if(chosenItemIndex == activeBuyer.GetComponent<CharacterData>().requiredItem1 || chosenItemIndex == activeBuyer.GetComponent<CharacterData>().requiredItem2 || chosenItemIndex == activeBuyer.GetComponent<CharacterData>().requiredItem3)
        {
            BuyerSpeak(activeBuyer.GetComponent<CharacterData>().phraseSatisfied);
            score += scoreScale;
        }
        else
        {
            BuyerSpeak(activeBuyer.GetComponent<CharacterData>().phraseDisappointed);
            score -= scoreScale;
        }

        UpdateScore();
        StopTimer();

        CashRegisterSound.Play();

        activeBuyer.GetComponent<FollowPath>().Trigger(backwardDirection);
        activeBuyer = null;

        chosenItemIndex = NO_CHOSEN_ITEM;
        DisableAllFrames();
    }

    private void OnReachedEndTrigger() 
    {
        BuyerSpeak(activeBuyer.GetComponent<CharacterData>().phraseBuying);
        StartTimer();
    }

    public void TrySpeak()
    {
        if(activeBuyer != null)
        {
            BuyerSpeak(activeBuyer.GetComponent<CharacterData>().phraseBuying);
        }
    }

    public void BuyerSpeak(string phrase)
    {
         dialogue.Say(phrase, activeBuyer.GetComponent<CharacterData>().characterName);
    }
    public void SetChosenItem(int index)
    {
        chosenItemIndex = index;
    }
    public void DisableAllFrames()
    {
        foreach(var frame in allFramesList)
        {
            frame.SetActive(false);
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

    private void UpdateTimerUI(float time)
    {
        timerText.text = $"Time: {Mathf.FloorToInt(time % 60):00}";
    }
}