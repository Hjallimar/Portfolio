using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Author: Hjalmar Andersson

[CreateAssetMenu(menuName = "Chapter")]
public class ChapterForBook : ScriptableObject
{

    [SerializeField] private string title;
    public string Title { get { return title; } }

    [TextArea(1, 20)][SerializeField] private string story;
    public string Story { get { return story; } }
    // Start is called before the first frame update
}
