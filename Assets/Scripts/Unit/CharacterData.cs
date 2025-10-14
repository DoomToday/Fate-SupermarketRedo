using System.Collections.Generic;
using UnityEngine;

public class CharacterData : MonoBehaviour 
{
    public Animator animator;
    public List<string> phrases;
    public List<int> requiredItems = new List<int>(); //list of item IDs that this character may require
    public List<PhraseSet> phraseSets = new List<PhraseSet>();
    private void OnEnable()
    {
        List<PhraseSet> available = phraseSets.FindAll(set => !set.hasBeenUsed);
        if (available.Count == 0)
        {
            Debug.Log($"{name}: No unused phrase sets left!");
            ResetUsedSets();
            available = phraseSets.FindAll(set => !set.hasBeenUsed);
        }

        int index = Random.Range(0, available.Count);
        PhraseSet currentPhraseSet = available[index];
        currentPhraseSet.hasBeenUsed = true;

        phrases = currentPhraseSet.phrases;
        requiredItems = currentPhraseSet.requiredItems;
    }

    public void ResetUsedSets()
    {
        foreach (var set in phraseSets)
            set.hasBeenUsed = false;
    }
}
