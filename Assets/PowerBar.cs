using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq;

public class PowerBar : MonoBehaviour {


	public static PowerBar PB;

	private Image[] img;
	
	Animator 	animator,
				arrow_animator;

	private bool wind_flipp;

	public float 	letter_scale,
					power;

	private string[] 	jump_word,
						wind_word;


	// Use this for initialization
	void Awake () {
		if (PB == null)
			PB = this;
		else if (PB != this)
			Destroy (gameObject);
	}

	// Use this for initialization
	void Start () {
		img = gameObject.GetComponentsInChildren<Image>();
		wind_flipp = false;
		power = 0f;

		arrow_animator = GameObject.Find ("arrow").GetComponent<Animator> ();

		letter_scale = GameObject.Find ("Jump_Box/j").transform.localScale.x;

		jump_word = new string[]{ "j", "u", "m", "p" };
		wind_word = new string[]{ "w", "i", "n", "d" };
	}

	public void activate_explanation()
	{
		Home_menu.main_menu.menu_clean_explain ();
		Home_menu.main_menu.power_bar_explain = true;
	}

	void minimize_letter(Transform tran)
	{
		tran.localScale = new Vector3(0,0,0);
	}

	void maximize_letter(Transform tran)
	{
		tran.localScale = new Vector3(letter_scale,letter_scale,1);
	}
	
	// Update is called once per frame
	void Update () {

		bool 	SW = false;

		if (Player_controller.control) {
			power = (Player_controller.control.jump_power) / 100;

			arrow_animator.enabled = false;

			if(!Player_controller.control.on_rail)
			{
				if(!Player_controller.control.won && !Player_controller.control.lost)
					arrow_animator.enabled = true;

				power = Player_controller.control.build_jump ? 1 : 0;

				if(!wind_flipp && power == 1)
				{
					wind_flipp = true;

					foreach(Transform child in transform)
					{
						if(jump_word.Contains(child.name))
							minimize_letter(child.transform);
						else if(wind_word.Contains(child.name))
							maximize_letter(child.transform);
					}
				}
			}
			else if(wind_flipp)
			{
				wind_flipp = false;
				
				foreach(Transform child in transform)
				{
					if(jump_word.Contains(child.name))
						maximize_letter(child.transform);
					else if(wind_word.Contains(child.name))
						minimize_letter(child.transform);
				}

			}
			
			img[0].fillAmount = power;

			foreach (Transform child in transform) {

				if(child.name != "Image")
				{
					animator = child.GetComponentInChildren<Animator> ();

					switch (child.name)
					{
					case "j":
						SW = power > 0f;
						break;
					case "u":
						SW = power > 0.265f;
						break;
					case "m":
						SW = power > 0.483f;
						break;
					case "p":
						SW = power > 0.672f;
						break;
					case "w":
						SW = power > 0f;
						break;
					case "i":
						SW = power > 0.265f;
						break;
					case "n":
						SW = power > 0.483f;
						break;
					case "d":
						SW = power > 0.672f;
						break;
					}
					animator.SetBool ("start_wobble", SW);
				}
			}
		}
	}
}
