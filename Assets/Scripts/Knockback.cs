// Title: Knockback.cs
// Description: script to grant knockback to the player's sword
// Instruction Credit: "Mister Taft Creates" @ YouTube
// Author, Adjusting, Commenting: Carter Roberts
// Professor, Class: Omar EL Khatib, Game Programming
// Date Created: 5/8/2024 (MM/DD/YYYY)
// Date Modified: 5/10/2024 (MM/DD/YYYY)

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Knockback : MonoBehaviour
{
    // power of the knockback given
    public float thrust;

    // length knocked back for
    public float knockTime;

    // damage dealt on knockback
    public float damage;

    private void OnTriggerEnter2D (Collider2D other) 
    {
            // check that it's colliding with an Enemy-tagged (or in enemy's case, Player-tagged) object
        if (other.gameObject.CompareTag("Enemy") || other.gameObject.CompareTag("Player")) 
        {
                // store a pointer of opponent's rigidbody2d and properly get it
            Rigidbody2D objectHit = other.GetComponent<Rigidbody2D> ();
                // as long as that opponent still exists,
            if (objectHit != null) 
            {
                    // store the difference between the opponent's position and the object's position,
                Vector2 difference = objectHit.transform.position - transform.position;
                    // normalize that difference and multiply it by the knockback power,
                difference = difference.normalized * thrust;
                    // apply that force with "impulse force" which is the flatter way to add force in 2D,
                objectHit.AddForce(difference, ForceMode2D.Impulse);

                    // if that opponent is an Enemy
                if (other.gameObject.CompareTag("Enemy") && other.isTrigger) 
                {
                        // set state machine of grabbed enemy to "stagger"
                    objectHit.GetComponent<EnemyBehavior>().currentState = EnemyState.stagger;
                        // call specifically the knock function from the EnemyBehavior script
                    other.GetComponent<EnemyBehavior>().Knock(objectHit, knockTime, damage);
                }
                    // if that opponent is a Player
                if (other.gameObject.CompareTag("Player") && other.isTrigger) 
                {
                        // only if the player isn't already staggered
                    if (other.GetComponent<PlayerMovement>().currentState != PlayerState.stagger)
                    {
                            // set state machine of grabbed player to "stagger"
                        objectHit.GetComponent<PlayerMovement>().currentState = PlayerState.stagger;
                            // call specifically the knock function from the PlayerMovement script
                        other.GetComponent<PlayerMovement>().Knock(knockTime, damage);
                    }
                }
            }
        }
    }
}
