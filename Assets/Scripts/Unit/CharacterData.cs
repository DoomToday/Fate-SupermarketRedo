using System.Collections.Generic;
using UnityEngine;

public class CharacterData : MonoBehaviour 
{
    public Animator animator;
    public List<string> phrases;
    public List<int> requiredItems = new List<int>(); //list of item IDs that this character may require
}
