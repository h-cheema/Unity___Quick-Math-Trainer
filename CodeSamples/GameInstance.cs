/*
Is instantiated once the Game scene is loaded and an actual round of a game starts. It handles all of the Text and Image 
component updates, animations, scoring, the game timer, and win/loss detection.

It takes the list of QuestionAndAnswer objects and displays the first one for the player to answer. If the user answers 
correctly, the question iterates, animations and a sound play to let the user know they answered correctly, and the UI 
components (answer area, question text, score area) update to the next question after the animation has stopped. Once the 
player either wins or loses the game, the timer is turned off and the finish screen is faded in.
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

/// <summary> Handles running data and events like scoring, win/loss, questions/answers displays. </summary>
public class GameInstance : MonoBehaviour
{
    Random random = new Random();

    // GameObject/script references...
    private static GlobalData globalData;
    private AudioManager audioManager;
    private GamePreprocessor gamePreprocessor;
    private PlayerScoreElement playerScoreElement;
    private GameDefaults gameDefaults;
    private MenuManager menuManager;

    // Game variables
    private int questionNumber = 0;
    private float answerDelay = 0.5f;
    private string answerTextString = "";
    private Text answerText;
    private Text questionText;

    private GameObject[] answerButtons;
    private GameObject clearAnswerButton;
    private CanvasGroup answerFeedbackCanvasGroup;
    private CanvasGroup answerTextCanvasGroup;

    // GameFinishScreen 
    private GameObject gameFinishScreen;
    private GameObject gameFinishScreenContent;
    private Text gameFinishScreenTitle;
    private Text gameFinishScreenSubTitle;

    // Timer variables
    private bool timerIsOn;
    private float gameTimeLimit;
    private float timeElapsed = 0;
    private float incrementalSliderValue;

    // Timer game objects and components
    private Slider timerSliderLeft;
    private Slider timerSliderRight;
    private Image timerSliderFillLeft;
    private Image timerSliderFillRight;
    private Text timerText;

    void Start()
    {
        // Linking scripts.
        globalData = FindObjectOfType<GlobalData>().GetComponent<GlobalData>();
        gameDefaults = globalData.GameDefaults;
        audioManager = FindObjectOfType<AudioManager>().GetComponent<AudioManager>();
        gamePreprocessor = FindObjectOfType<GamePreprocessor>().GetComponent<GamePreprocessor>();
        playerScoreElement = FindObjectOfType<PlayerScoreElement>().GetComponent<PlayerScoreElement>();
        menuManager = FindObjectOfType<MenuManager>().GetComponent<MenuManager>();


        // Initializes GameSetupData object.
        gamePreprocessor.NewQuestionsAndAnswers();
        gameTimeLimit = gamePreprocessor.GameSetupData.GameTimeLimit;


        // Finding gameobjects in game.
        answerButtons = GameObject.FindGameObjectsWithTag("AnswerButton");
        answerFeedbackCanvasGroup = GameObject.Find("Image_AnswerFeedback").GetComponent<CanvasGroup>();
        answerText = GameObject.Find("Answer_Text").GetComponent<Text>();
        answerTextCanvasGroup = GameObject.Find("Answer_Text").GetComponent<CanvasGroup>();
        clearAnswerButton = GameObject.Find("Btn_ClearAnswer");
        questionText = GameObject.Find("Area_Question_Text").GetComponent<Text>();

        gameFinishScreen = GameObject.Find("GameFinishScreen");
        gameFinishScreenContent = GameObject.Find("GameFinishScreenContent");
        gameFinishScreenSubTitle = GameObject.Find("GameFinishScreenSubTitle").GetComponent<Text>();
        gameFinishScreenTitle = GameObject.Find("GameFinishScreenTitle").GetComponent<Text>();

        timerSliderFillLeft = GameObject.Find("TimerSliderFillLeft").GetComponent<Image>();
        timerSliderFillRight = GameObject.Find("TimerSliderFillRight").GetComponent<Image>();
        timerSliderLeft = GameObject.Find("TimerSliderLeft").GetComponent<Slider>();
        timerSliderRight = GameObject.Find("TimerSliderRight").GetComponent<Slider>();
        timerText = GameObject.Find("Text_TimeRemaining").GetComponent<Text>();

        // Adds listeners for answer button sounds.
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("AnswerButton"))
            go.GetComponentInChildren<Button>().onClick.AddListener(() => audioManager.Play("Btn_Click"));


        // Disables gameFinishScreen.
        gameFinishScreen.GetComponent<CanvasGroup>().alpha = 0f;
        gameFinishScreenContent.GetComponent<CanvasGroup>().alpha = 0f;


        // Time-limit.
        timerIsOn = true;
        incrementalSliderValue = gameTimeLimit / 4;

        timerSliderFillLeft.color = gameDefaults.DefaultColors[Enum_Colors.GREEN];
        timerSliderFillRight.color = gameDefaults.DefaultColors[Enum_Colors.GREEN];

        timerSliderLeft.maxValue = gameTimeLimit;
        timerSliderRight.maxValue = gameTimeLimit;

        timerSliderLeft.value = gameTimeLimit;
        timerSliderRight.value = gameTimeLimit;


        // Initial labels update.
        UpdateQuestionText();
    }

    private void Update()
    {
        RunGameTimer();
    }

    public void RunGameTimer()
    {
        if (timerIsOn)
        {
            timeElapsed = Time.deltaTime;

            gameTimeLimit -= Time.deltaTime;

            // If time runs out.
            if (gameTimeLimit <= 0)
            {
                Debug.Log("GAME OVER");
                timerIsOn = false;
                GameFinish();
                StartCoroutine(GameFinishScreen(Enum_GameFinishCondition.LOSE)); // Lose game.
            }

            timerText.text = gameTimeLimit.ToString("0.");

            RunTimeLimit();
        }
    }
    public void RunTimeLimit()
    {
        // Progresses the time limit slider bar.
        timerSliderLeft.value -= Time.deltaTime;
        timerSliderRight.value -= Time.deltaTime;


        // Changes the color based on the progress of the time limit slider bar based on a small interval.
        if (timerSliderLeft.value > incrementalSliderValue - 0.1f
            && timerSliderLeft.value < incrementalSliderValue)
        {
            timerSliderFillLeft.color = Color.red;
            timerSliderFillRight.color = Color.red;
            Debug.Log("Time limit color changed to red.");
        }
        else
        if (timerSliderLeft.value > (incrementalSliderValue - 0.1) * 2
            && timerSliderLeft.value < incrementalSliderValue * 2)
        {
            timerSliderFillLeft.color = Color.yellow;
            timerSliderFillRight.color = Color.yellow;
            Debug.Log("Time limit color changed to yellow.");
        }
    }


    #region Answer Checking

    /// <summary> Appends a number to the answer input text in game.</summary> 
    /// <param name="buttonValue">Number value of the button this method is called by.</param>
    public void Btn_AnswerNumber(int buttonValue)
    {
        answerTextString += buttonValue;
        answerText.text = answerTextString;

        CheckAnswer();
    }

    /// <summary> Checks if the answer text matches the correct answer. Called each time a number button is pressed. </summary>
    public void CheckAnswer()
    {
        // Correct answer corresponding to the current question.
        int correctAnswer = gamePreprocessor.QuestionsAndAnswers[questionNumber].answer;
        string correctAnswerString = correctAnswer.ToString();


        // Checks for correct or wrong answer.
        if (answerTextString.Length == correctAnswerString.Length) // If input # of digits = answer # of digits.
        {
            if (answerTextString == correctAnswerString) // If answer is correct.
                CorrectAnswer();
            else // If answer is incorrect.
                WrongAnswer();
        }
    }

    /// <summary> Processes sound, question incrementing, fade animations, finish condition. </summary>
    public void CorrectAnswer()
    {
        audioManager.Play("Game_CorrectAnswer"); // Play correct answer sound.

        // Change question number then process changes to game UI
        IncrementQuestionNumber();
        playerScoreElement.UpdateScoreLabels();


        if (questionNumber >= gamePreprocessor.QuestionsAndAnswers.Count) // Win game.
        {
            // Fade in game-win screen.
            StartCoroutine(GameFinishScreen(Enum_GameFinishCondition.WIN));

            // Fade animations run paralell to waiting time.
            StartCoroutine(FadeCanvasGroup(answerFeedbackCanvasGroup.gameObject, Enum_FadeOptions.OUT, Enum_ComponentTypes.IMAGE, 2, gameDefaults.DefaultColors[Enum_Colors.GREEN]));

            GameFinish();
        }
        else if (questionNumber < gamePreprocessor.QuestionsAndAnswers.Count)
        {
            // Fade in game-lose screen.
            StartCoroutine(AnswerWait(answerDelay, Enum_AnswerResult.CORRECT));

            // Fade animations run paralell to waiting time.
            StartCoroutine(FadeCanvasGroup(answerFeedbackCanvasGroup.gameObject, Enum_FadeOptions.OUT, Enum_ComponentTypes.IMAGE, 2, gameDefaults.DefaultColors[Enum_Colors.GREEN]));
            StartCoroutine(FadeCanvasGroup(answerText.gameObject, Enum_FadeOptions.OUT, Enum_ComponentTypes.TEXT, 2, gameDefaults.DefaultColors[Enum_Colors.GREEN]));
        }
    }

    /// <summary>  </summary>
    public void WrongAnswer()
    {
        audioManager.Play("Game_WrongAnswer"); // Play incorrect answer sound.

        // Waits before resetting the answer text and re-enabling answer buttons.
        StartCoroutine(AnswerWait(answerDelay, Enum_AnswerResult.INCORRECT));

        // Fade animations run paralell to waiting time.
        StartCoroutine(FadeCanvasGroup(answerFeedbackCanvasGroup.gameObject, Enum_FadeOptions.OUT, Enum_ComponentTypes.IMAGE, 2, gameDefaults.DefaultColors[Enum_Colors.RED]));
        StartCoroutine(FadeCanvasGroup(answerText.gameObject, Enum_FadeOptions.OUT, Enum_ComponentTypes.TEXT, 2, gameDefaults.DefaultColors[Enum_Colors.RED]));
    }

    /// <summary> Handles the UI for when a game has finished completely. </summary>
    /// <param name="condition">Whether the game has finished as a win or a loss.</param>
    /// <returns></returns>
    IEnumerator GameFinishScreen(Enum_GameFinishCondition condition)
    {
        // Modifies the base GameFinishScreenContent components based on whether the user won or lost.
        if (condition == Enum_GameFinishCondition.WIN)
        {
            gameFinishScreen.GetComponent<Image>().color = gameDefaults.DefaultColors[Enum_Colors.GREEN];
            gameFinishScreenTitle.text = "You Won";
            gameFinishScreenSubTitle.text = "With " + (gameTimeLimit - timeElapsed).ToString("0.00") + "s left"; // Seconds left at time of win.

        }
        else if (condition == Enum_GameFinishCondition.LOSE)
        {
            gameFinishScreen.GetComponent<Image>().color = gameDefaults.DefaultColors[Enum_Colors.RED];
            gameFinishScreenTitle.text = "Game Over";
            gameFinishScreenSubTitle.text = "";
        }

        // Disable interactability with game UI.
        GameObject.Find("Panel_Areas").GetComponent<CanvasGroup>().interactable = false;
        GameObject.Find("Panel_Areas").GetComponent<CanvasGroup>().blocksRaycasts = false;

        // Fade in the solid background for the game finish screen content.
        yield return StartCoroutine(FadeCanvasGroup(gameFinishScreen, Enum_FadeOptions.IN, Enum_ComponentTypes.GAME_OBJECT, 0.95f, null));

        // Enable interactability for game finish screen.
        GameObject.Find("GameFinishScreenContent").GetComponent<CanvasGroup>().interactable = true;
        GameObject.Find("GameFinishScreenContent").GetComponent<CanvasGroup>().blocksRaycasts = true;
        gameFinishScreen.GetComponent<CanvasGroup>().interactable = true;
        gameFinishScreen.GetComponent<CanvasGroup>().blocksRaycasts = true;

        // Fade in the game finish screen content.
        yield return StartCoroutine(FadeCanvasGroup(gameFinishScreenContent, Enum_FadeOptions.IN, Enum_ComponentTypes.GAME_OBJECT, 0.98f, null));
    }

    public void GameButtonsEnabled(bool value)
    {
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("AnswerButton");

        foreach (GameObject go in gameObjects)
        {
            go.GetComponent<Button>().enabled = value;
        }

        GameObject.Find("Btn_ClearAnswer").GetComponent<Button>().enabled = value;
    }

    IEnumerator AnswerWait(float duration, Enum_AnswerResult answerResult)
    {
        // Disable answer buttons.
        GameButtonsEnabled(false);

        // Reset alpha.
        answerFeedbackCanvasGroup.alpha = 1.0f;

        switch (answerResult)
        {
            case Enum_AnswerResult.CORRECT:

                // Pause the timer after a correct answer while waiting for the post-answer animations.
                TimerLimitEnabled(false);

                // Answer feedback background image.
                answerFeedbackCanvasGroup.GetComponent<Image>().color = gameDefaults.DefaultColors[Enum_Colors.GREEN];

                // Update score.
                playerScoreElement.UpdateScoreLabels();

                // WAIT
                yield return new WaitForSeconds(duration);

                // Resets text/labels for next question.
                UpdateQuestionText();

                // Re-enable timer.
                TimerLimitEnabled(true);

                break;

            case Enum_AnswerResult.INCORRECT:

                // Vibrate phone for wrong answers.
                Handheld_Vibrate();

                // Answer text gameobject = red.
                answerText.color = gameDefaults.DefaultColors[Enum_Colors.RED];

                // Disable clear button.
                clearAnswerButton.GetComponent<Button>().enabled = false;

                answerFeedbackCanvasGroup.GetComponent<Image>().color = gameDefaults.DefaultColors[Enum_Colors.RED];

                // WAIT
                yield return new WaitForSeconds(duration);

                break;

            default:
                break;
        }

        // Reset answer text component.
        answerText.color = gameDefaults.DefaultColors[Enum_Colors.BLACK];
        answerTextCanvasGroup.alpha = 1f;

        answerFeedbackCanvasGroup.alpha = 0f;

        // Clear answer text gameobject for new answer.
        ClearAnswerText();

        // Re-enable game buttons.
        GameButtonsEnabled(true);

        // Re-enable game buttons after waiting.
        clearAnswerButton.GetComponent<Button>().enabled = true;
    }

    public void TimerLimitEnabled(bool isEnabled)
    {
        if (isEnabled)
            timerIsOn = true;
        else
            timerIsOn = false;
    }

    /// <summary> Used for testing. Answers the current question correctly. </summary>
    public void Btn_CorrectAnswer()
    {
        // USED FOR TESTING
        GameObject.Find("Btn_NextQ_Text").GetComponent<Text>().text
            = gamePreprocessor.QuestionsAndAnswers[questionNumber].Answer.ToString();

        answerText.text = gamePreprocessor.QuestionsAndAnswers[questionNumber].Answer.ToString();
        answerTextString = gamePreprocessor.QuestionsAndAnswers[questionNumber].Answer.ToString();

        CheckAnswer();
    }

    #endregion

    #region Fade Animations

    /// <summary>  </summary>
    public IEnumerator FadeCanvasGroup(GameObject go, Enum_FadeOptions fadeOption, Enum_ComponentTypes componentType, float duration, Color? newColor)
    {
        CanvasGroup goCanvasGroup = go.GetComponent<CanvasGroup>();

        // Set color of component to fade before fading.
        if (newColor.HasValue)
            switch (componentType)
            {
                case Enum_ComponentTypes.IMAGE:
                    goCanvasGroup.GetComponent<Image>().color = (Color)newColor;
                    break;

                case Enum_ComponentTypes.TEXT:
                    goCanvasGroup.GetComponent<Text>().color = (Color)newColor;
                    break;

                default:
                    break;
            }

        // Fade GameObject 'go' in/out.
        switch (fadeOption)
        {
            case Enum_FadeOptions.IN:

                // Fades alpha based on Time.deltaTime.
                while (goCanvasGroup.alpha < 1f)
                {
                    goCanvasGroup.alpha += Time.deltaTime * duration;
                    yield return null;
                }
                break;

            case Enum_FadeOptions.OUT:

                // Fades alpha based on Time.deltaTime.
                while (goCanvasGroup.alpha > 0)
                {
                    goCanvasGroup.alpha -= Time.deltaTime * duration;
                    yield return null;
                }
                break;

            default:
                break;
        }
    }

    #endregion

    #region Label Upddates

    /// <summary> Resets text to the deafult 6 underscores. </summary>
    public void ClearAnswerText()
    {
        answerText.text = "______";
        answerTextString = "";
        answerText.color = Color.black;
    }

    /// <summary> Increments the question by one. </summary>
    public void IncrementQuestionNumber()
    {
        questionNumber += 1;
    }

    public void UpdateQuestionText()
    {
        questionText.text = gamePreprocessor.QuestionsAndAnswers[questionNumber].Question;
    }

    #endregion

    #region Game Events

    /// <summary> Complete end of a game. Pauses timer and disables game buttons. </summary>
    public void GameFinish()
    {
        GameButtonsEnabled(false);

        // Stops the timer.
        timerIsOn = false;
    }

    /// <summary> Incomplete end of a game. Handles all loose ends. </summary>
    public void GameEnd()
    {
        globalData.DestroyGamePreprocessorGameObjects();
    }

    /// <summary> Reloads the scene, restarting the game with the same game options but with different questions. </summary>
    public void RestartGame()
    {
        menuManager.ChangeScene_Game();
        Debug.Log("Game restarted.");
    }

    #endregion

    /// <summary> Verifies the device is a handheld, then vibrates. </summary>
    public void Handheld_Vibrate()
    {
        #if UNITY_IPHONE || UNITY_ANDROID
                    Handheld.Vibrate();
        #endif
    }

    public int QuestionNumber { get => questionNumber; set => questionNumber = value; }
    public GamePreprocessor GamePreprocessor { get => gamePreprocessor; set => gamePreprocessor = value; }
}
