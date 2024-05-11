// Title: Key.cs
// Description: script for the key item to be picked up & consumed
// Author, Adjusting, Commenting: Carter Roberts
// Professor, Class: Omar EL Khatib, Game Programming
// Date Created: 5/10/2024 (MM/DD/YYYY)
// Date Modified: 5/10/2024 (MM/DD/YYYY)

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour
{
    // so it can't be grabbed multiple times
    public bool taken = false;
	public AudioClip getFX;
    // explosion to create on pickup
    public GameObject explosion;
    
    // if the player touches the key, and it has not already been taken, then take the key
	void OnTriggerEnter2D (Collider2D other)
	{
		if (other.tag == "Player"  && !taken)
		{
				// mark as taken so doesn't get taken multiple times
			taken=true;

				// if explosion prefab is provide, then instantiate it
			if (explosion)
			{
				Instantiate(explosion,transform.position,transform.rotation);
			}
	
				// do the collect key thing
			if (GameManager.gm)
            {
					// play the sound
				other.GetComponent<PlayerMovement>().PlaySound(getFX);
				GameManager.gm.AddKey();
            }

				// destroy the key
			Object.Destroy(this.gameObject);
		}
	}
}
