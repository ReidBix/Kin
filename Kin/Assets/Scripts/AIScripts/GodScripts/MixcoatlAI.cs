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
	public int spears;

	// Use this for initialization
	protected override void Start () {
		currState = AIStates.Intro;
		introCooldown = introDelay;
	}
	
	// Update is called once per frame
	protected override void Update () {
		switch (currState) {
		case AIStates.Intro:
			introCooldown -= Time.deltaTime;
			if (introCooldown <= 0) {
				currState = AIStates.SpearShooting;
			}
			break;
		case AIStates.SpearShooting:
			//IF READY TO SHOOT SPEARS
			//MAKE SPEAR PROJECTILE + SHOOT IT
			//spears--
			if (spears <= 0) {
				currState = AIStates.SpearGathering;
			} else {
				//MOVE TO NEW LOCATION
			}
			break;
		case AIStates.SpearGathering:
			//Move to spear (?)
			//Get them spear (?)
			//Wait ????
			//Profit.
			break;
		case AIStates.BowStage:
			break;
		default:
			break;
		}
	}
}
