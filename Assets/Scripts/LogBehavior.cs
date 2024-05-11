// Title: LogBehavior.cs
// Description: script controlling behavior for log enemy in Mason's Missing Keys
// Instruction Credit: "Mister Taft Creates" @ YouTube
// Author, Adjusting, Commenting: Carter Roberts
// Professor, Class: Omar EL Khatib, Game Programming
// Date Created: 5/8/2024 (MM/DD/YYYY)
// Date Modified: 5/10/2024 (MM/DD/YYYY)

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// inheriting everything from EnemyBehavior.cs automatically
public class LogBehavior : EnemyBehavior
{
    // pointer to own rigidbody
    private Rigidbody2D myRigidbody;

    // target to chase
    public Transform target;

    // square radius of object where, if target enters, it will chase its target
    public float chaseRadius;

    // square radius of object where, if target enters, it will attack its target
    public float attackRadius;

    // pointer to object's animator
    public Animator Animator;

    // Start is called before the first frame update
    void Start()
    {
        currentState = EnemyState.idle;
            // complete pointer to own rigidbody
        myRigidbody = GetComponent<Rigidbody2D> ();

            // complete pointer to own animator
        Animator = GetComponent<Animator> ();

            // set target to player-tagged object
        target = GameObject.FindWithTag("Player").transform;
    }

    // FixedUpdate is called a set number of times per frame
    void FixedUpdate()
    {
        checkDistance();   
    }

    // checks distance from target and approaches if necessary
    void checkDistance() 
    {
            // if the distance between the target's position and the object's position <= the chase radius,
            // but it's > the attack radius (if it's between the chase and attack radii)
        if (Vector3.Distance(target.position, transform.position) <= chaseRadius &&
            Vector3.Distance(target.position, transform.position) > attackRadius) 
        {
                // and if the enemy is currently in "idle" or "walk" state,
            if (currentState == EnemyState.idle || currentState == EnemyState.walk && currentState != EnemyState.stagger)
            {
                    // a spot between object's current position and its target's position
                    // where it can only move as far as its (movement speed * elapsed time)
                Vector3 temp = Vector3.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);
                    // put that spot minus the enemy's current position into the function to change
                    // animation state as direction; normalized to reduce code length
                ChangeAnim(Vector3.Normalize(temp - transform.position));
                    // move position of object to (see comment/s 2 lines of code up)
                myRigidbody.MovePosition(temp);
                    // change enemy's state to "walk" if it isn't walking already
                ChangeState(EnemyState.walk);
                    // update animator to match this
                Animator.SetBool("wakeUp", true);
            }
        }
            // if the distance between the target's position and the object's position is more than
            // the chaseRadius,
        else if (Vector3.Distance(target.position, transform.position) > chaseRadius)
        {
                // go to beddy bye bye :-)
            Animator.SetBool("wakeUp", false);
        }
    }

    // changes animation state
    private void ChangeAnim(Vector3 direction) 
    {
        Animator.SetFloat("moveX", direction.x);
        Animator.SetFloat("moveY", direction.y);
    }

    // changes state in state machine
    private void ChangeState(EnemyState newState) 
    {
        if (currentState != newState)
        {
            currentState = newState;
        }
    }
}
