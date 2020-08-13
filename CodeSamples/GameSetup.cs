using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSetup : MonoBehaviour
{
    private static GlobalData globalData;
    private GameDefaults gameDefaults;
    private GamePreprocessor gamePreprocessor;
    private GameSetupData gameSetupData;
    private AudioManager audioManager;

    private Color activatedColor;

    // Mute button
    public Sprite soundOnSprite;
    public Sprite soundOffSprite;
    private Image soundButtonImage;
    private Image soundButtonImageBackground;

    private void Start()
    {
        globalData = FindObjectOfType<GlobalData>().GetComponent<GlobalData>();
        gameDefaults = globalData.GameDefaults;
        gamePreprocessor = FindObjectOfType<GamePreprocessor>().GetComponent<GamePreprocessor>();
        gameSetupData = gamePreprocessor.GameSetupData;
        audioManager = globalData.AudioManager;

        activatedColor = gameDefaults.DefaultColors[Enum_Colors.GREEN_LIGHT];

        soundButtonImage = GameObject.Find("Mute_Image").GetComponent<Image>();
        soundButtonImageBackground = GameObject.Find("Mute_Image_Background").GetComponent<Image>();

        GameObject.Find("Mute_Image_Background").GetComponent<Image>().color
                = gameDefaults.DefaultColors[Enum_Colors.GREEN_LIGHT];

        // Sets all 'activated state' button's/toggle's image colors.
        SetGameObjectColors();

        UpdateSetupData();
    }

    /// <summary> Sets the default colors of the GameSetup scene's menu buttons/toggles. </summary>
    public void SetGameObjectColors()
    {
        // Operator-toggles colors
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("OperatorToggleCheckmark"))
            go.GetComponent<Image>().color = activatedColor;

        // Difficulty-toggles colors
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("DifficultyToggle"))
            go.GetComponent<Image>().color = activatedColor;
    }

    /// <summary> Checks toggle boxes in GameSetup scene for which operators are selected, then 
    /// updates GameSetupData object. </summary>
    public void Retrieve_Operators()
    {
        gameSetupData.OperatorsAndSymbols["add"] = FindToggle("Toggle_Add").isOn;
        gameSetupData.OperatorsAndSymbols["sub"] = FindToggle("Toggle_Sub").isOn;
        gameSetupData.OperatorsAndSymbols["mul"] = FindToggle("Toggle_Mul").isOn;
        gameSetupData.OperatorsAndSymbols["div"] = FindToggle("Toggle_Div").isOn;

        // Checks if at least one operator is selected.
        int countToggles = 0;
        for (int i = 0; i < 4; i++)
            if (gameSetupData.OperatorsAndSymbols.ContainsValue(true))
                countToggles++;

        // Toggles interactable state of the start button. 
        Button startGameButton = GameObject.Find("Area_MenuControls_Btn_Play").GetComponent<Button>();
        Image startGameButtonImage = GameObject.Find("MenuControls_Btn_Play").GetComponent<Image>();
        if (countToggles >= 1) // Stays interactable.
        {
            gameSetupData.IsValid = true;

            startGameButton.interactable = true;
            startGameButtonImage.color = gameDefaults.DefaultColors[Enum_Colors.GREEN_LIGHT];

            GameObject.Find("Area_Operators_Title").GetComponent<Text>().color = gameDefaults.DefaultColors[Enum_Colors.BLACK];
            GameObject.Find("Menu_Button_Play_Text").GetComponent<Text>().color = gameDefaults.DefaultColors[Enum_Colors.BLACK];
        }
        else // Game is invalid (won't start) and the start button is non-interactable when no operators are selected.
        {
            gameSetupData.IsValid = false;

            startGameButton.interactable = false;
            startGameButtonImage.color = gameDefaults.DefaultColors[Enum_Colors.RED_LIGHT];

            GameObject.Find("Area_Operators_Title").GetComponent<Text>().color = gameDefaults.DefaultColors[Enum_Colors.RED_LIGHT];
            GameObject.Find("Menu_Button_Play_Text").GetComponent<Text>().color = gameDefaults.DefaultColors[Enum_Colors.RED_LIGHT];
        }
    }
    public Toggle FindToggle(string toggleName)
    {
        Toggle toggle = GameObject.Find(toggleName).GetComponent<Toggle>();
        return toggle;
    }

    /// <summary>
    /// Retreives all game-setup options.
    /// </summary>
    public void UpdateSetupData()
    {
        Retrieve_Operators();
        Retrieve_NumberOfQuestions();
        Retrieve_Difficulty();
    }

    /// <summary>  GameSetup scene - number of questions input field. </summary>
    /// <param name="incrementValue">Value to increment number of questions input field in 
    /// GameSetup scene. Can be a negative value. Examples: 1, -1, 2, -3.</param>
    public void Btn_IncrementNumberOfQuestions(int incrementValue)
    {
        int maxQuestions = 15;
        int minQuestions = 5;

        // Link to GameSetup data object.
        GamePreprocessor gamePreprocessor
            = FindObjectOfType<GamePreprocessor>().GetComponent<GamePreprocessor>();

        // Ensures that the number of questions is within the specified range.
        if ((gamePreprocessor.GameSetupData.NumberOfQuestions + incrementValue) < minQuestions)
        {
            gamePreprocessor.GameSetupData.NumberOfQuestions = minQuestions;
        }
        else if ((gamePreprocessor.GameSetupData.NumberOfQuestions + incrementValue) > maxQuestions)
        {
            gamePreprocessor.GameSetupData.NumberOfQuestions = maxQuestions;
        }
        else // Value is valid.
        {
            gamePreprocessor.GameSetupData.NumberOfQuestions += incrementValue;
        }

        GameObject.Find("InputField").GetComponent<InputField>().text
            = gamePreprocessor.GameSetupData.NumberOfQuestions.ToString();

        GameObject.Find("Text_NumberOfQuestions").GetComponent<Text>().text
            = gamePreprocessor.GameSetupData.NumberOfQuestions.ToString();

        Debug.Log("Number of questions value changed to: "
            + gamePreprocessor.GameSetupData.NumberOfQuestions.ToString());
    }

    /// <summary>  Checks number of questions text component in GameSetup scene </summary>
    public void Retrieve_NumberOfQuestions()
    {
        gameSetupData.NumberOfQuestions
            = Int32.Parse(GameObject.Find("Text_NumberOfQuestions").GetComponent<Text>().text);
    }
    public void Retrieve_Difficulty()
    {
        ToggleGroup toggleGroup = GameObject.Find("ToggleGroup_Difficulty").GetComponent<ToggleGroup>();

        string toggleName = "Toggle_Difficulty_Easy";
        foreach (Toggle toggle in toggleGroup.ActiveToggles())
        {
            // Active toggle
            toggleName = toggle.name;
        }

        switch (toggleName)
        {
            case "Toggle_Difficulty_Easy":
                gameSetupData.Difficulty = Enum_Difficulty.EASY;
                SetTimeLimit(0);
                break;

            case "Toggle_Difficulty_Medium":
                gameSetupData.Difficulty = Enum_Difficulty.MEDIUM;
                SetTimeLimit(1);
                break;

            case "Toggle_Difficulty_Hard":
                gameSetupData.Difficulty = Enum_Difficulty.HARD;
                SetTimeLimit(2);
                break;

            default:
                break;
        }
    }

    public void ToggleMute()
    {

        if (audioManager.SoundIsOn) // Mute sounds.
        {
            AudioListener.volume = 0;

            soundButtonImage.sprite = soundOffSprite;
            soundButtonImageBackground.color = gameDefaults.DefaultColors[Enum_Colors.BLACK];

            GameObject.Find("Mute_Image_Background").GetComponent<Image>().color 
                = gameDefaults.DefaultColors[Enum_Colors.BLACK];
        }
        else // Restore volumes.
        {
            AudioListener.volume = 1;

            soundButtonImage.sprite = soundOnSprite;
            soundButtonImageBackground.color = activatedColor;

            GameObject.Find("Mute_Image_Background").GetComponent<Image>().color
                = gameDefaults.DefaultColors[Enum_Colors.GREEN_LIGHT];
        }

        audioManager.SoundIsOn = !audioManager.SoundIsOn;
    }

    /// <summary> Sets the game time limit based on the difficulty and number of questions. </summary>
    /// <param name="index"></param>
    public void SetTimeLimit(int index)
    {
        float[] timeLimitFactor = { 2f, 4f, 7f };
        gamePreprocessor.GameSetupData.GameTimeLimit = gamePreprocessor.GameSetupData.NumberOfQuestions * timeLimitFactor[index];

        Debug.Log("number of questions: " + gamePreprocessor.GameSetupData.NumberOfQuestions);
        Debug.Log("game time limit: " + gamePreprocessor.GameSetupData.GameTimeLimit);
    }

}
