using UnityEngine;
using System.Collections;

public class animations : MonoBehaviour {

	Animator animator;
	float prev_jmp;


	// Use this for initialization
	void Start () {
		animator = GetComponent<Animator> ();
		prev_jmp = 0f;
	}
	
	// Update is called once per frame
	void Update () {
		if (!animator) {
			animator = GetComponent<Animator> ();
			return;
		}
		if (Player_controller.control) {

			animator.SetFloat ("y_speed", 		Master_creator.player.GetComponent<Rigidbody2D>().velocity.y);
			animator.SetFloat ("jump_power", 	Player_controller.control.jump_power);
			animator.SetBool  ("idle", 			Master_creator.first_game && (Player_controller.control.ground_speed == 0f));
			animator.SetBool  ("jmp_release", 	prev_jmp > Player_controller.control.jump_power);
			animator.SetBool  ("in_snow", 		Player_controller.control.snow_contact);
			animator.SetBool  ("won", 			Player_controller.control.won);
			animator.SetBool  ("lost", 			Player_controller.control.lost);
			animator.SetBool  ("on_ground", 	Player_controller.control.on_rail);

			prev_jmp = Player_controller.control.jump_power;
		}
	}
}
