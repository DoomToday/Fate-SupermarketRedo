using UnityEngine;

public class CharacterData : MonoBehaviour
{
    public Animator animator;
    public string characterName;
    public string phraseBuying;
    public string phraseSatisfied;
    public string phraseDisappointed;
    public int requiredItem1;
    public int requiredItem2; //put in -1 if no second or third item is required
    public int requiredItem3;
}
