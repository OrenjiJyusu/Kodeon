using System.IO;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class JavaExecutor
{
    private string jdkBinPath;

    /// <summary>
    /// Initializes JavaExecutor and ensures the JDK is copied to persistentDataPath.
    /// </summary>
    /// <param name="jdkStreamingPath">Path inside StreamingAssets to your JDK folder</param>
    public JavaExecutor(string jdkStreamingPath)
    {
        SetupJDK(jdkStreamingPath);
    }

    /// <summary>
    /// Copies the JDK from StreamingAssets to persistentDataPath if it doesn't exist.
    /// </summary>
    private void SetupJDK(string sourcePath)
    {
        string targetPath = Path.Combine(Application.persistentDataPath, "JDK8");

        if (!Directory.Exists(targetPath))
        {
            Directory.CreateDirectory(targetPath);
            CopyDirectory(sourcePath, targetPath);
            Debug.Log("Copied JDK to: " + targetPath);
        }

        jdkBinPath = Path.Combine(targetPath, "bin");
        Debug.Log("JDK runtime bin path: " + jdkBinPath);
    }

    /// <summary>
    /// Recursively copies a directory.
    /// </summary>
    private void CopyDirectory(string sourceDir, string destDir)
    {
        foreach (var dir in Directory.GetDirectories(sourceDir, "*", SearchOption.AllDirectories))
        {
            Directory.CreateDirectory(dir.Replace(sourceDir, destDir));
        }

        foreach (var file in Directory.GetFiles(sourceDir, "*.*", SearchOption.AllDirectories))
        {
            string destFile = file.Replace(sourceDir, destDir);
            File.Copy(file, destFile, true);
        }
    }

    /// <summary>
    /// Compiles a Java file using javac.exe.
    /// </summary>
    public string CompileJava(string javaFilePath)
    {
        if (!File.Exists(javaFilePath))
            return $"Error: Java file not found: {javaFilePath}";

        string javacPath = Path.Combine(jdkBinPath, "javac.exe");
        if (!File.Exists(javacPath))
            return $"Error: javac.exe not found at: {javacPath}";

        string workingDir = Path.GetDirectoryName(javaFilePath);

        Process p = new Process();
        p.StartInfo.FileName = javacPath;
        p.StartInfo.Arguments = $"\"{javaFilePath}\"";
        p.StartInfo.WorkingDirectory = workingDir;
        p.StartInfo.UseShellExecute = false;
        p.StartInfo.RedirectStandardError = true;
        p.StartInfo.CreateNoWindow = true;

        p.Start();
        string errors = p.StandardError.ReadToEnd();
        p.WaitForExit();

        return errors;
    }

    /// <summary>
    /// Runs a compiled Java class using java.exe.
    /// </summary>
    public string RunJava(string className, string workingDir)
    {
        string javaPath = Path.Combine(jdkBinPath, "java.exe");
        if (!File.Exists(javaPath))
            return $"Error: java.exe not found at: {javaPath}";

        Process p = new Process();
        p.StartInfo.FileName = javaPath;
        p.StartInfo.WorkingDirectory = workingDir;
        p.StartInfo.Arguments = className;
        p.StartInfo.UseShellExecute = false;
        p.StartInfo.RedirectStandardOutput = true;
        p.StartInfo.RedirectStandardError = true;
        p.StartInfo.CreateNoWindow = true;

        p.Start();
        string output = p.StandardOutput.ReadToEnd();
        string errors = p.StandardError.ReadToEnd();
        p.WaitForExit();

        if (!string.IsNullOrEmpty(errors))
            return "Runtime Error:\n" + errors;

        return output;
    }
}