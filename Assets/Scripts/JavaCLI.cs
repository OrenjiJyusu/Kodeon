using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Diagnostics;
using System.IO;
using System;
using System.Threading.Tasks;

public class JavaCLI : MonoBehaviour
{
    public TMP_InputField inputField;
    public TextMeshProUGUI outputText;
    public TMP_InputField userInputField; // New input field for user input
    //public TypingAnimation typingAnimation;

    void Start()
    {
        SetInitialJavaTemplate();
    }

    void SetInitialJavaTemplate()
    {
        inputField.text = 
@"
class TempJavaFile {
    public static void main(String[] args) {
       
    }
}";
        outputText.text = "Output Goes Here";
        userInputField.text = "If using Scanner, enter input here";
    }

    public void ScannerTemplate()
    {
        inputField.text = 
@"import java.util.Scanner; 
class TempJavaFile {
    public static void main(String[] args) {
        Scanner scanner = new Scanner(System.in);
        System.out.println(""Please enter a line of text:"");
        
        if (scanner.hasNextLine()) {
            String line = scanner.nextLine();
            System.out.println(""You entered: "" + line);
        } else {
            System.out.println(""No input was found."");
        }
        
        scanner.close();
    }
}";
        outputText.text = "Template loaded for Scanner. Awaiting user input.";
    }
   public void ControlStructure() {
    inputField.text = 
    @"class TempJavaFile {
        public static void main(String[] args) {
            // The if Statement (One-Way Decision)
            int age = 16;
            if (age >= 18) {
                System.out.println(""You are an adult."");
            }

            // The if-else Statement (Two-Way Decision)
            if (age >= 18) {
                System.out.println(""You are an adult."");
            } else {
                System.out.println(""You are a minor."");
            }

            // Nested if-else Statements (Multiple Decisions)
            int score = 85;

            if (score >= 90) {
                System.out.println(""A"");
            } else if (score >= 80) {
                System.out.println(""B"");
            } else if (score >= 70) {
                System.out.println(""C"");
            } else {
                System.out.println(""F"");
            }

            // The switch Statement (A Quick Look)
            int day = 3;
            String dayName;

            switch (day) {
                case 1:
                    dayName = ""Monday"";
                    break;
                case 2:
                    dayName = ""Tuesday"";
                    break;
                case 3:
                    dayName = ""Wednesday"";
                    break;
                case 4:
                    dayName = ""Thursday"";
                    break;
                case 5:
                    dayName = ""Friday"";
                    break;
                case 6:
                    dayName = ""Saturday"";
                    break;
                case 7:
                    dayName = ""Sunday"";
                    break;
                default:
                    dayName = ""Invalid day"";
                    break;
            }

            System.out.println(""Day "" + day + "" is "" + dayName);
        }
    }";
}

  public void Operators() {
            inputField.text = 
            @"class TempJavaFile {
                public static void main(String[] args) {
                    int add = 5 + 3;       // addition
                    int subtract = 10 - 4; // subtraction
                    int multiply = 6 * 2;  // multiplication
                    int divide = 15 / 3;   // division
                    int modulo = 17 % 5;   // modulo

                    System.out.println(""Arithmetic Operations:"");
                    System.out.println(""5 + 3 = "" + add);
                    System.out.println(""10 - 4 = "" + subtract);
                    System.out.println(""6 * 2 = "" + multiply);
                    System.out.println(""15 / 3 = "" + divide);
                    System.out.println(""17 % 5 = "" + modulo);

                    // Relational Operators
                    boolean isGreater = 10 > 5;
                    boolean isLess = 3 < 7;
                    boolean isEqualOrGreater = 8 >= 8;
                    boolean isEqualOrLess = 4 <= 4;
                    boolean isEqual = 5 == 5;
                    boolean isNotEqual = 2 != 9;

                    System.out.println(""\nRelational Operations:"");
                    System.out.println(""10 > 5: "" + isGreater);
                    System.out.println(""3 < 7: "" + isLess);
                    System.out.println(""8 >= 8: "" + isEqualOrGreater);
                    System.out.println(""4 <= 4: "" + isEqualOrLess);
                    System.out.println(""5 == 5: "" + isEqual);
                    System.out.println(""2 != 9: "" + isNotEqual);

                    // Logical Operators
                    boolean andResult = true && false;     // AND
                    boolean orResult = true || false;      // OR
                    boolean notResult = !true;             // NOT

                    System.out.println(""\nLogical Operations:"");
                    System.out.println(""true && false: "" + andResult);
                    System.out.println(""true || false: "" + orResult);
                    System.out.println(""!true: "" + notResult);

                    // Order of Operations (Precedence)
                    int result = 5 + 3 * 2;                // multiplication before addition
                    int resultWithParentheses = (5 + 3) * 2; // parentheses first

                    System.out.println(""\nOrder of Operations:"");
                    System.out.println(""5 + 3 * 2 = "" + result); 
                    System.out.println(""(5 + 3) * 2 = "" + resultWithParentheses);
                }
            }";
        }

    public void Expression() {
    inputField.text = 
    @"class TempJavaFile {
        public static void main(String[] args) {
            int intExpression = 12 + 5 * 2;            // Integer expression
            double floatExpression = 3.14 * 2;         // Floating-point expression
            System.out.println(""Arithmetic Expressions:"");
            System.out.println(""12 + 5 * 2 = "" + intExpression);
            System.out.println(""3.14 * 2 = "" + floatExpression);

            // Mixed Expression
            double mixedExpression = 10 + 2.5 * 3;     // Mixed expression
            System.out.println(""\nMixed Expression:"");
            System.out.println(""10 + 2.5 * 3 = "" + mixedExpression);

            // Logical Expressions
            boolean greaterThan = 5 > 2;
            boolean lessThan = 10 < 5;
            boolean andExpression = (5 > 2) && (10 > 5);
            boolean orExpression = (5 > 2) || (10 < 5);
            boolean notExpression = !(5 > 2);

            System.out.println(""\nLogical Expressions:"");
            System.out.println(""5 > 2: "" + greaterThan);
            System.out.println(""10 < 5: "" + lessThan);
            System.out.println(""(5 > 2) && (10 > 5): "" + andExpression);
            System.out.println(""(5 > 2) || (10 < 5): "" + orExpression);
            System.out.println(""!(5 > 2): "" + notExpression);

            // Operator Precedence and Parentheses
            int precedenceExample1 = 10 + 5 * 2;             // Multiplication before addition
            int precedenceExample2 = (10 + 5) * 2;           // Parentheses change the order

            System.out.println(""\nOperator Precedence and Parentheses:"");
            System.out.println(""10 + 5 * 2 = "" + precedenceExample1);   // Output: 20
            System.out.println(""(10 + 5) * 2 = "" + precedenceExample2); // Output: 30
        }
    }";
}


    public void ClearInput()
    {
        SetInitialJavaTemplate();
    }

    public void PrintLNTemplate()
    {
        inputField.text = 
@"class TempJavaFile {
    public static void main(String[] args) {
        System.out.println(""Hello world"");
    }
}";
    }

      public void Array()
    {
        inputField.text = 
        @"class TempJavaFile {
            public static void main(String[] args) {
                // 1. What is an Array?
                // An array is like a container that holds a bunch of values of the same type (all numbers, all words, etc.).
                // Each value has a numbered position called an index, starting from 0.

                // 2. Creating Arrays
                int[] scores = new int[5]; // Creates an array named 'scores' to hold 5 integers.
                
                // 3. Accessing Array Elements
                scores[0] = 85; // Sets the first score (index 0) to 85.
                scores[1] = 92;
                scores[2] = 78;
                scores[3] = 88;
                scores[4] = 95;

                System.out.println(scores[2]); // Prints 78 (the score at index 2).

                // 4. Array Length
                int numElements = scores.length; // numElements will be 5.
                System.out.println(numElements);

                // 5. Initializing Arrays Directly
                double[] prices = {12.99, 9.95, 25.50, 19.99}; // An array of doubles is created and initialized

                // 6. Looping Through Arrays
                int[] numbers = {1, 2, 3, 4, 5};
                for (int i = 0; i < numbers.length; i++) {
                    System.out.println(numbers[i]); // Prints each number
                }

                // 7. Enhanced For Loop (For-Each Loop)
                for (int number : numbers) {
                    System.out.println(number); // Prints each number
                }

                // 8. Two-Dimensional Arrays (Tables)
                int[][] grades = new int[3][4]; // 3 rows, 4 columns
                grades[0][0] = 80;
                grades[1][2] = 95; // Accessing a specific element.
                // and so on...
            }
        }";
            }

            public void functions() {
    inputField.text = 
    @"class TempJavaFile {
        public static void main(String[] args) {
            // 1. Pre-defined Functions
            // Example of a pre-defined function: calculating the square root
            double squareRoot = Math.sqrt(9); // Gives 3
            System.out.println(""Square root of 9: "" + squareRoot);

            // 2. User-defined Functions: Creating Your Own Functions
            // Function to add two numbers
            int result = addNumbers(5, 3); // Calls the function, adds 5 and 3.
            System.out.println(""Result of adding 5 and 3: "" + result); // Prints ""8""

            // 3. Using Your Function
            System.out.println(""Result of adding 10 and 20: "" + addNumbers(10, 20));

            // 4. Void Functions
            printMessage(""Hello, world!""); // Calls a void function that prints a message
        }

        // User-defined function to add two numbers
        public static int addNumbers(int num1, int num2) {
            int sum = num1 + num2; // Adds num1 and num2
            return sum; // Returns the sum
        }

        // Void function that prints a message
        public static void printMessage(String message) {
            System.out.println(message); // Prints the given message
        }
    }";
}


      public void FlowofPrograme()
            {
                inputField.text = 
            @"class TempJavaFile {
                public static void main(String[] args) {
                    // 1. The if Statement: Making a Simple Choice
                    int age = 16;
                    if (age >= 18) { // Check if age is 18 or greater
                        System.out.println(""You can vote!""); // Runs only if age is 18 or older
                    }

                    // 2. The if-else Statement: Handling Two Possibilities
                    if (age >= 18) {
                        System.out.println(""You can vote!"");
                    } else {
                        System.out.println(""You are too young to vote."");
                    }

                    // 3. Nested if-else Statements: Making More Complex Decisions
                    int score = 85;
                    if (score >= 90) {
                        System.out.println(""A"");
                    } else if (score >= 80) {
                        System.out.println(""B"");
                    } else if (score >= 70) {
                        System.out.println(""C"");
                    } else {
                        System.out.println(""F"");
                    }

                    // 4. The switch Statement: A Quick Overview
                    int dayOfWeek = 3; // 3 represents Wednesday
                    switch (dayOfWeek) {
                        case 1:
                            System.out.println(""Monday"");
                            break;
                        case 2:
                            System.out.println(""Tuesday"");
                            break;
                        case 3:
                            System.out.println(""Wednesday"");
                            break;
                        case 4:
                            System.out.println(""Thursday"");
                            break;
                        case 5:
                            System.out.println(""Friday"");
                            break;
                        case 6:
                        case 7:
                            System.out.println(""Weekend!"");
                            break;
                        default:
                            System.out.println(""Invalid day"");
                            break;
                    }
                }
            }";
            }

    public void DataTypesTemplate()
    {
        inputField.text = 
@"class TempJavaFile {
    public static void main(String[] args) {
        int integerVar = 42;
        double doubleVar = 3.14;
        boolean booleanVar = true;
        char charVar = 'A';
        String stringVar = ""Hello, Data Types!"";

        System.out.println(""Integer: "" + integerVar);
        System.out.println(""Double: "" + doubleVar);
        System.out.println(""Boolean: "" + booleanVar);
        System.out.println(""Character: "" + charVar);
        System.out.println(""String: "" + stringVar);

        int sum = integerVar + (int)doubleVar;
        System.out.println(""Sum of Integer and Double (cast to int): "" + sum);
    }
}";
    }
            public void Length()
        {
            inputField.text = 
            @"class TempJavaFile {
                public static void main(String[] args) {
                    // 1. Creating Strings
                    String message = ""Hello, world!"";
                    String anotherMessage = new String(""Goodbye!"");
                    
                    // 2. String Length
                    String name = ""Alice"";
                    int length = name.length(); // length will be 5
                    System.out.println(""Length of name: "" + length);
                    
                    // 3. Accessing Individual Characters
                    String city = ""London"";
                    char firstLetter = city.charAt(0); // firstLetter will be 'L'
                    System.out.println(""First letter of city: "" + firstLetter);
                    
                    // 4. Comparing Strings
                    String str1 = ""hello"";
                    String str2 = ""hello"";
                    String str3 = ""Hello"";
                    System.out.println(""str1 equals str2: "" + str1.equals(str2)); // true
                    System.out.println(""str1 equals str3: "" + str1.equals(str3)); // false
                    System.out.println(""str1 equalsIgnoreCase str3: "" + str1.equalsIgnoreCase(str3)); // true
                    
                    // 5. Combining Strings (Concatenation)
                    String firstName = ""Bob"";
                    String lastName = ""Dylan"";
                    String fullName = firstName + "" "" + lastName;
                    System.out.println(""Full name: "" + fullName); // ""Bob Dylan""
                    
                    // 6. Changing String Case
                    String greeting = ""Hello There"";
                    String lowerCase = greeting.toLowerCase();
                    String upperCase = greeting.toUpperCase();
                    System.out.println(""Lower case: "" + lowerCase); // ""hello there""
                    System.out.println(""Upper case: "" + upperCase); // ""HELLO THERE""
                    
                    // 7. Extracting Parts of a String (Substrings)
                    String sentence = ""This is a sentence."";
                    String sub = sentence.substring(5, 8); // sub will be ""is ""
                    System.out.println(""Substring: "" + sub);
                    
                    // 8. Removing Whitespace (trim)
                    String messyString = "" Extra spaces "";
                    String cleanString = messyString.trim(); // ""Extra spaces""
                    System.out.println(""Cleaned string: '"" + cleanString + ""'"");
                }
            }";
        }

    



    public async void RunJavaCodeAsync()
    {
        outputText.text = "Compiling...";
        await Task.Run(() => RunJavaCode());
    }

    private void RunJavaCode()
    {
        string javaCode = inputField.text;
        string userInput = userInputField.text;
        string filePath = Path.Combine(Application.persistentDataPath, "TempJavaFile.java");

        try
        {
            File.WriteAllText(filePath, javaCode);
        }
        catch (IOException ioEx)
        {
            UpdateOutput($"Error writing Java file: {ioEx.Message}");
            return;
        }

        // Compile the Java code
        string compileOutput, compileErrors;
        bool compilationSuccess = CompileJavaCode(filePath, out compileOutput, out compileErrors);

        UpdateOutput(compileOutput + "\n" + compileErrors);

        if (compilationSuccess)
        {
            string runOutput, runErrors;
            RunJavaProgram(out runOutput, out runErrors, userInput);
            UpdateOutput(runOutput + "\n" + runErrors);
        }

        // Clean up by deleting the temporary Java file
        File.Delete(filePath);
    }

    private bool CompileJavaCode(string filePath, out string compileOutput, out string compileErrors)
    {
        Process compileProcess = new Process();
        compileProcess.StartInfo.FileName = "javac";
        compileProcess.StartInfo.Arguments = $"\"{filePath}\"";
        compileProcess.StartInfo.UseShellExecute = false;
        compileProcess.StartInfo.RedirectStandardOutput = true;
        compileProcess.StartInfo.RedirectStandardError = true;
        compileProcess.StartInfo.CreateNoWindow = true;

        try
        {
            compileProcess.Start();
            compileOutput = compileProcess.StandardOutput.ReadToEnd();
            compileErrors = compileProcess.StandardError.ReadToEnd();
            compileProcess.WaitForExit();
            return compileProcess.ExitCode == 0;
        }
        catch (Exception ex)
        {
            compileOutput = string.Empty;
            compileErrors = $"Error during compilation: {ex.Message}\nCheck if Java is installed and in your PATH.";
            return false;
        }
        finally
        {
            compileProcess.Dispose();
        }
    }

    private void RunJavaProgram(out string runOutput, out string runErrors, string userInput)
    {
        Process runProcess = new Process();
        runProcess.StartInfo.FileName = "java";
        runProcess.StartInfo.Arguments = $"-classpath \"{Application.persistentDataPath}\" TempJavaFile";
        runProcess.StartInfo.UseShellExecute = false;
        runProcess.StartInfo.RedirectStandardOutput = true;
        runProcess.StartInfo.RedirectStandardError = true;
        runProcess.StartInfo.RedirectStandardInput = true; // Enable input redirection
        runProcess.StartInfo.CreateNoWindow = true;

        try
        {
            runProcess.Start();

            using (StreamWriter writer = runProcess.StandardInput)
            {
                if (writer.BaseStream.CanWrite)
                {
                    writer.WriteLine(userInput);
                }
            }

            runOutput = runProcess.StandardOutput.ReadToEnd();
            runErrors = runProcess.StandardError.ReadToEnd();
            runProcess.WaitForExit();
        }
        catch (Exception ex)
        {
            runOutput = string.Empty;
            runErrors = $"Error during execution: {ex.Message}";
        }
        finally
        {
            runProcess.Dispose();
        }
    }

    private void UpdateOutput(string message)
    {
        outputText.text += "\n" + message;
        //typingAnimation.SetText(outputText.text);
    }
}

