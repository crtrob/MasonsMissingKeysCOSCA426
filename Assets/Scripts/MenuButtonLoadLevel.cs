// Title: MenuButtonLoadLevel.cs
// Description: script to load the menu when the button's pressed
// Author, Adjusting, Commenting: Carter Roberts
// Professor, Class: Omar EL Khatib, Game Programming
// Date Created: 5/10/2024 (MM/DD/YYYY)
// Date Modified: 5/10/2024 (MM/DD/YYYY)

using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement; // include so we can load new scenes

public class MenuButtonLoadLevel : MonoBehaviour {

	public void loadLevel(string levelToLoad)
	{
		SceneManager.LoadScene(levelToLoad);
	}
}
