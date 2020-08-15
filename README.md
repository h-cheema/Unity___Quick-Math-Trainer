<h1 align="center">Quick Math Trainer</h1>

<p align="center">
	<a href="https://play.google.com/store/apps/details?id=com.TeraKeySoftware.QuickMathTrainer" target="_blank">
	<img width="180" height="80" 
	src="https://play.google.com/intl/en_us/badges/static/images/badges/en_badge_web_generic.png" alt="">
	</a>
</p>

<br>

### Project Summary:

* Quick Math Trainer was created with the Unity Game Engine and C# scripting and is available on Android OS. It is my second larger scale project after Switchy Shapes.
* As of right now, it is a single player game. I plan on adding P2P multiplayer (using the Mirror framework) in the near future so players can play with their friends over the internet. The game scripts are already set up in a modular way which support the addition of a server and client system.
<br>

### Game Overview:

* Practice arithmetic questions against the clock. 

* Arithmetic skills are important. Practicing basic arithmetic problems daily is a solid way to improve your arithmetic skills. Quick Math Trainer allows you to do just this.

* How To Play
1. Choose one or multiple operators: Add, Subtract, Multiply, Divide.
2. Choose a difficulty setting from: Easy, Medium, Hard.
3. Answer all of the questions correctly before time runs out!

<br>

### Code Samples:

* _<a href="CodeSamples/GameSetup.cs" target="_blank">GameSetup.cs</a>_
	* Manages the "NewGame" menu scene. In this scene, the user chooses which algebraic operators they want the questions to use, how difficult the questions should be (easy, medium or hard) and whether they want sound on or off.
	* Modifies the GameSetupData.cs class directly. Once the user chooses to start the game, this is when  GamePreprocessor.cs generates all of the QuestionAndAnswer objects using the data from GameSetupData before the Game scene is loaded.
	

* _<a href="CodeSamples/GamePreprocessor.cs" target="_blank">GamePreprocessor.cs</a>_
	* Generates questions and answers based on GameSetupData.cs using GenerateQuestionAndAnswer().

	
* _<a href="CodeSamples/GameSetupData.cs" target="_blank">GameSetupData.cs</a>_
	* A regular class which is instantiated when the GameSetup scene is loaded. It keeps track of all the relevant variables needed to create a single round of a game.


* _<a href="CodeSamples/GameInstance.cs" target="_blank">GameInstance.cs</a>_
	* Is instantiated once the Game scene is loaded and an actual round of a game starts. It handles all of the Text and Image component updates, animations, scoring, the game timer, and win/loss detection.
	* It takes the list of QuestionAndAnswer objects and displays the first one for the player to answer. If the user answers correctly, the question iterates,  animations and a sound play to let the user know they answered correctly, and the UI components (answer area, question text, score area) update to the next question after the animation has stopped. Once the player either wins or loses the game, the timer is turned off and the finish screen is faded in.

<br>

### Technical Details:
* 2D mobile game
* Built from scratch using the Unity Game Engine and C# scripting
* Close to 2000 lines of C# code across 7 scripts
* Responsive UI

<br>

### Screenshots - Menus
* ![Screenshot](/Screenshots/Screenshot_Menus.jpg)</li>

<br>

### Screenshots - In-Game
* ![Screenshot](/Screenshots/Screenshot_Game.jpg)</li>

<br>

### Web Links:
* <a href="https://play.google.com/store/apps/details?id=com.TeraKeySoftware.QuickMathTrainer" target="_blank">Google play store</a>
* <a href="https://www.harjindercheema.com" target="_blank">My Website</a>
