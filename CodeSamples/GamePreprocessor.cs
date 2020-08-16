/*
Generates questions and answers based on GameSetupData.cs using GenerateQuestionAndAnswer().
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

/// <summary> Processes game data before the game begins. Mainly used to generate questions and answers. </summary>
public class GamePreprocessor : MonoBehaviour
{
    private Random random = new Random();
    private static GlobalData globalData;

    private GameSetupData gameSetupData;
    private List<QuestionAndAnswer> questionsAndAnswers;

    private void Awake()
    {
        // GamePreprocessor persists through scenes until it is destroyed after a game finishes.
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        globalData = FindObjectOfType<GlobalData>().GetComponent<GlobalData>();
        gameSetupData = InitGameSetupData(); // Initialize GameSetupData object.
    }

    /// <summary> 
    /// Resets list of questions. Can be used for restarting a game with new questions and same setup options. 
    /// </summary>
    public void NewQuestionsAndAnswers()
    {
        questionsAndAnswers = new List<QuestionAndAnswer>();

        for (int i = 0; i < gameSetupData.NumberOfQuestions; i++)
            questionsAndAnswers.Add(GenerateQuestionAndAnswer(gameSetupData.Difficulty));
    }

    /// <summary>
    /// Retreives game options from user's input from the menus. 
    /// Contains everything needed to create default questions and answers.
    /// </summary>
    public GameSetupData InitGameSetupData()
    {
        GameSetupData temp_gameSetupData = new GameSetupData();


        // Required default values. Do not change.
        temp_gameSetupData.NumberOfWrongAnswers = 3;
        temp_gameSetupData.NumberOfQuestions = 5;
        temp_gameSetupData.DigitRange = new int[2];
        temp_gameSetupData.NumberOfWrongAnswers = 3;
        temp_gameSetupData.NumberOfAnswersTotal = 4;

        temp_gameSetupData.OperatorsAndSymbols = new Dictionary<string, bool>();
        temp_gameSetupData.OperatorsAndSymbols.Add("div", false);
        temp_gameSetupData.OperatorsAndSymbols.Add("mul", false);
        temp_gameSetupData.OperatorsAndSymbols.Add("add", false);
        temp_gameSetupData.OperatorsAndSymbols.Add("sub", false);
        temp_gameSetupData.IsValid = false;
        temp_gameSetupData.GameTimeLimit = 10;


        // User can change these variables.
        temp_gameSetupData.Difficulty = Enum_Difficulty.EASY;
        temp_gameSetupData.DigitRange[0] = 1;
        temp_gameSetupData.DigitRange[1] = 2;
        temp_gameSetupData.MaxMultiple = 12;
        temp_gameSetupData.NegativeDifference = false;

        return temp_gameSetupData;
    }

    /// <summary>
    /// Used in GenerateQuestionAndAnswer() to account for different variables used to make
    /// questions easier or harder. This method modifies GameSetupData to created a question
    /// with specified characteristics.
    /// </summary>
    /// <param name="maxDigits">Maximum number of digits a term can have.</param>
    /// <param name="maxMultiple">Maximum number a multiplication term can have.</param>
    public void ModifyDifficultyFactors(int maxDigits, int maxMultiple)
    {
        // Max digits
        gameSetupData.DigitRange[0] = 1;
        gameSetupData.DigitRange[1] = maxDigits;

        // Max multiple
        gameSetupData.MaxMultiple = maxMultiple;
    }


    public QuestionAndAnswer GenerateQuestionAndAnswer(Enum_Difficulty difficultyEnum)
    {
        // Placeholder object.
        QuestionAndAnswer tempQuestionAndAnswer = null;

        // List of valid operators that can be used.
        List<string> validOperators = new List<string>();

        // Determines which operators are specified to use.
        foreach (KeyValuePair<string, bool> entry in gameSetupData.OperatorsAndSymbols)
            if (entry.Value == true)
                validOperators.Add(entry.Key); // Adds the 3 letter operator name

        // Variables used to construct QuestionAndAnswer object.
        string operatorToUse = validOperators[random.Next(0, validOperators.Count)];
        string question = null;
        int answer = 0;
        List<int> wrongAnswers = new List<int>();

        // Sets values based on difficulty.
        switch (difficultyEnum)
        {
            case Enum_Difficulty.EASY:
                // EASY
                // single digits
                // up to 12 x 12
                // positive differences
                ModifyDifficultyFactors(1, 12);
                break;

            case Enum_Difficulty.MEDIUM:
                // MEDIUM
                // double digits
                // up to 15 x 15
                // positive differences
                ModifyDifficultyFactors(2, 15);
                break;

            case Enum_Difficulty.HARD:
                // HARD
                // triple dgits
                // up to 20 x 20
                // negative differences
                ModifyDifficultyFactors(3, 20);
                break;

            default:
                break;
        }

        // Creates equation, answer and data needed for return-object.
        switch (operatorToUse)
        {
            case "div":

                // maxValue equation: f(x) = 10 ^ x - 1. 
                // f(1) = 10 ^ 1 - 1 = 9
                // f(2) = 10 ^ 2 - 1 = 99
                int dividend = random.Next(2, gameSetupData.MaxMultiple);
                int divisor = random.Next(2, gameSetupData.MaxMultiple);
                int quotient = dividend * divisor;

                question = "" + quotient + " ÷ " + dividend;
                answer = divisor;

                break;

            case "mul":

                int mulTerm1 = random.Next(2, gameSetupData.MaxMultiple);
                int mulTerm2 = random.Next(2, gameSetupData.MaxMultiple);

                // Places larger number first.
                if (mulTerm1 < mulTerm2)
                {
                    int tempMulTerm = mulTerm1;

                    mulTerm1 = mulTerm2;
                    mulTerm2 = tempMulTerm;
                }

                int mulProduct = mulTerm1 * mulTerm2;

                question = "" + mulTerm1 + " x " + mulTerm2;
                answer = mulProduct;

                break;

            case "add":

                // maxValue equation: f(x) = 10 ^ x - 1. 
                // f(1) = 10 ^ 1 - 1 = 9
                // f(2) = 10 ^ 2 - 1 = 99
                int addTerm1 = random.Next(1, Convert.ToInt32(Math.Pow(10, gameSetupData.DigitRange[1]) - 1));
                //int addTerm2 = random.Next(1, Convert.ToInt32(Math.Pow(10, gameSetupData.DigitRange[1]) - 1));
                int addTerm2 = random.Next(1, Convert.ToInt32(Math.Pow(10, gameSetupData.DigitRange[1]) - 1));

                // Places larger number first.
                if (addTerm1 < addTerm2)
                {
                    int tempAddTerm = addTerm1;

                    addTerm1 = addTerm2;
                    addTerm2 = tempAddTerm;
                }

                int addSum = addTerm1 + addTerm2;

                question = addTerm1 + " + " + addTerm2;
                answer = addSum;

                break;

            case "sub":

                int subTerm1 = random.Next(1, Convert.ToInt32(Math.Pow(10, gameSetupData.DigitRange[1]) - 1));
                int subTerm2 = random.Next(1, Convert.ToInt32(Math.Pow(10, gameSetupData.DigitRange[1]) - 1));

                // Places larger number first.
                if (subTerm1 < subTerm2)
                {
                    int tempSubTerm = subTerm1;

                    subTerm1 = subTerm2;
                    subTerm2 = tempSubTerm;
                }

                int subTerm2Temp = subTerm2;

                for (int i = random.Next(1,2); i == 2; i++)
                {
                    if (subTerm2 > 20)
                    {
                        subTerm2 = random.Next(1, 10);
                        Debug.Log("subTerm2 changed from " + subTerm2Temp + "  to " + subTerm2);
                    }
                }

                int subDif = subTerm1 - subTerm2;

                question = subTerm1 + " - " + subTerm2;
                answer = subDif;

                break;

            default:
                Debug.LogWarning("<color=orange>No operator used!</color>");
                break;
        }

        // Determines wrong answers.
        for (int i = 0; wrongAnswers.Count < gameSetupData.NumberOfWrongAnswers; i++)
        {
            int offsetAnswer = answer + random.Next(-15, 15);

            // Ensures divWrongAnswers has unique values only.
            while (wrongAnswers.Contains(offsetAnswer) || offsetAnswer == 0 || offsetAnswer == answer)
                offsetAnswer = answer + random.Next(-15, 15);

            wrongAnswers.Add(offsetAnswer);
        }

        // Prevents sequential duplicate questions. If there is a duplicate, this statement recursively creates another/more questions.
        if (questionsAndAnswers.Count >= 1 && questionsAndAnswers[questionsAndAnswers.Count - 1].question.Equals(question))
        {
            Debug.LogWarning("Duplicate question \"" + question + "\" found at index: " + (questionsAndAnswers.Count) + ". Creating a replacement.");
            tempQuestionAndAnswer = GenerateQuestionAndAnswer(gameSetupData.Difficulty);
        }
        else
        {
            tempQuestionAndAnswer = new QuestionAndAnswer(question, answer, wrongAnswers);
        }

        return tempQuestionAndAnswer;
    }
    
    public List<QuestionAndAnswer> QuestionsAndAnswers { get => questionsAndAnswers; set => questionsAndAnswers = value; }
    public GameSetupData GameSetupData { get => gameSetupData; set => gameSetupData = value; }

}
