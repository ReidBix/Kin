using UnityEngine;
using System.Collections;

public class MixcoatlAI : BaseGodAI {

	protected enum AIStates
	{
		Intro,
		SpearShooting,
		SpearGathering,
		BowStage,
		AnimalStage
	}

	AIStates currState;
	public float introDelay;
	float introCooldown;
	int spears;
	public int NumberOfSpears;
	EnemyHealth health;

	GameObject[] coverObjects;
	GameObject currentCover;

	float spearThrowIntroCd,spearThrowOutroCd;
	public float spearThrowIntroMaxCd, spearThrowOutroMaxCd;
	bool spearThrown;

	// Use this for initialization
	protected override void Start () {
		currState = AIStates.Intro;
		introCooldown = introDelay;
		coverObjects = GameObject.FindGameObjectsWithTag ("Cover");
		health = gameObject.GetComponent<EnemyHealth> ();
		spears = NumberOfSpears;
		spearThrowIntroCd = spearThrowIntroMaxCd;
		spearThrowOutroCd = spearThrowOutroMaxCd;
		spearThrown = false;
		currentCover = null;
	}
	
	// Update is called once per frame
	protected override void Update () {
		switch (currState) {
		case AIStates.Intro:
			introCooldown -= Time.deltaTime;
			if (introCooldown <= 0) {
				currState = AIStates.SpearShooting;
				health.isVulnerable = false;
			}
			break;
		case AIStates.SpearShooting:
			if (currentCover == null) {
				int newCoverChoice = (int)(Random.Range (0, coverObjects.Length));
				currentCover = coverObjects [newCoverChoice];
			}
			if (!spearThrown) {
				spearThrowIntroCd -= Time.deltaTime;
				if (spearThrowIntroCd <= 0) {
					//throw spear
					spears--;
					spearThrown = true;
				}
			} else {
				spearThrowOutroCd -= Time.deltaTime;
				if (spearThrowOutroCd <= 0) {
					if (spears <= 0) {
						currState = AIStates.SpearGathering;
						health.isVulnerable = true;
					} else {
						int newCoverChoice = (int)(Random.Range (0, coverObjects.Length));
						while (coverObjects [newCoverChoice] == currentCover) {
							newCoverChoice = (int)(Random.Range (0, coverObjects.Length));
						}
						currentCover = coverObjects [newCoverChoice];
						spearThrowIntroCd = spearThrowIntroMaxCd;
						spearThrowOutroCd = spearThrowOutroMaxCd;
						spearThrown = false;
					}
				}
			}
			break;
		case AIStates.SpearGathering:
			//Move to spear (?)
			//Get them spear (?)
			//Wait ????
			//Profit.
			if (spears == NumberOfSpears) {
				if (health.getHp () / health.maxHealth < (2f / 3f)) {
					currState = AIStates.BowStage;
				} else {
					currState = AIStates.SpearShooting;
					health.isVulnerable = false;
				}
			}
			break;
		case AIStates.BowStage:
			break;
		default:
			break;
		}
	}
}
