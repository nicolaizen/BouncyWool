using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class letter_animations : MonoBehaviour {

	public GameObject the_copy;

	private bool updated_this_round;

	GameObject number_to_objects (float val, bool int_type, int decimal_count)
	{
		GameObject number = new GameObject();
		GameObject[] arr;

		float letter_distance = 1.8f;

		string number_name = "number: ";

		int pre_point  = 1;
		int arr_length = 0;
		if(val >= 1)
			pre_point += (int)Mathf.Log10 (val);
		if (!int_type) {
			arr_length += decimal_count;
			arr_length++;
		}

		arr_length += pre_point;
		arr = new GameObject[arr_length];

		float a, b;
		int c;
		string d;
		for (c = 0; c < pre_point; c++)
		{
			a = val % Mathf.Pow(10, (pre_point-c));
			b = Mathf.Pow(10, (pre_point-c-1));
			d = (Mathf.Floor(a/b)).ToString();

			number_name += d;

			arr[c] = (GameObject)Instantiate (Resources.Load ("symbols/" + d)); 
			arr[c].transform.SetParent(number.transform);
			arr[c].transform.position = new Vector3((c*letter_distance),0,0);
		}
		if (!int_type) {
			arr[c] = (GameObject)Instantiate (Resources.Load ("symbols/point"));
			arr[c].transform.SetParent(number.transform);


			arr[c].transform.position = new Vector3((c*letter_distance),-1,0);

			number_name += ".";

			for(c = 0; c < decimal_count; c++)
			{
				a = Mathf.Floor ((val * Mathf.Pow(10, (c+1))) % 10);
				d = a.ToString();

				number_name += d;

				arr[pre_point + 1 + c] = (GameObject)Instantiate (Resources.Load ("symbols/" + d)); 
				arr[pre_point + 1 + c].transform.SetParent(number.transform);
				arr[pre_point + 1 + c].transform.position = new Vector3(((pre_point + 1 + c)*letter_distance),0,0);
			}
		}
		number.name = number_name;
		return number;
	}

	GameObject copy_the_copy()
	{
		if (the_copy) {
			GameObject new_copy = (GameObject)Instantiate (the_copy);
			return new_copy;
		} else
			return new GameObject();
	}

	void number_to_images (float val, bool int_type, int decimal_count)
	{
		string atlasPath = "symbols/numbers";
		Sprite[] atlasAssets;
		atlasAssets = Resources.LoadAll<Sprite>(atlasPath);

		Transform main_parent = transform;


		Image img;

		foreach(RectTransform child in transform)
		{
			if(child.name == "Number")
			{
				foreach(RectTransform child2 in child.transform)
				{
					if(child2.name == "copy_this")
					{
						the_copy = (GameObject)Instantiate(child2.gameObject);
						the_copy.name = "the_copy";

						main_parent   = child.transform;
						the_copy.transform.SetParent(main_parent);
						child2.localScale = new Vector3(0,0,0);

						the_copy.transform.localPosition 	= new Vector3(0,0,0);
						the_copy.transform.localScale 		= new Vector3(1,1,1);
						break;
					}
				}
				break;
			}
		}

		GameObject[] arr;
		float letter_distance = 25f;
		
		int pre_point  = 1;
		int arr_length = 0;
		if(val >= 1)
			pre_point += (int)Mathf.Log10 (val);
		if (!int_type) {
			arr_length += decimal_count;
			arr_length++;
		}
		
		arr_length += pre_point;
		arr = new GameObject[arr_length];
		
		float a, b;
		int   c, d;

		for (c = 0; c < pre_point; c++)
		{
			a = val % Mathf.Pow(10, (pre_point-c));
			b = Mathf.Pow(10, (pre_point-c-1));
			d = (int)(Mathf.Floor(a/b));

			arr[c] = copy_the_copy();
			arr[c].name = d.ToString();

			img = arr[c].GetComponentInChildren<UnityEngine.UI.Image>();
			img.sprite = atlasAssets[d];

			arr[c].transform.SetParent(main_parent);
		}
		if (!int_type) {

			arr[c] = copy_the_copy();
			arr[c].transform.SetParent(main_parent);

			arr[c].name = "point";
			
			img = arr[c].GetComponentInChildren<UnityEngine.UI.Image>();
			img.sprite = atlasAssets[12];
			
			for(c = 0; c < decimal_count; c++)
			{
				d = (int)Mathf.Floor ((val * Mathf.Pow(10, (c+1))) % 10);
				
				arr[pre_point + 1 + c] = copy_the_copy();
				arr[pre_point + 1 + c].transform.SetParent(main_parent);
				arr[pre_point + 1 + c].name = d.ToString();

				img = arr[pre_point + 1 + c].GetComponentInChildren<UnityEngine.UI.Image>();
				img.sprite = atlasAssets[d];
			}
		}

		c = 0;
		foreach(GameObject child in arr)
		{
			if(child.name == "point")
			{
				child.transform.localPosition = new Vector3(((c - (arr_length/2)) * letter_distance),-10,0);
				child.transform.localScale    = new Vector3(0.5f,0.5f,0.5f);
			}
			else
			{
				child.transform.localPosition = new Vector3(((c - (arr_length/2)) * letter_distance),0,0);
				child.transform.localScale    = new Vector3(1,1,1);
			}
			c++;
		}
		Destroy (the_copy);
	}


	// Use this for initialization
	void Start () {

		updated_this_round = false;

		if(transform.name == "Wins_cube")
			number_to_images (Player_controller.control.wins, true, 0);
		else if(transform.name == "Losses_cube")
			number_to_images (Player_controller.control.losses, true, 0);
		else if(transform.name == "Difficulty_cube")
			number_to_images (Player_controller.control.difficulty, false, 2);
	}

	void remove_images()
	{
		foreach(RectTransform child in transform)
		{
			if(child.name == "Number")
			{
				foreach(RectTransform child2 in child.transform)
				{
					if(child2.name != "copy_this")
					{
						Destroy(child2.gameObject);
					}
				}
				break;
			}
		}
	}

	// Update is called once per frame
	void Update () {
		if (Player_controller.control.won || Player_controller.control.lost) {
			if(!updated_this_round)
			{
				updated_this_round = true;
				remove_images();
				if(transform.name == "Wins_cube")
					number_to_images (Player_controller.control.wins, true, 0);
				else if(transform.name == "Losses_cube")
					number_to_images (Player_controller.control.losses, true, 0);
				else if(transform.name == "Difficulty_cube")
				{
					number_to_images (((Sculpt_land.sculpter.start_evener + Player_controller.control.wins) / 
					 				   (Sculpt_land.sculpter.start_evener + Player_controller.control.losses)),
					                  false, 2);
				}
			}
		}
	}
}
