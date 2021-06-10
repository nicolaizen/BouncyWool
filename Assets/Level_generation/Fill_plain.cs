using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/* Fill_plain creates the ground between two points in the level.
 */
public class Fill_plain : Sculpt_land {

	// Contain length and with of ground_blocks.
	public Vector2 tile_size = new Vector2(0,0);

	// Array used to store every single prefab of commonly used types.
	// The prefabs are read from disk to memory once in this fashion.
	// This is done to reduce storage lookoups and instead instatiate objects as clones from memory.
	// Structured in arrays to make them accessable by index thus easier to randize.
	private GameObject[] 	grass_cache,		// Stores every grass  prefrab.
							flower_cache,		// Stores every flower prefrab.
							cloud_cache,		// Stores every cloud  prefrab.
							snow_cache_xs,		// Stores every extra-small snow prefrab.
							snow_cache_s,		// Stores every small 		snow prefrab.
							snow_cache_m,		// Stores every medium 		snow prefrab.
							snow_cache_l,		// Stores every large 		snow prefrab.
							snow_cache_xl;		// Stores every extra-large snow prefrab.

	private GameObject  	grass_cache_parent,	// Parrent object off all the instances of the grass_cache.
							flower_cache_parent,// Parrent object off all the instances of the flower_cache.
							cloud_cache_parent,	// Parrent object off all the instances of the cloud_cache.
							snow_cache_parent,	// Parrent object off all the instances of the snow_cache.
							Snow_cache,			// Parrent object off all the instances of the snow_cache.
							new_grass,			// Used to construct new grass objects.
							new_cloud,			// Used to construct new cloud objects.
							new_snow,			// Used to construct new snow  objects.
							new_misc;			// Used to construct new misc  objects.

	private float  	inc_x, inc_y, 		// Position increment between blocks.
					plain_length, 		// Length plain to fill.
					plain_angle_rad, 	// Plain angle in radians relative to V2(1,0).
					plain_angle_degree, // Plain angle in degree  relative to V2(1,0).

					prev_end_x,			// Location of the previous block.
					prev_end_y,			// -------------||---------------.
					prev_angle,			// Previous plains anglee in degrees.
					level_x_length,		// Length of level in X direction.
					level_y_length,		// Length of level in Y direction.
					cloud_top_height; 	// Couds maxlength from the ground.

	public  int		mesh_vertices;		// Amount of vertices in the mesh.
	private int    	tile_count,			// Amount of tiles to place.
					prev_snow_stop,		// Index the last snow patch ended on.
					cloud_stack,		// Current count of clouds. Used to set sorting order.
					flower_stack;		// Current count of flowers.Used to set sorting order.

	private bool   	grass_inited,		// Set when the grass has been created.
					snow_inited;		// Set when the snow  has been created.

	void start()
	{
		cloud_top_height 		= 100f;
		cloud_stack 	 		= 0;
		flower_stack 	 		= 0;
	}

	// Creates a copies of the active prefab,
	public GameObject duplicate_frefab (GameObject dupe){
		return (GameObject)Instantiate(dupe,
		                        	   new Vector3(0,0,0),
		                        	   Quaternion.identity);
	}

	// Moves the block to the current work position.
	public void adjust_block(GameObject GO, Vector3 rot){
		GO.transform.position = new Vector3 (prev_end_x, prev_end_y, 0);
		GO.transform.Rotate (rot);
	}

	// Deactivates caches of prefabs read from disk.
	public void deactivate_caches()
	{
		grass_cache_parent.SetActive (false);
		flower_cache_parent.SetActive (false);
		cloud_cache_parent.SetActive (false);
		Snow_cache.SetActive (false);
	}

	/* Stretches the polygon collider into the shape of the ground.
	 * Input:	V2_vertices is the vectors between vertecies of the level floor,
	 * 			v_count 	is the number vertecies in the top line.
	 */
	public void Construct_polygon_collider (Vector2[] V2_vertices, int v_count) {

		prev_end_x = 0;
		prev_end_y = 0;

		float 	lowest_surface 	= 0f,
				thickness 		= -60f,
				lowest_bottom  	= thickness;

		grass_inited = false;

		float bx, by;

		int mesh_vertices 	= (2 * v_count) + 2;
		Vector2[] vertecies = new Vector2[mesh_vertices];

		vertecies [0] = new Vector2 (0, 0);
		Master_creator.snowy_indexes [0] = false;
		Master_creator.surface_layer [0] = vertecies [0];

		// Moves vertexes into possition.
		for (int c = 0; c < mesh_vertices-2; c++) {

			// Find location of top line of vetecies.
			if(c < (v_count))
			{
				Master_creator.snowy_indexes[c] = false;
				bx = V2_vertices[c].x + prev_end_x;
				by = V2_vertices[c].y + prev_end_y;

				if(level_y_length < by)
					level_y_length = by;

				vertecies[c+1] = new Vector2(bx, by);
				Master_creator.surface_layer [c+1] = vertecies [c+1];

				prev_end_x += V2_vertices[c].x;
				prev_end_y += V2_vertices[c].y;

				if (prev_end_y < lowest_surface)
				{ 
					lowest_surface = prev_end_y;
					lowest_bottom  = lowest_surface + thickness;
				}
			}else{
				// Find location of bottom line of vetecies.
				vertecies[c+1].x = vertecies[mesh_vertices - c - 2].x;
				vertecies[c+1].y = lowest_bottom;
			}
			vertecies [mesh_vertices-1] = new Vector2 (0, lowest_bottom);
		}

		level_x_length = prev_end_x;

		// Applies array of vertecies to the ground collider.
		Master_creator.pc2d.SetPath (0, vertecies);
	}

	/* Initiates a single instance of each cloud object.
	 * They are all placed under the parrent object named "Cloud_cache".
	 */
	void init_clouds()
	{
		cloud_cache_parent = new GameObject ();
		cloud_cache_parent.name = "Cloud_cache";

		cloud_cache = new GameObject[8];

		cloud_cache[0] = (GameObject)Instantiate (Resources.Load ("clouds/cloud_1"));
		cloud_cache[0].transform.SetParent (cloud_cache_parent.transform);

		cloud_cache[1] = (GameObject)Instantiate (Resources.Load ("clouds/cloud_2"));
		cloud_cache[1].transform.SetParent (cloud_cache_parent.transform);

		cloud_cache[2] = (GameObject)Instantiate (Resources.Load ("clouds/cloud_3"));
		cloud_cache[2].transform.SetParent (cloud_cache_parent.transform);

		cloud_cache[3] = (GameObject)Instantiate (Resources.Load ("clouds/cloud_4"));
		cloud_cache[3].transform.SetParent (cloud_cache_parent.transform);

		cloud_cache[4] = (GameObject)Instantiate (Resources.Load ("clouds/cloud_5"));
		cloud_cache[4].transform.SetParent (cloud_cache_parent.transform);

		cloud_cache[5] = (GameObject)Instantiate (Resources.Load ("clouds/cloud_6"));
		cloud_cache[5].transform.SetParent (cloud_cache_parent.transform);

		cloud_cache[6] = (GameObject)Instantiate (Resources.Load ("clouds/cloud_7"));
		cloud_cache[6].transform.SetParent (cloud_cache_parent.transform);

		cloud_cache[7] = (GameObject)Instantiate (Resources.Load ("clouds/cloud_8"));
		cloud_cache[7].transform.SetParent (cloud_cache_parent.transform);

	}

	/* Genrates and moves a set of new clouds into game scene.
	 */
	public void generate_clouds()
	{
		// Creates an array containing an instance of each cloud object.
		init_clouds ();

		int current_cloud_num,
			index,
			flower_source_id;

		Vector2 new_cloud_pos;
		SpriteRenderer sprite;

		RaycastHit2D RH;

		for (current_cloud_num = 0; current_cloud_num < cloud_count; current_cloud_num++)
		{
			// Instantiates a random cloud out of the cloud cache.
			new_cloud	= duplicate_frefab (cloud_cache[Random.Range (0, 8)]);

			// Finds new x-position.
			new_cloud_pos.x	= Random.Range (0, level_x_length);

			// Locates the first spec ground on the new x-position.
			RH = Physics2D.Raycast(new Vector2(new_cloud_pos.x, cloud_top_height + level_y_length),
			                       new Vector2(0,-1));

			index = Player_controller.binary_search(Master_creator.surface_layer,
													RH.point.x,
													0, 0, 
													(Master_creator.plain_count));

			if(!Master_creator.snowy_indexes[index] && RH.transform.name == "Level_constructor")
			{
				// Plants a flower on the ground where the new cloud is located,
				// this is only done on locations where there are no snow.

				flower_source_id = Random.Range((int)0,(int)5);
				new_misc = duplicate_frefab(flower_cache[flower_source_id]);

				if(flower_source_id == 0)
				{
					// Object is a butterfly.
					new_misc.transform.position = new Vector3(new_cloud_pos.x, (RH.point.y + 8f), 0);
				}
				else
					new_misc.transform.position = new Vector3(new_cloud_pos.x, RH.point.y, 0);

				new_misc.transform.Rotate (new Vector3(0, (180f * (int)Random.Range((int)0,(int)1)),0));

				// Sets sorting order of the new flower to be the one on top.
				sprite = new_misc.GetComponentInChildren<SpriteRenderer>();
				sprite.sortingOrder = flower_stack;
				flower_stack++;

				new_misc.transform.SetParent(Master_creator.Master_misc.transform);
			}

			new_cloud_pos.y = Random.Range(RH.point.y, (RH.point.y + cloud_top_height));
			new_cloud.transform.position = new_cloud_pos;

			// Sets sorting order of the new cloud to be the one on top.
			sprite = new_cloud.GetComponentInChildren<SpriteRenderer>();
			sprite.sortingOrder = cloud_stack;
			cloud_stack++;

			new_cloud.transform.SetParent(Master_creator.Master_cloud.transform);
		}
	}

	/* Initiates a single instance of each grass and flower object.
	 * Every grass object are placed under the parrent object named "Grass_cache".
	 * Every flower object are placed under the parrent object named "flower_cache".
	 */
	void init_grass()
	{
		grass_cache_parent = new GameObject ();
		grass_cache_parent.name = "Grass_cache";

		grass_cache = new GameObject[3];

		grass_cache [0] = (GameObject)Instantiate (Resources.Load ("plants/grass_1"));
		grass_cache [0].transform.SetParent (grass_cache_parent.transform);

		grass_cache [1] = (GameObject)Instantiate (Resources.Load ("plants/grass_2"));
		grass_cache [1].transform.SetParent (grass_cache_parent.transform);

		grass_cache [2] = (GameObject)Instantiate (Resources.Load ("plants/grass_3"));
		grass_cache [2].transform.SetParent (grass_cache_parent.transform);


		flower_cache_parent = new GameObject ();
		flower_cache_parent.name = "flower_cache";

		flower_cache = new GameObject[7];

		flower_cache [0] = (GameObject)Instantiate (Resources.Load ("plants/butterfly"));
		flower_cache [0].transform.SetParent (flower_cache_parent.transform);

		flower_cache [1] = (GameObject)Instantiate (Resources.Load ("plants/flower_1"));
		flower_cache [1].transform.SetParent (flower_cache_parent.transform);

		flower_cache [2] = (GameObject)Instantiate (Resources.Load ("plants/flower_2"));
		flower_cache [2].transform.SetParent (flower_cache_parent.transform);

		flower_cache [3] = (GameObject)Instantiate (Resources.Load ("plants/flower_3"));
		flower_cache [3].transform.SetParent (flower_cache_parent.transform);

		flower_cache [4] = (GameObject)Instantiate (Resources.Load ("plants/flower_4"));
		flower_cache [4].transform.SetParent (flower_cache_parent.transform);

		flower_cache [5] = (GameObject)Instantiate (Resources.Load ("plants/leaf"));
		flower_cache [5].transform.SetParent (flower_cache_parent.transform);

		flower_cache [6] = (GameObject)Instantiate (Resources.Load ("plants/tree"));
		flower_cache [6].transform.SetParent (flower_cache_parent.transform);

		grass_inited = true;
	}

	/* Fills a plain with grass objects.
	 * Instantiates different between sets of grass objects based on the length of the plain.
	 * This is done because the sprites are stretched, 
	 * and to avoid contorting the sprites different prites are chosen under various circomstances.
	 * Since the plain is just a line, can it be need for multiple objects to cover one stretch.
	 * Input: 	first 	-> is the start potint of the plain,
	 * 			second 	-> is the end point of the plain.
	 */
	void grass_to_length(Vector2 first, Vector2 second)
	{

		float rest_length 		= Vector2.Distance(first, second);
		Vector3 current_scale 	= Vector3.one;
		Vector3 work_location;

		work_location = new Vector3 (first.x, first.y, 0);

		// Finds the angle of the plain.
		plain_angle_degree = Vector2.Angle (new Vector2(1,0), (second - first));
		if(first.y > second.y)
			plain_angle_degree *= -1;
		plain_angle_rad    = Mathf.Deg2Rad * plain_angle_degree;

		while (rest_length >= 0) {

			if (rest_length < 10) {
				// Finds scale
				current_scale = new Vector3(rest_length / 10, 1, 1);
				// instantiates random grass object from cache.
				new_grass = duplicate_frefab (grass_cache[0]);
				break;
			} else if (rest_length < 20) {
				// Finds scale
				current_scale = new Vector3(1 + ((rest_length - 10) / 10), 1, 1);
				// instantiates random grass object from cache.
				new_grass = duplicate_frefab (grass_cache[2]);
				break;
			}else{

				current_scale = Vector3.one;
				new_grass = duplicate_frefab (grass_cache[1]);

				new_grass.transform.position = work_location;

				new_grass.transform.Rotate (new Vector3(0, 0, plain_angle_degree));

				work_location.x += Mathf.Cos(plain_angle_rad) * 20;
				work_location.y += Mathf.Sin(plain_angle_rad) * 20;

				new_grass.transform.SetParent(Master_creator.Master_grass.transform);

				rest_length -= 20;
			}
		}

		// Updates the new grass objects possition, scale and rotation.
		new_grass.transform.position   = work_location;
		new_grass.transform.localScale = current_scale;

		new_grass.transform.Rotate (new Vector3(0, 0, plain_angle_degree));
		// Placed under the master grass parrent.
		new_grass.transform.SetParent(Master_creator.Master_grass.transform);
	}

	/* Places grass unto all non-snowcovered plains on top of the ground.
	 */
	public void add_grass(int index)
	{
		// Makes sure the grass cache is initiates.
		if(!grass_inited)
			init_grass ();

		// Places grass on every top plain.
		// This places grass from where the snow stoped forward until the snow start.
		for(int c = prev_snow_stop; c < index; c++)
		{
			grass_to_length(Master_creator.surface_layer[c],
			                Master_creator.surface_layer[c+1]);
		}
	}

	/* Initiates a single instance of each snow object.
	 * Every grass object are placed under the parrent object named "Snow_cache".
	 * The snowcache is seperated into 5 subcahces, sotrted by thier size.
	 */
	void init_snow()
	{
		snow_cache_parent 	= new GameObject();
		snow_cache_xs 		= new GameObject[2];
		snow_cache_s 		= new GameObject[5];
		snow_cache_m 		= new GameObject[4];
		snow_cache_l 		= new GameObject[2];
		snow_cache_xl 		= new GameObject[1];

		snow_cache_parent.name	= "Snow_cache";

		snow_cache_xs[0] = (GameObject)(Instantiate (Resources.Load ("snow/snow_9")));
		snow_cache_xs[1] = (GameObject)(Instantiate (Resources.Load ("snow/snow_13")));
		snow_cache_s [0] = (GameObject)(Instantiate (Resources.Load ("snow/snow_4")));
		snow_cache_s [1] = (GameObject)(Instantiate (Resources.Load ("snow/snow_7")));
		snow_cache_s [2] = (GameObject)(Instantiate (Resources.Load ("snow/snow_11")));
		snow_cache_s [3] = (GameObject)(Instantiate (Resources.Load ("snow/snow_12")));
		snow_cache_s [4] = (GameObject)(Instantiate (Resources.Load ("snow/snow_14")));
		snow_cache_m [0] = (GameObject)(Instantiate (Resources.Load ("snow/snow_2")));
		snow_cache_m [1] = (GameObject)(Instantiate (Resources.Load ("snow/snow_3")));
		snow_cache_m [2] = (GameObject)(Instantiate (Resources.Load ("snow/snow_6")));
		snow_cache_m [3] = (GameObject)(Instantiate (Resources.Load ("snow/snow_10")));
		snow_cache_l [0] = (GameObject)(Instantiate (Resources.Load ("snow/snow_1"))); 
		snow_cache_l [1] = (GameObject)(Instantiate (Resources.Load ("snow/snow_5"))); 
		snow_cache_xl[0] = (GameObject)(Instantiate (Resources.Load ("snow/snow_8")));

		snow_cache_xs[0].transform.SetParent (snow_cache_parent.transform);
		snow_cache_xs[1].transform.SetParent (snow_cache_parent.transform);
		snow_cache_s [0].transform.SetParent (snow_cache_parent.transform);
		snow_cache_s [1].transform.SetParent (snow_cache_parent.transform);
		snow_cache_s [2].transform.SetParent (snow_cache_parent.transform);
		snow_cache_s [3].transform.SetParent (snow_cache_parent.transform);
		snow_cache_s [4].transform.SetParent (snow_cache_parent.transform);
		snow_cache_m [0].transform.SetParent (snow_cache_parent.transform);
		snow_cache_m [1].transform.SetParent (snow_cache_parent.transform);
		snow_cache_m [2].transform.SetParent (snow_cache_parent.transform);
		snow_cache_m [3].transform.SetParent (snow_cache_parent.transform);
		snow_cache_l [0].transform.SetParent (snow_cache_parent.transform);
		snow_cache_l [1].transform.SetParent (snow_cache_parent.transform);
		snow_cache_xl[0].transform.SetParent (snow_cache_parent.transform);
	}

	/* Fills a plain with snow objects.
	 * Instantiates different between sets of snow objects based on the length of the plain.
	 * This is done because the sprites are stretched, 
	 * and to avoid contorting the sprites different prites are chosen under various circomstances.
	 * * Since the plain is just a line, can it be need for multiple objects to cover one stretch.
	 * Input: 	first 	-> is the start potint of the plain,
	 * 			second 	-> is the end point of the plain.
	 */
	void snow_to_length(Vector2 first, Vector2 second)
	{
		float rest_length = Vector2.Distance(first, second);
		Vector3 current_scale = Vector3.one;
		Vector3 work_location;

		work_location = new Vector3 (first.x, first.y, 0);
		
		// Finds the angle of the plain.
		plain_angle_degree = Vector2.Angle (new Vector2(1,0), (second - first));
		if(first.y > second.y)
			plain_angle_degree *= -1;

		plain_angle_rad    = Mathf.Deg2Rad * plain_angle_degree;

		while (rest_length >= 0) {
			
			if (rest_length < 5) {

				current_scale = new Vector3(rest_length / 5, 1, 1);
				new_snow = (GameObject)(Instantiate (GameObject.FindGameObjectsWithTag("xs")[Random.Range((int)0,(int)1)]));

				break;
			} else if (rest_length < 10) {
				
				current_scale = new Vector3((rest_length / 10), 1, 1);
				new_snow = (GameObject)(Instantiate (GameObject.FindGameObjectsWithTag("s")[Random.Range((int)0,(int)4)]));

				break;
			}
			else if(rest_length < 15) {
				
				current_scale = new Vector3((rest_length / 15), 1, 1);
				new_snow = (GameObject)(Instantiate (GameObject.FindGameObjectsWithTag("m")[Random.Range((int)0,(int)3)]));

				break;
			}else if(rest_length < 25) {
				
				current_scale = new Vector3((rest_length / 20), 1, 1);
				new_snow = (GameObject)(Instantiate (GameObject.FindGameObjectsWithTag("l")[Random.Range((int)0,(int)1)]));

				break;
			}
			else{
				
				new_snow = (GameObject)(Instantiate (GameObject.FindGameObjectsWithTag("xl")[Random.Range((int)0,(int)0)]));

				current_scale = Vector3.one;

				new_snow.transform.position = work_location;
				new_snow.transform.Rotate (new Vector3(0, 0, plain_angle_degree));
				
				rest_length 	-= 25;
				
				work_location.x += Mathf.Cos(plain_angle_rad) * 25;
				work_location.y += Mathf.Sin(plain_angle_rad) * 25;
				
				new_snow.transform.SetParent(Master_creator.Master_snow.transform);
			}
		}
		new_snow.transform.localScale = current_scale;
		new_snow.transform.position   = work_location;

		new_snow.transform.Rotate (new Vector3(0, 0, plain_angle_degree));
		new_snow.transform.SetParent(Master_creator.Master_snow.transform);
	}

	/* Covers plains with snow.
	 * Input: 	pos 	-> startlocation of snow chunk,
	 * 			length	-> length of snow chunk in world space.
	 */
	public void Construct_snow_chunks (float pos, float length)
	{
		int start_index, 
			end_index,
			current_index;
		float snow_length = 0;
		Vector2 startPos, endPos;
		
		if (!GameObject.Find ("Snow_cache")) {
			Snow_cache = (GameObject)(Instantiate (Resources.Load ("snow/Snow_cache")));
			Snow_cache.name = "Snow_cache";
		}
		
		start_index = Player_controller.binary_search(Master_creator.surface_layer,
				                                      pos,
					              					  0, 0, 
					              					  (Master_creator.plain_count));

		end_index = Player_controller.binary_search(Master_creator.surface_layer,
				                                    pos + length,
				                                    0, 0, 
				                                    (Master_creator.plain_count));

		add_grass(start_index);
		prev_snow_stop = end_index + 1;
		
		for (current_index = start_index; current_index <= end_index; current_index++) {
			Master_creator.snowy_indexes[current_index] = true;
			startPos 	= Master_creator.surface_layer [current_index];
			endPos 		= Master_creator.surface_layer [current_index +1];
			snow_to_length (startPos,endPos);
			snow_length += (endPos-startPos).magnitude;
		}
		if (snow_length >= seagull_spawn_length) {
			GameObject Bird = (GameObject)(Instantiate (Resources.Load ("Bird")));
			Vector3 BirdPos = (Vector3)(Master_creator.surface_layer[start_index]);
			Bird.transform.position = BirdPos;
		}

		//Debug.Log ("seagull_spawn_length: " + seagull_spawn_length);
		//Debug.Log ("snow_length: " + snow_length);
	}
}
