using UnityEngine;
using System.Collections;

/*
 * 
 */
public class Home_menu : MonoBehaviour {

	public static Home_menu main_menu;	// Makes a class object accessable thoughout the scene.
	
	Animator 	home_animator,			// Animator of the "home section"(first quadrant bars).
				continue_animator;		// Animator of the bottom right bar.
	
	public bool active_splash,			// Enables session init menus.
				menu_active,			// Enables the menu main core.
				power_bar_explain,		// Enables exlonation of the power bar   		(down left).
				stamina_bar_explain,	// Enables exlonation of the stamina bar 		(down left).
				win_explain,			// Enables exlonation of the win pannel  		(top left).
				lose_explain,			// Enables exlonation of lose win pannel 		(top left).
				difficulty_explain,		// Enables exlonation of the difficulty pannel 	(top left).
				explain,				// Routes to an explonation.
				jump_next_flipper,		// Goes to next jump explonation when flipped(moved its negative).
				energy_next_flipper,	// Goes to next energy explonation when flipped(moved its negative).
				difficulty_next_flipper,// Goes to next difficulty explonation when flipped(moved its negative).
				reset_all;				// Negates all booleans above. 

	// Use this for initialization
	void Awake () {
		if (main_menu == null)
			main_menu = this;
		else if (main_menu != this)
			Destroy (gameObject);
	}

	// Use this for initialization
	void Start () {

		active_splash = true;

		menu_active 		= false;
		power_bar_explain 	= false;
		stamina_bar_explain = false;
		win_explain 		= false;
		lose_explain 		= false;
		difficulty_explain 	= false;
		explain		 		= false;
		jump_next_flipper 	= false;
		energy_next_flipper = false;
		difficulty_next_flipper = false;

		reset_all 			= false;
		
		home_animator = transform.parent.GetComponent<Animator> ();
		foreach (Transform child in transform.parent.parent)
		{
			if(child.name == "continue_canvas")
			{
				foreach(Transform child2 in child.transform)
				{
					if(child2.name == "Continue_cube")
						continue_animator = child2.GetComponent<Animator> ();
				}
			}
		}
	}

	public void activate_menu()
	{
		menu_active = !menu_active;
		InGameBars.ingamebars.Active = !InGameBars.ingamebars.Active;
		if (InGameBars.ingamebars.Active)
			InGameBars.ingamebars.ActivateBars ();
		else
			InGameBars.ingamebars.DeActivateBars ();
	}

	public void erase_saved_data()
	{
		Player_controller.control.wins   = 0;
		Player_controller.control.losses = 0;
		Player_controller.control.Save ();
		Destroy (Master_creator.game_played);
		Master_creator.rebuild = true;
	}
	
	public void enter_erase()
	{
		home_animator.SetBool ("erase_enable", true);
	}

	public void leave_erase()
	{
		home_animator.SetBool ("erase_enable", false);
	}

	public void exit_game()
	{
		Application.Quit ();
	}

	public void enter_quit()
	{
		home_animator.SetBool ("quit_enable", true);
	}
	
	public void leave_quit()
	{
		home_animator.SetBool ("quit_enable", false);
	}

	public void jump_next()
	{
		jump_next_flipper = !jump_next_flipper;
		home_animator.SetBool ("jump_next", jump_next_flipper);
		if (!jump_next_flipper) {
			power_bar_explain = false;
		}
	}

	public void energy_next()
	{
		energy_next_flipper = !energy_next_flipper;
		home_animator.SetBool ("energy_next", energy_next_flipper);
		if (!energy_next_flipper) {
			stamina_bar_explain = false;
		}
	}

	public void wins_next()
	{
		win_explain = false;
		home_animator.SetBool ("wins_next", win_explain);
	}

	public void losses_next()
	{
		lose_explain = false;
		home_animator.SetBool ("losses_next", lose_explain);

	}

	public void difficulty_next()
	{
		difficulty_next_flipper = !difficulty_next_flipper;
		home_animator.SetBool ("difficulty_next", difficulty_next_flipper);
		if (!difficulty_next_flipper) {
			difficulty_explain = false;
		}
	}

	public void flipp_sound()
	{
		MusicControl.MC.enable_sound = !MusicControl.MC.enable_sound;
		GameObject SN = GameObject.Find ("SoundNegative");
		Animator anm = SN.GetComponent<Animator> ();
		anm.SetBool ("Enable_sound", MusicControl.MC.enable_sound);
	}

	public void menu_clean_explain()
	{
		power_bar_explain 	= false;
		stamina_bar_explain = false;
		win_explain 		= false;
		lose_explain 		= false;
		difficulty_explain 	= false;
		jump_next_flipper 	= false;
		energy_next_flipper = false;
		difficulty_next_flipper = false;

		home_animator.SetBool ("jump_next", false);
		home_animator.SetBool ("energy_next", false);
		home_animator.SetBool ("difficulty_next", false);
		home_animator.SetBool ("reset_explonation", true);
	}

	public void menu_disable_all()
	{
		active_splash 		= false;
		menu_active 		= false;
		power_bar_explain 	= false;
		stamina_bar_explain = false;
		win_explain 		= false;
		lose_explain 		= false;
		difficulty_explain 	= false;
		explain		 		= false;
		jump_next_flipper 	= false;
		energy_next_flipper = false;
		difficulty_next_flipper = false;
		reset_all 			= false;
		score_menu.score.list_off();
	}

	
	// Update is called once per frame
	void Update () {

		if (reset_all) {
			menu_disable_all ();
			home_animator.SetBool ("reset_explonation", true);
		}

		AnimatorStateInfo currentBaseState = home_animator.GetCurrentAnimatorStateInfo (0);

		if (currentBaseState.IsName ("menu offscreen") ||
		    currentBaseState.IsName ("explonation"))
		{
			home_animator.SetBool ("reset_explonation", false);
		}

		explain =	power_bar_explain 	||
               		stamina_bar_explain ||
        			win_explain 		||
        			lose_explain 		||
        			difficulty_explain;

		home_animator.SetBool ("enable_menu", 		 menu_active);
		home_animator.SetBool ("jump_explain", 		 power_bar_explain);
		home_animator.SetBool ("energy_explain", 	 stamina_bar_explain);
		home_animator.SetBool ("win_explain", 		 win_explain);
		home_animator.SetBool ("lose_explain", 		 lose_explain);
		home_animator.SetBool ("difficulty_explain", difficulty_explain);

		home_animator.SetBool ("explain", explain);

		if (Master_creator.first_game || menu_active || explain) {

			if (Player_controller.control.game_paused != menu_active)
				Player_controller.control.just_paused = true;

			Player_controller.control.game_paused = true;

			if(menu_active)
				active_splash = false;


		} else {

			home_animator.SetBool ("erase_enable", false);
			home_animator.SetBool ("quit_enable", false);

			if(Player_controller.control.game_paused != menu_active)
				Player_controller.control.just_un_paused = true;

			Player_controller.control.game_paused = false;
		}

		home_animator.SetBool ("start_splash_enter", active_splash && Master_creator.first_game);

		if(menu_active)
			continue_animator.SetBool ("enter", true);
		else if (!Player_controller.control.won && !Player_controller.control.lost)
			continue_animator.SetBool ("enter", false);
	}
}
