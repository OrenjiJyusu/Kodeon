using System;
using System.IO;
using SQLite;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DBscripts : MonoBehaviour
{
    [Header("Login Inputs")]
    public TMP_InputField loginUsernameInput;
    public TMP_InputField loginPasswordInput;

    [Header("Register Inputs")]
    public TMP_InputField registerUsernameInput;
    public TMP_InputField registerPasswordInput;
    public TMP_InputField firstNameInput;
    public TMP_InputField lastNameInput;
    public TMP_InputField courseInput;
    public TMP_InputField sectionInput;
    public TMP_InputField yearLevelInput;

    [Header("UI Panels")]
    public GameObject loginPage;
    public GameObject registerPage;

    [Header("UI Text")]
    public TextMeshProUGUI LFieldChecker;
    public TextMeshProUGUI RFieldChecker;

    private string dbPath;

    void Start()
    {
        //Database path
        dbPath = @"D:\SqlLite(Unity)\LastRegister.db";

        //Directory
        string dir = Path.GetDirectoryName(dbPath);
        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        // If DB does not exist, create it
        if (!File.Exists(dbPath))
        {
            // Create an empty database file
            File.Create(dbPath).Close();
        }

        // Ensure table exists
        using (var db = new SQLiteConnection(dbPath))
        {
            db.CreateTable<Registration>();
        }

        loginPage?.SetActive(true);
        registerPage?.SetActive(false);
        LFieldChecker.text = "Please Login or Register.";

    }

    public void RegisterUser()
    {
        if (string.IsNullOrEmpty(registerUsernameInput.text) ||
            string.IsNullOrEmpty(registerPasswordInput.text) ||
            string.IsNullOrEmpty(firstNameInput.text) ||
            string.IsNullOrEmpty(lastNameInput.text) ||
            string.IsNullOrEmpty(courseInput.text) ||
            string.IsNullOrEmpty(sectionInput.text) ||
            string.IsNullOrEmpty(yearLevelInput.text))
        {
            RFieldChecker.text = "Please fill in all fields.";
            return;
        }

        using (var db = new SQLiteConnection(dbPath))
        {
            // Check if username exists
            var existingUser = db.Table<Registration>()
                .Where(u => u.UserName == registerUsernameInput.text)
                .FirstOrDefault();

            if (existingUser != null)
            {
                RFieldChecker.text = "Username already taken!";
                return;
            }

            Registration newUser = new Registration()
            {
                UserName = registerUsernameInput.text,
                Password = registerPasswordInput.text,
                FirstName = firstNameInput.text,
                LastName = lastNameInput.text,
                Course = courseInput.text,
                Section = sectionInput.text,
                YearLevel = yearLevelInput.text
            };

            db.Insert(newUser);
        }

        LFieldChecker.text = "Registration successful! You can now login.";

        ClearRegisterFields();
        GoToLogin();
    }

    public void LoginUser()
    {
        if (string.IsNullOrEmpty(loginUsernameInput.text) ||
            string.IsNullOrEmpty(loginPasswordInput.text))
        {
            LFieldChecker.text = "Enter username and password.";
            return;
        }

        using (var db = new SQLiteConnection(dbPath))
        {
            var user = db.Table<Registration>()
                .Where(u => u.UserName == loginUsernameInput.text &&
                            u.Password == loginPasswordInput.text)
                .FirstOrDefault();

            if (user != null)
            {
                ClearLoginFields();
                SceneManager.LoadScene("Main Menu");
            }
            else
            {
                LFieldChecker.text = "Invalid username or password.";
            }
        }
    }

    public void GoToRegister()
    {
        loginPage.SetActive(false);
        registerPage.SetActive(true);
    }

    public void GoToLogin()
    {
        registerPage.SetActive(false);
        loginPage.SetActive(true);
    }
    private void ClearRegisterFields()
    {
        registerUsernameInput.text = "";
        registerPasswordInput.text = "";
        firstNameInput.text = "";
        lastNameInput.text = "";
        courseInput.text = "";
        sectionInput.text = "";
        yearLevelInput.text = "";
    }

    private void ClearLoginFields()
    {
        loginUsernameInput.text = "";
        loginPasswordInput.text = "";
    }
}

//DATABASE MODEL
public class Registration
{
    [PrimaryKey, AutoIncrement]
    public int ID { get; set; }

    public string UserName { get; set; }
    public string Password { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Course { get; set; }
    public string Section { get; set; }
    public string YearLevel { get; set; }
}