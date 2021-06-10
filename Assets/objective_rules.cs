using UnityEngine;
using System.Collections;

public class objective_rules : MonoBehaviour {

	Animator animator;
	private bool used;

	// Use this for initialization
	void Start () {
		animator = GetComponent<Animator> ();

		if (Master_creator.first_game) {
			used = true;
			Player_controller.control.game_paused = true;
			animator.SetBool ("enter", true);
		} else {
			used = false;
			animator.SetBool ("enter", false);
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (used) {
			if (!Master_creator.first_game || Home_menu.main_menu.menu_active)
			{
				used = false;
				animator.SetBool ("exit", true);
				animator.SetBool ("enter", false);
			}
		}
	}
}
