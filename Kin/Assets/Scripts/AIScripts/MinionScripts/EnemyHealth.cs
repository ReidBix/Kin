using UnityEngine;
//using UnityEditor;
using System.Collections;

public class EnemyHealth:MonoBehaviour
{
    public int maxHealth = 100;
    int currentHealth;
    bool isDead;
	public bool isVulnerable;

    void Start()
    {
        currentHealth = maxHealth;
        isDead = false;
		isVulnerable = true;
    }
    public void takeDamage(int amount)
    {
		if (isVulnerable) {
			currentHealth -= amount;
			Debug.Log ("Took Damage");
		}
        // Play damage audio clip
       // if (currentHealth <= 0 && !isDead)
        //{
          //  Death();
        //}
    }

	public int getHp()
	{
		return currentHealth;
	}


}