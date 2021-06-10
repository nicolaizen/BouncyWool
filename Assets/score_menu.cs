using UnityEngine;
using System.Collections;

public class score_menu : MonoBehaviour {

	public static score_menu score;
		
	Animator animator;

	// Use this for initialization
	void Awake () {
		if (score == null)
			score = this;
		else if (score != this)
			Destroy (gameObject);
	}

	public void activate_explanation_wins()
	{
		Home_menu.main_menu.menu_clean_explain ();
		Home_menu.main_menu.win_explain = true;
	}

	public void activate_explanation_losses()
	{
		Home_menu.main_menu.menu_clean_explain ();
		Home_menu.main_menu.lose_explain = true;
	}

	public void activate_explanation_difficulty()
	{
		Home_menu.main_menu.menu_clean_explain ();
		Home_menu.main_menu.difficulty_explain = true;
	}
	
	public void list_on()
	{
		animator.SetBool ("list_score", true);
	}

	public void list_off()
	{
		if(!Home_menu.main_menu.menu_active)
			animator.SetBool ("list_score", false);
	}

	public void menu_score()
	{
		animator.SetBool ("list_score", Home_menu.main_menu.menu_active);
	}

	// Use this for initialization
	void Start () {
		animator = transform.GetComponent<Animator> ();
		if (Master_creator.first_game) {
			animator.SetBool ("list_score", true);
			Invoke ("list_off", 4f);
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (!animator)
			return;
		if (Player_controller.control) {
			if(Player_controller.control.won || Player_controller.control.lost)
				animator.SetBool ("list_score", true);
		}
	}
}
