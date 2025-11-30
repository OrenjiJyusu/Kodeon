using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.IO;

public class JavaQuiz : MonoBehaviour
{
    [Header("UI References")]
    public TMP_InputField codeInput;
    public TMP_Text outputText;
    public Button submitButton;
    public GameObject QA;

    [Header("Padlock Reference")]
    public PadlockQ padlockQ;   // <<— Added this

    [Header("JDK Settings")]
    public string jdkStreamingAssetsPath = "JDK8/jdk8u472-lite"; // Relative to StreamingAssets

    private JavaExecutor javaExecutor;
    private string expectedOutput = "15";

    void Start()
    {
        // Initialize JavaExecutor
        string jdkPath = Path.Combine(Application.streamingAssetsPath, jdkStreamingAssetsPath);
        javaExecutor = new JavaExecutor(jdkPath);

        // Initialize UI
        submitButton.interactable = false;
        codeInput.onValueChanged.AddListener(OnCodeChanged);

        // Prefill template Java code
        codeInput.text =
@"public class MyClass {
    public static void main(String[] args) {

    }
}";

        submitButton.onClick.AddListener(OnSubmit);
    }

    private void OnCodeChanged(string text)
    {
        // Enable submit button only if code field is not empty
        submitButton.interactable = !string.IsNullOrWhiteSpace(text);
    }

    public void OnSubmit()
    {
        string javaFilePath = Path.Combine(Application.persistentDataPath, "MyClass.java");
        File.WriteAllText(javaFilePath, codeInput.text);

        string compileErrors = javaExecutor.CompileJava(javaFilePath);
        if (!string.IsNullOrEmpty(compileErrors))
        {
            outputText.text = "Compile Error:\n" + compileErrors;
            return;
        }

        string output = javaExecutor.RunJava("MyClass", Application.persistentDataPath);

        if (output.Trim() == expectedOutput)
        {
            Debug.Log("PASS!");

            // Use PadlockQ to properly unlock and close panel
            padlockQ.ClosePanel();            // <<— This fixes the freeze!

            // Hide the compiler UI
            QA.SetActive(false);
        }
        else
        {
            outputText.text = "FAIL!\nYour output: " + output + "\nExpected: " + expectedOutput;
        }
    }
}
