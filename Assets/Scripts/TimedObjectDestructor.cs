// Title: TimedObjectDestructor.cs
// Description: script for explosions to destroy themselves after a second and a half
// Author, Adjusting, Commenting: Carter Roberts
// Professor, Class: Omar EL Khatib, Game Programming
// Date Created: 5/10/2024 (MM/DD/YYYY)
// Date Modified: 5/10/2024 (MM/DD/YYYY)

using UnityEngine;
using System.Collections;

public class TimedObjectDestructor : MonoBehaviour {

	public float timeOut = 1.6f;
	public bool detachChildren = false;

	// invote the DestroyNow funtion to run after timeOut seconds
	void Awake () {
		Invoke ("DestroyNow", timeOut);
	}

	// destroy the gameobject
	void DestroyNow ()
	{
		if (detachChildren) { // detach the children before destroying if specified
			transform.DetachChildren ();
		}

		// destroy the game Object
		Destroy(gameObject);
	}
}
