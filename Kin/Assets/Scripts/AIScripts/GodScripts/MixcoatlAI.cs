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

	// Use this for initialization
	protected override void Start () {
		currState = AIStates.Intro;
	}
	
	// Update is called once per frame
	protected override void Update () {
		switch (currState) {
		case AIStates.Intro:
			break;
		default:
			break;
		}
	}
}
