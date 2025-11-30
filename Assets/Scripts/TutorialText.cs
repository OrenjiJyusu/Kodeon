using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TutorialText : MonoBehaviour
{
    
    public TMP_Text tutorialText;

    void Start()
    {
        TextAsset textFile = Resources.Load<TextAsset>("Tutorials/DataTypesTable");
        tutorialText.text = textFile.text;
    }
}

