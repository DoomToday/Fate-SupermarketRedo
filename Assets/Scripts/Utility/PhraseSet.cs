using System.Collections.Generic;

[System.Serializable]
public class PhraseSet
{
    public List<string> phrases = new List<string>();
    public List<int> requiredItems = new List<int>();
    public bool hasBeenUsed = false;
}
