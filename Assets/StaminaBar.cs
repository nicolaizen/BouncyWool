using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StaminaBar : MonoBehaviour {

	private Image[] img;
	
	Animator animator;
	public bool active;


	// Use this for initialization
	void Start () {
		img = gameObject.GetComponentsInChildren<Image>();
		active = true;
	}

	public void activate_explanation()
	{
		Home_menu.main_menu.menu_clean_explain ();
		Home_menu.main_menu.stamina_bar_explain = true;
	}
	
	// Update is called once per frame
	void Update () {

		bool 	SW = false;
		float 	remain;

		if (Player_controller.control) {
			remain = (Player_controller.control.stamina) / 100;
			img[0].fillAmount = remain;

			foreach (Transform child in transform) {

				if(child.name != "Image")
				{
					animator = child.GetComponentInChildren<Animator> ();

					switch (child.name)
					{
					case "e":
						SW = remain < 0.072f;
							break;
					case "n":
						SW = remain < 0.188f;
						break;
					case "e 1":
						SW = remain < 0.315f;
						break;
					case "r":
						SW = remain < 0.441f;
						break;
					case "g":
						SW = remain < 0.581f;
						break;
					case "y":
						SW = remain < 0.735f;
						break;
					}
					
					animator.SetBool ("stop_wobble", SW);
				}
			}
		}
	}
}
