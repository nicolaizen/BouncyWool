using UnityEngine;
using System.Collections;

public class Continue_bar : MonoBehaviour {
	
	Animator animator;

	private bool entr;
	
	// Use this for initialization
	void Start () {

		entr = true;

		animator = GetComponent<Animator> ();

		animator.SetBool ("enter", false);

		if (Master_creator.first_game) {
			Player_controller.control.game_paused = true;
			animator.SetBool ("first_game", true);
			animator.SetBool ("active_splash", true);
		}
		else
			animator.SetBool ("first_game", false);
	}

	// Update is called once per frame
	void Update () {
		if (!animator)
			return;
		if (Player_controller.control) {

			if(!Home_menu.main_menu.active_splash)
			{
				entr = true;
				animator.SetBool ("active_splash", false);
			}

			if (!Master_creator.first_game)
			{
				entr = true;
				animator.SetBool ("first_game", false);
			}

			if(Player_controller.control.won || Player_controller.control.lost)
				animator.SetBool ("enter", entr);
		}
	}
}
