// Title: EnemyBehavior.cs
// Description: script controlling enemy behavior for any enemy in Mason's Missing Keys
// Instruction Credit: "Mister Taft Creates" @ YouTube
// Author, Adjusting, Commenting: Carter Roberts
// Professor, Class: Omar EL Khatib, Game Programming
// Date Created: 5/8/2024 (MM/DD/YYYY)
// Date Modified: 5/10/2024 (MM/DD/YYYY)

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum EnemyState
{
    idle,
    walk,
    attack,
    stagger
}

public class EnemyBehavior : MonoBehaviour
{  
    // instance of enemy state machine option
    public EnemyState currentState;

    // maximum possible health
    public float maxHealth;

    // enemy's health pool
    public float health;

    // enemy's name
    public string enemyName;

    // enemy's movement speed
    public float moveSpeed;

    // score given on death
    public int grantedScore;

    // explosion upon death
    public GameObject explosion;

    // pointer to audio source to play sounds with
    private AudioSource myAudioSource;

    // SFX
    public AudioClip hitFX;
    public AudioClip dieFX;

    // Awake is called when the script is loaded
    private void Awake()
    {
        health = maxHealth;
        
            // properly storing audio source reference
        myAudioSource = GetComponent<AudioSource> ();
        if (myAudioSource == null)
        {
            Debug.LogWarning("No AudioSource component found. Adding one...");
            myAudioSource = gameObject.AddComponent<AudioSource> ();
        }
    }

    // calls coroutine which inflicts damage upon enemy
    private void takeDamage(float damage)
    {
        StartCoroutine(DamageCo(damage));
    }

    // coroutine to inflict damage upon enemy
    private IEnumerator DamageCo(float damage)
    {
        health -= damage;
        if (health > 0)
            PlaySound(hitFX);
        if (health <= 0)
        {
            PlaySound(dieFX);
                // just so the vision & trigger don't linger
            this.gameObject.GetComponent<SpriteRenderer>().enabled = false;
            this.gameObject.GetComponent<Collider2D>().enabled = false;
                // if an explosion was included
            if (explosion)
            {
                    // create the explosion
                Instantiate(explosion, transform.position, transform.rotation);
            }

            if (GameManager.gm)
            {
                GameManager.gm.AddPoints(grantedScore);
            }
                // wait for sound to play
            yield return new WaitForSeconds(0.8f);
            this.gameObject.SetActive(false);
            Destroy(this.gameObject);
        }

    }

    // calls coroutine which deals knockback to enemy
    public void Knock(Rigidbody2D myRigidbody, float knockTime, float damage) 
    {
            // deal the knockback
        StartCoroutine(KnockCo(myRigidbody, knockTime));
            // deal the damage of the knockback
        takeDamage(damage);
    }

    // coroutine which deals knockback to enemy
    private IEnumerator KnockCo(Rigidbody2D myRigidbody, float knockTime) 
    {
            // this check exists to ensure that the enemy hasn't died already & isn't already staggered
        if (myRigidbody != null)
        {
                // wait as many seconds as knockback duration is set to
            yield return new WaitForSeconds(knockTime);
                // then halt all movement & reset state machine to as it was before
            myRigidbody.velocity = Vector2.zero;
                // set state machine of grabbed enemy to "idle"
            myRigidbody.GetComponent<EnemyBehavior>().currentState = EnemyState.idle;
            myRigidbody.velocity = Vector2.zero;
        }
    }

    // plays sounds given to enemy
    void PlaySound(AudioClip clip) 
    {
        myAudioSource.PlayOneShot(clip);
    }
}
