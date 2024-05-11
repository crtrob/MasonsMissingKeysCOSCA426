// Title: PlayerMovement.cs
// Description: script to move the player character in Mason's Missing Keys
// Instruction Credit: "Mister Taft Creates" @ YouTube
// Author, Adjusting, Commenting: Carter Roberts
// Professor, Class: Omar EL Khatib, Game Programming
// Date Created: 5/6/2024 (MM/DD/YYYY)
// Date Modified: 5/10/2024 (MM/DD/YYYY)

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

    // small state enum for clarification of current state at any given moment
public enum PlayerState
{
    walk,
    attack,
    interact,
    stagger,
    idle
}
public class PlayerMovement : MonoBehaviour
{
    // player's current state
    public PlayerState currentState;
    
    // player's movement speed
    public float moveSpeed = 4.0f;

    // Has the player picked up the sword?
    [HideInInspector]
    public bool playerHasSword = false;

    // pointer to owner's rigidbody component
    private Rigidbody2D myRigidbody;

    // pointer to owner's animator
    private Animator myAnimator;

    // pointer to audio source to play sfx
    private AudioSource myAudioSource;

    // amount of change in player's position with movement
    private Vector3 positionChange;

    // max health of player
    private float maxHealth = 4;

    // current health of player
    public float currentHealth;

    
    // SFX
    public AudioClip swordSwingFX;
    public AudioClip takeDamageFX;
    public AudioClip dieFX;
    public AudioClip victoryFX;

    // Start is called before the first frame update
    void Start()
    {
            // state should begin at walk
        currentState = PlayerState.walk;
            // you at max health RN
        currentHealth = maxHealth;

            // properly storing rigidbody reference
        myRigidbody = GetComponent<Rigidbody2D> ();
        if (myRigidbody == null)
        {
            Debug.LogWarning("No Rigidbody2D component found. Adding one...");
            myRigidbody = gameObject.AddComponent<Rigidbody2D> ();
        }

            // properly storing animator reference
        myAnimator = GetComponent<Animator> ();
        if (myAnimator == null)
        {
            Debug.LogWarning("No Animator component found. Adding one...");
            myAnimator = gameObject.AddComponent<Animator> ();
        }

            // properly storing audio source reference
        myAudioSource = GetComponent<AudioSource> ();
        if (myAudioSource == null)
        {
            Debug.LogWarning("No AudioSource component found. Adding one...");
            myAudioSource = gameObject.AddComponent<AudioSource> ();
        }

            // ensures character begins in down idle position
        myAnimator.SetFloat("moveX", 0);
        myAnimator.SetFloat("moveY", -1);
    }

    
    
    // FixedUpdate is called a certain number of times per frame
    void FixedUpdate()
    {
            // no change in position at the start   
        positionChange = Vector3.zero;  
            // change in x coordinate updates based on horizontal move input
        positionChange.x = Input.GetAxisRaw("Horizontal");
            // same for y coordinate but with vertical move input
            // "raw" is used because it doesn't use decimal half-returns
        positionChange.y = Input.GetAxisRaw("Vertical");

            // if the attack button is pressed and the current player state is NOT "attacking",
        if (Input.GetButtonDown("Attack") && currentState != PlayerState.attack && 
            currentState != PlayerState.stagger) 
        {
            if (playerHasSword) 
            {
                StartCoroutine(AttackCo());
            }
        }       
        else if (currentState == PlayerState.walk || currentState == PlayerState.idle) 
        {
            UpdateAnimationAndMove();
        }
    }
    
    // attacks
    private IEnumerator AttackCo() 
    {
            // update "attacking" bool based on if statement which starts this coroutine
        myAnimator.SetBool("attacking", true);
            // playerstate is set to attack so you can't attack mid attack
        currentState = PlayerState.attack;
            // play the sound of the sword swing
        PlaySound(swordSwingFX);
            // wait just *one* frame...
        yield return null;
            // *then*, afterwards, attacking is reset to false
        myAnimator.SetBool("attacking", false);
            // then wait 0.33 seconds, duration of attack animation
        yield return new WaitForSeconds(0.33f);
            // and boom, player state's reset to walk too
        currentState = PlayerState.walk;

    }

    // moves & updates the animation in accordance with moving
    void UpdateAnimationAndMove() 
    {
            // if the positionChange updated to indicate an input,
        if (positionChange != Vector3.zero) 
        {
                // normalize position change first so that diagonal isn't 1.41x faster
            positionChange.Normalize(); 
                // add (position change * move speed * time elapsed) to the attached
                // object's position 
            myRigidbody.MovePosition(transform.position + positionChange * moveSpeed * Time.deltaTime);
                // update moveX & moveY parameters based on positionChange x & y

                // round out change x & y to prevent multi-input using multiple attack animations
            positionChange.x = Mathf.Round(positionChange.x);
            positionChange.y = Mathf.Round(positionChange.y);

            myAnimator.SetFloat("moveX", positionChange.x);
            myAnimator.SetFloat("moveY", positionChange.y);
                // update "moving" to true, since we're now moving
            myAnimator.SetBool("moving", true);
        }
        else 
        {
                // since "else" here means not moving, set "moving" to false
            myAnimator.SetBool("moving", false);
        }
    }

    // for when you get knocked back
    public void Knock (float knockTime, float damage) 
    {
            // play sound for taking damage
        PlaySound(takeDamageFX);
            // take the damage
        currentHealth -= damage;
            // update player health if gameManager exists
        if (GameManager.gm)
        {
            GameManager.gm.UpdateHearts(currentHealth);
        }
            // if the current health of the player is over 0,
        if (currentHealth > 0)
        {
                // start coroutine for knockback
            StartCoroutine(KnockCo(knockTime));
        }
            // if you should be dead
        else
        {
                // deal knockback
            StartCoroutine(KnockCo(knockTime));
                // play the death sound
            PlaySound(dieFX);
                // if the game manager exists
            if (GameManager.gm)
            {
                GameManager.gm.ResetGame();
            }
        }
    }

    // what the above function actually does
    private IEnumerator KnockCo(float knockTime) 
    {
            // this check exists to ensure that the player still lives
        if (myRigidbody != null)
        {
                // wait as many seconds as knockback duration is set to
            yield return new WaitForSeconds(knockTime);
                // then halt all movement & reset state machine to as it was before
            myRigidbody.velocity = Vector2.zero;
                // set state machine of grabbed player to "idle"
            currentState = PlayerState.idle;
            myRigidbody.velocity = Vector2.zero;
        }
    }

    // plays sounds given to player
    public void PlaySound(AudioClip clip) 
    {
        myAudioSource.PlayOneShot(clip);
    }

    // upon getting the sword, the script on the sword will trigger this
    public void CollectSword(int amount) 
    {
        playerHasSword = true;
        if (GameManager.gm)
        {
            GameManager.gm.AddPoints(amount);
        }
    }
    
    // upon getting a coin, the script on the coin will trigger this
    public void CollectCoin(int amount) 
    {
        if (GameManager.gm) // add the points through the game manager, if it is available
			GameManager.gm.AddPoints(amount);
	}

    // for colliding with level door trigger
    private void OnTriggerEnter2D (Collider2D other) 
    {
        if (other.gameObject.CompareTag("Door") && other.isTrigger)
        {
            PlaySound(victoryFX);
		    myAnimator.SetTrigger("Victory");

		    if (GameManager.gm) // do the game manager level compete stuff, if it is available
            {
			    GameManager.gm.LevelComplete();
            }
        }
    }

    // public function to respawn the player at the appropriate location
	public void Respawn(Vector3 spawnloc) 
    {
            // just in case, reset state machine to idle
        myRigidbody.velocity = Vector2.zero;
        currentState = PlayerState.idle;
        myRigidbody.velocity = Vector2.zero;
            // put health back at max and refresh UI for it
		currentHealth = maxHealth;
        if (GameManager.gm)
        {
            GameManager.gm.UpdateHearts(currentHealth);
        }
           // put position back to spawn location specified by argument 
		this.transform.position = spawnloc;
            // set respawn trigger for animation
		myAnimator.SetTrigger("Respawn");
	}
}