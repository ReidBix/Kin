﻿using UnityEngine;
using UnityEditor;
using System.Collections;

public class Kamikaze : BaseMinionAI
{
    
    float explodeRadius; //radius where kamikaze stops and explodes
    bool isExploding;
    float explodeDelay;  //time delay between stopping and exploding
    float timeToExplode; //buildup to the explosion
	public int explodeDamage;

    protected new void Start()
    {
        //Establish rigid body for minion
        rb = gameObject.GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("AI has no RigidBody. AI name is " + gameObject.name + "!");
        }
        if (targetObject == null)
        {
            Debug.LogError("AI has no target. AI name is " + gameObject.name + "!");
        }

        curState = AIStates.IdleState;

        explodeRadius = .2f;
        timeToExplode = 0.0f;
        explodeDelay = .8f;
        awarenessRadius = 1.0f;
    }

    protected new void Update()
    {
        float distanceToPlayer = Vector2.Distance((Vector2)targetObject.transform.position, (Vector2)gameObject.transform.position);

        if (curState == AIStates.DetectedState)
        {
            if (distanceToPlayer < explodeRadius)
            {
                isExploding = true;
                rb.velocity = Vector2.zero;
            }
            if (!isExploding)
                MoveTowardsTarget();
            else
            { 
				if (timeToExplode > explodeDelay) {
					if (distanceToPlayer < explodeRadius) {
						targetObject.GetComponent<PlayerHealth> ().TakeDamage (explodeDamage);
					}
					
					Destroy (gameObject);
				}
                else
                {
                    timeToExplode += Time.deltaTime;
                }
            }
        }
        else
        {
            if (distanceToPlayer < awarenessRadius)
                curState = AIStates.DetectedState;
        }
    }
}