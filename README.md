<h1 align="center">Quick Math Trainer</h1>

<p align="center">
	<a href="https://play.google.com/store/apps/details?id=com.TeraKeySoftware.QuickMathTrainer" target="_blank">
	<img width="180" height="80" 
	src="https://play.google.com/intl/en_us/badges/static/images/badges/en_badge_web_generic.png" alt="">
	</a>
</p>

<br>

### Summary:

<br>

### Code Samples:

* _<a href="CodeSamples/GameSetup.cs" target="_blank">GameSetup.cs</a>_
	* GameSetup.cs manages the "NewGame" menu scene. In this scene, the user chooses which algebraic operators they want the questions to use, how difficult the questions should be (easy, medium or hard) and whether they want sound on or off.
	* GameSetup.cs modifies the GameSetupData.cs class directly. Once the user chooses to start the game, this is when  GamePreprocessor.cs generates all of the QuestionAndAnswer objects using the data from GameSetupData. 



* _<a href="CodeSamples/GamePreprocessor.cs" target="_blank">GamePreprocessor.cs</a>_
	* GamePreprocessor.cs is responsible for generating questions and answers based on GameSetupData.cs using GenerateQuestionAndAnswer().

	
* _<a href="CodeSamples/GameSetupData.cs" target="_blank">GameSetupData.cs</a>_
	* GameSetupData.cs is a regular class which is instantiated when the GameSetup scene is loaded. It keeps track of all the relevant variables needed to create a single round of a game.

* _<a href="CodeSamples/GameInstance.cs" target="_blank">GameInstance.cs</a>_
	* GameInstance.cs is instantiated once an actual round of a game starts. It handles all of the Text and Image component updates, animations, scoring, the game timer, and win/loss detection.
	* It takes the list of QuestionAndAnswer objects and displays the first one for the player to answer. If the user answers correctly, the question iterates,  animations and a sound play to let the user know they answered correctly, and the UI components (answer area, question text, score area) update to the next question after the animation has stopped. Once the player 

<br>

### Technical Details:
* 2D mobile game
* Built from scratch using the Unity Game Engine and C# scripting
* Close to 2000 lines of C# code across 7 scripts
* Responsive UI

<br>

### Screenshots - Menus
* ![Screenshot](/Screenshots/Screenshot_Menus.jpg)</li>

### Screenshots - In-Game
* ![Screenshot](/Screenshots/Screenshot_Game.jpg)</li>

<br>

### Web Links:
* <a href="https://play.google.com/store/apps/details?id=com.TeraKeySoftware.QuickMathTrainer" target="_blank">Google play store</a>
* <a href="https://www.harjindercheema.com" target="_blank">My Website</a>
