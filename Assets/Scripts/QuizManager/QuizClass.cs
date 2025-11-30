using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Question
{
    public string questionText;
    public string[] answers;   // Array of 4 answers (A, B, C, D)
    public int correctIndex;   // 0 = A, 1 = B, 2 = C, 3 = D
}
