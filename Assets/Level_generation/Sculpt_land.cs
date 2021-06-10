using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/* Creates the level-layout "core".
 * Is set up by creating a list of vectors.
 * The vectors are the skeletal-versions of the ground.
 */ 
public class Sculpt_land : MonoBehaviour {
	
	public static Sculpt_land sculpter;				// Makes a class object accessable thoughout the scene.

	public 	Vector2[]	ground_outline,				// Vectors between vertecies of the level floor.
						patch_locations;			// Snow patch locations.

	public  float		win_power, loss_power,		// Determined by the player success rate.
						total_games_played,			// Amount of games played by the player.
						start_plain_length,	 		// Horizontal plain length BEFORE climb.
						end_plain_length, 			// Horizontal plain length AFTER  climb.
						start_evener, 				// Evens out level difficulty the first few rounds.
						seagull_spawn_length;		// Required length of snow for a seagull to spawn.
	
	private float		native_x_inc, native_y_inc, // Lower bound of plain length/width increment potential.
						intensity_x, intensity_y, 	// Current plain length/width increment potential.
						increment_x, increment_y, 	// Length and width of the current plain under construction.
						x_inc_minimum,				// Shortest increment (cathetus-> x)length of plain.
						level_length,				// Length of level in x direction.
						snow_length_cap,			// Maximum length of snowpatch in x direction.
						snow_min_gap,				// Minimum length of snowpatch in x direction.
						snow_patch_count;			// Amount of snowpatches.
	
	public int   		total_plain_count,			// Amount of plains(Stretches of ground).
						cloud_count;				// Amount of clouds.


	// Use this for initialization
	void Awake () {
		if (sculpter == null) {
			//DontDestroyOnLoad (gameObject);
			sculpter = this;
		} else if (sculpter != this) {
			Destroy (gameObject);
		}
	}

	/* Calculates the overall difficulty of the scene.
	 * Returns the amount of plains to be created.
	 */
	public int Plain_counter(int wins, int losses) {

		seagull_spawn_length= 90f;
		start_plain_length  = 100f;
		end_plain_length	= 100f;
		native_x_inc		= 5f;
		native_y_inc		= 5f;
		start_evener		= 40f;
		level_length 		= 0f;
		snow_length_cap 	= 40f;
		snow_min_gap		= 40f;
		x_inc_minimum 		= 3f;

		// Finds the relevance of the start_evener.
		total_games_played 	= (float)(wins + losses);
		start_evener 		= (total_games_played < start_evener) ?
							  (start_evener - total_games_played) : 0f;

		// Applies the start_evener.
		win_power			= start_evener + (float)wins;
		loss_power 			= start_evener + (float)losses;

		Player_controller.control.difficulty = win_power / loss_power;

		// native + 2e^x
		intensity_x = (int)(2*(Mathf.Exp (win_power / loss_power)) + native_x_inc);
		intensity_y = (int)(2*(Mathf.Exp (win_power / loss_power)) + native_y_inc);

		// 10(e^x+1)
		total_plain_count = (int)((Mathf.Exp (win_power / loss_power) * 10) + 10);
		cloud_count       = total_plain_count;

		return total_plain_count;
	}

	/* Calculates the amount of snow patches, their lenghts and locations.
	 * Retruns the amount of usable snowpatches and their 
	 */
	public KeyValuePair<int, Vector2[]> plan_snow () {

		// Finds the amout of snow patches.
		float win_rate 	= win_power / loss_power;
		float calc	 	= (level_length * win_rate / 100) + 2;

		int patch_count = (int)Mathf.Ceil (calc);
		patch_locations = new Vector2[patch_count];

		// min, max X-lengths.
		float min = 0f;
		float max = (level_length / patch_count) - snow_length_cap;

		float start_pos;

		float prev_end 		= 0f;
		float snow_length 	= 0f;

		Vector2 snow_chunk;

		int c;
		for (c = 0; c < patch_count; c++)
		{
			// Finds the position the snow patch can start at.
			start_pos = ((c / patch_count) * level_length) + Random.Range(min, max);
			if(start_pos < prev_end + snow_min_gap)
				start_pos = prev_end + snow_min_gap;

			// Finds the length of the snow patch.
			snow_length = Random.Range(intensity_x, snow_length_cap);

			// Stops the snow from reaching the faimly.
			if(start_pos + snow_length > level_length)
				break;

			snow_chunk.x = start_pos;
			snow_chunk.y = snow_length;

			patch_locations[c] = snow_chunk;

			prev_end = start_pos + snow_length;
		}

		if (c < 0)
			c = 0;
		return new KeyValuePair<int, Vector2[]>(c, patch_locations);
	}

	// Creates vectors between vertecies of the level floor.
	public Vector2[] Constelate_scene () {
	
		float player_radius = Master_creator.player.GetComponent<CircleCollider2D> ().radius;
			
		ground_outline 	    = new Vector2[total_plain_count];
		ground_outline [0]  = new Vector2(start_plain_length, 0);

		for (int c = 1; c < (total_plain_count -1); c++) {
			// Finds increments in x and y direction.
			// Y increment can only be negative every other plain.
			increment_x = Random.Range((player_radius + x_inc_minimum), intensity_x);
			increment_y = (c % 2 == 1) ? Random.Range(0f,intensity_y * (win_power  / loss_power)) :
										 Random.Range(-intensity_y   * (loss_power / win_power),intensity_y);

			level_length 	 += increment_x;
			ground_outline[c] = new Vector2 (increment_x, increment_y);
		}
		ground_outline [total_plain_count -1] = new Vector2 (end_plain_length, 0);

		return ground_outline;
	}
}
