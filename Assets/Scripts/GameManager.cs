// Title: GameManager.cs
// Description: script to manage the game's canvas, score, lives, health & pausing
// Author, Adjusting, Commenting: Carter Roberts
// Professor, Class: Omar EL Khatib, Game Programming
// Date Created: 5/9/2024 (MM/DD/YYYY)
// Date Modified: 5/10/2024 (MM/DD/YYYY)

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.Animations;

public class GameManager : MonoBehaviour
{

    // static reference to game manager so can be called from other scripts directly (not just through gameobject component)
	public static GameManager gm;

    // levels to move to on victory and lose
	public string levelAfterVictory;
	public string levelAfterGameOver;

    // game performance
	public int score = 0;
	public int highscore = 0;
	public int startLives = 2;
	public int lives = 2;

    // Door to control based on key
    public GameObject levelDoor;

    // UI elements to control
	public TMP_Text UIScore;
	public TMP_Text UIHighScore;
	public TMP_Text UILevel;
    public Image keyUI;
	public Image[] heartContainers;
    public GameObject[] UIExtraLives;
	public GameObject UIGamePaused;
    // pointer to full/half/empty heart sprites
    public Sprite fullHeart;
    public Sprite halfHeart;
    public Sprite emptyHeart;
    // pointer to key sprite for canvas / ui
    public Sprite keyForUI;

    // private variables
	GameObject _player;
	Vector3 _spawnLocation;
	Scene _scene;

    // Awake is called when the script is first loaded
    void Awake()
    {
        // setup reference to game manager
		if (gm == null)
			gm = this.GetComponent<GameManager>();

        // call in-script func to initialize variables
        setupDefaults();
    }

    // Update is called once per frame
    void Update()
    {
        // if ESC pressed then pause the game
		if (Input.GetKeyDown(KeyCode.Escape)) {
			if (Time.timeScale > 0f) {
				UIGamePaused.SetActive(true); // this brings up the pause UI
				Time.timeScale = 0f; // this pauses the game action
			} else {
				Time.timeScale = 1f; // this unpauses the game action (ie. back to normal)
				UIGamePaused.SetActive(false); // remove the pause UI
			}
		}
    }

    // sets up default values for variables
    void setupDefaults() {
		// setup reference to player
		if (_player == null)
			_player = GameObject.FindGameObjectWithTag("Player");
		
		if (_player == null)
			Debug.LogError("Player not found in Game Manager");

		// get current scene
		_scene = SceneManager.GetActiveScene();

		// get initial _spawnLocation based on initial position of player
		_spawnLocation = _player.transform.position;

		// if levels not specified, default to current level
		if (levelAfterVictory == "") {
			Debug.LogWarning("levelAfterVictory not specified, defaulted to current level");
			levelAfterVictory = _scene.name;
		}
		
		if (levelAfterGameOver == "") {
			Debug.LogWarning("levelAfterGameOver not specified, defaulted to current level");
			levelAfterGameOver = _scene.name;
		}

		// friendly error messages
        if (levelDoor == null)
			Debug.LogError ("Need to set levelDoor on Game Manager.");

		if (UIScore == null)
			Debug.LogError ("Need to set UIScore on Game Manager.");
		
		if (UIHighScore == null)
			Debug.LogError ("Need to set UIHighScore on Game Manager.");
		
		if (UILevel == null)
			Debug.LogError ("Need to set UILevel on Game Manager.");

        if (keyUI == null)
            Debug.LogError ("Need to set keyUI on Game Manager.");
		
		if (UIGamePaused == null)
			Debug.LogError ("Need to set UIGamePaused on Game Manager.");
		
        // initialize heart counter
        InitHearts();

        // get stored player prefs
		refreshPlayerState();

		// get the UI ready for the game
		refreshGUI();
	}

    // initialize hearts on canvas
    void InitHearts()
    {
            // iterate through number of heart containers
        for (int i = 0; i < heartContainers.Length; i++)
        {
                // activate the heart as it's being used
            heartContainers[i].gameObject.SetActive(true);
                //  they're full hearts since you start with full health
            heartContainers[i].sprite = fullHeart;
        }
    }

    // update hearts on canvas (activated by PlayerMovement.Knock through static gm)
    public void UpdateHearts(float currentHealth)
    {
            // create temporary value for health that is current health / 2 since 2hp = 1 container
        float tempHealth = currentHealth / 2;
            // iterate through number of heart containers
        for (int i = 0; i < heartContainers.Length; i++) 
        {
                // if i at its current value is less than or equal to (the temp value for health created - 1)
            if (i <= tempHealth - 1)
            {
                heartContainers[i].sprite = fullHeart;
            }
                // if i at its current value is greater than or equal to the temp value for health created
            else if (i >= tempHealth)
            {
                heartContainers[i].sprite = emptyHeart;
            }
                // if i is neither of these 
            else
            {
                heartContainers[i].sprite = halfHeart;
            }
        }
    }

    // get stored Player Prefs if they exist, otherwise go with defaults set on gameObject
	void refreshPlayerState() {
        lives = PlayerPrefManager.GetLives();

		// special case if lives <= 0 then must be testing in editor, so reset the player prefs
		if (lives <= 0) 
        {
			PlayerPrefManager.ResetPlayerState(startLives,false);
			lives = PlayerPrefManager.GetLives();
		}
		score = PlayerPrefManager.GetScore();
		highscore = PlayerPrefManager.GetHighscore();

		// save that this level has been accessed so the MainMenu can enable it
		PlayerPrefManager.UnlockLevel();
	}

    // refresh all the GUI elements
	void refreshGUI() {
		// set the text elements of the UI
		UIScore.text = "Score: "+score.ToString();
		UIHighScore.text = "Highscore: "+highscore.ToString ();
		UILevel.text = _scene.name;
        
		// turn on the appropriate number of life indicators in the UI based on the number of lives left
		for(int i = 0 ; i < UIExtraLives.Length ; i++) {
			if (i < (lives-1)) { // show one less than the number of lives since you only typically show lifes after the current life in UI
				UIExtraLives[i].SetActive(true);
			} else {
				UIExtraLives[i].SetActive(false);
			}
		}
	}

    // public function to add points and update the gui and highscore player prefs accordingly
	public void AddPoints(int amount)
	{
		// increase score
		score+=amount;

		// update UI
		UIScore.text = "Score: "+score.ToString();

		// if score>highscore then update the highscore UI too
		if (score > highscore) {
			highscore = score;
			UIHighScore.text = "Highscore: "+score.ToString();
		}
	}

    
    // public function to set "hasKey" to true and update gui & door accordingly
    public void AddKey()
    {
        keyUI.sprite = keyForUI;
		keyUI.enabled = true;
        if (levelDoor)
        {
            Animator doorAnimatorRef = levelDoor.GetComponent<Animator> ();
            if (doorAnimatorRef == null)
            {
                Debug.LogWarning("No Animator component found... Add one yourself!");
                    // I just didn't wanna deal with also making the trigger, the transition
                    // and all that within .cs; instead this warning will just tell the user
                    // to put an animator on levelDoor
            }
                // open the door thru animator, and the animation changes the colliders itself
            doorAnimatorRef.SetTrigger("keyAcquired");
        }
        
    }
    

    // public function to remove player life and reset game accordingly
	public void ResetGame() {
        // remove life and update GUI
		lives--;
		refreshGUI();
        
		if (lives<=0) { // no more lives
			// save the current player prefs before going to GameOver
			PlayerPrefManager.SavePlayerState(score, highscore, lives);

			// load the gameOver screen
			SceneManager.LoadScene(levelAfterGameOver);
		} else { // tell the player to respawn
			_player.GetComponent<PlayerMovement>().Respawn(_spawnLocation);
		}
	}

    // public function for level complete
	public void LevelComplete() {
		// save the current player prefs before moving to the next level
		PlayerPrefManager.SavePlayerState(score,highscore,lives);

		// use a coroutine to allow the player to get fanfare before moving to next level
		StartCoroutine(LoadNextLevel());
	}

	// load the nextLevel after delay for sound & animator
	IEnumerator LoadNextLevel() {
		yield return new WaitForSeconds(2f);
		SceneManager.LoadScene(levelAfterVictory);
	}
}  
