using System.Collections.Generic;

/// <summary>
/// Used as a data holder for the GamePreprocessor script.
/// </summary>
public class GameSetupData
{
    private Dictionary<string, bool> operatorsAndSymbols;
    /// <summary>
    /// First index must be >= 1
    /// </summary>
    private int[] digitRange;

    private int numberOfWrongAnswers;
    private int numberOfQuestions;
    private int numberOfAnswersTotal;
    private bool negativeDifference;
    private float gameTimeLimit;


    /// <summary>
    /// First index must be >= 1
    /// </summary>
    private int[] termsRange;
    private int maxMultiple;
    private bool isValid;
    private Enum_Difficulty difficulty;

    public Dictionary<string, bool> OperatorsAndSymbols { get => operatorsAndSymbols; set => operatorsAndSymbols = value; }
    public int NumberOfWrongAnswers { get => numberOfWrongAnswers; set => numberOfWrongAnswers = value; }
    public int[] DigitRange { get => digitRange; set => digitRange = value; }
    public int[] TermsRange { get => termsRange; set => termsRange = value; }
    public int NumberOfQuestions { get => numberOfQuestions; set => numberOfQuestions = value; }
    public int MaxMultiple { get => maxMultiple; set => maxMultiple = value; }
    public int NumberOfAnswersTotal { get => numberOfAnswersTotal; set => numberOfAnswersTotal = value; }
    public bool IsValid { get => isValid; set => isValid = value; }
    public bool NegativeDifference { get => negativeDifference; set => negativeDifference = value; }
    public float GameTimeLimit { get => gameTimeLimit; set => gameTimeLimit = value; }
    public Enum_Difficulty Difficulty { get => difficulty; set => difficulty = value; }
}