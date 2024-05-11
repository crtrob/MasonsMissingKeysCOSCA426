// Title: Coin.cs
// Description: script for the sword item to be picked up & consumed
// Author, Adjusting, Commenting: Carter Roberts
// Professor, Class: Omar EL Khatib, Game Programming
// Date Created: 5/10/2024 (MM/DD/YYYY)
// Date Modified: 5/10/2024 (MM/DD/YYYY)

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    // so it can't be grabbed multiple times
    public bool taken = false;
	// value of the sword
	public int value = 6;
	public AudioClip getFX;
    // explosion to create on pickup
    public GameObject explosion;

    // if the player touches the sword, and it has not already been taken, then take the sword
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
	
				// do the collect sword thing
					// play the sound
				other.GetComponent<PlayerMovement>().PlaySound(getFX);
				other.GetComponent<PlayerMovement>().CollectSword(value);
        }

				// destroy the sword
			Object.Destroy(this.gameObject);
		}
}
