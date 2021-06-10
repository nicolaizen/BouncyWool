using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* Master_creator is the class resposible for creating and organizing most game objects in the hierarchy.
 */
public class Master_creator : Fill_plain {

	public static 	int 		plain_count,	// Amount of ground "lengths".
								snow_count;		// Amount of snow patches.

	public static 	Vector2[]	surface_layer;	// Contains the vertexes of the ground.
	public 			Vector2[] 	land_sketch,	// Holds direction vecrtors from vertecie to vertecie in surface_layer.
								snow_sketch;	// Snow patch locations.

	public static 	GameObject 	player,			// Player object initiated through prefab.
								Master_grass,	// Parrent of every grass object.
								Master_cloud,	// Parrent of every cloud object.
								Master_snow,	// Parrent of every snow object.
								Master_misc,	// Parrent of every other (mostly flowers) object.
								game_played,	// False on the first round of the session.
								Music_player;

	public static 	bool[] 		snowy_indexes;	// Indexes surface_layer deemed snowy.
	public static 	bool 		rebuild,		// Enables the recreating of the entire scene (every old object is destoyed).
								first_game;		// Active when its the first round of the session.

	public static 	Mesh 		ground_mesh;	// Texture applied to the ground collider.
	public static 	PolygonCollider2D pc2d;		// Ground collider.


	// Use this for initialization
	void OnEnable () {
		player = (GameObject)Instantiate (Resources.Load ("Player")); 

		Player_controller.control.Load();
		create_level ();

		if (!Music_player) {
			Music_player = (GameObject)Instantiate (Resources.Load ("Sound/MusicPlayer"));
			DontDestroyOnLoad (Music_player);
		}
	}

	/* Constructs the entire level, including ground, plants, snow, clouds and the player.
	 */
	void create_level()
	{
		// If the first game of the session.
		if (!game_played) {
			game_played = new GameObject();
			first_game = true;
			DontDestroyOnLoad(game_played);
		}
		else
			first_game = false;

		// Instanciates parrent object of grass.
		Master_grass = new GameObject();
		Master_grass.name = "Master_grass";

		// Instanciates parrent object of cloud.
		Master_cloud = new GameObject ();
		Master_cloud.name = "Master_cloud";
		// Applies movement to the clouds.
		Master_cloud.AddComponent<cloud>();

		// Instanciates parrent object of snow.
		Master_snow = new GameObject ();
		Master_snow.name = "Master_snow";

		// Instanciates parrent object of misc.
		Master_misc = new GameObject ();
		Master_misc.name = "Master_misc";

		// Initiates player.
		Player_controller.control.Init_player ();
		player.GetComponent<Rigidbody2D>().fixedAngle = true;

		// Finds the ground collider.
		pc2d = GetComponent<PolygonCollider2D>();
		// Creates the mesh section that will be stretched over the ground collider.
		ground_mesh = new Mesh ();
		ColliderToMesh.rebuildMesh = true;

		/* Calculates the overall difficulty of the scene.
		 * Finds the amount of plains to be created.
		 */
		plain_count = Plain_counter (Player_controller.control.wins,
		                             Player_controller.control.losses);

		// Makes the vertecies array ready.
		surface_layer = new Vector2[plain_count+1];
		snowy_indexes = new bool[plain_count];
		// Vectors between vertecies of the level floor.
		land_sketch   = Constelate_scene();
		// Stretches the polygon collider into the shape of the ground.
		Construct_polygon_collider (land_sketch, plain_count);

		// Calculates the amount of snow patches, their lenghts and locations.
		var pair 	= plan_snow ();
		snow_count 	= pair.Key;
		snow_sketch = pair.Value;

		// Relocate the family to the end of the stage.
		GameObject fam = GameObject.Find ("Family");
		fam.transform.position = new Vector3(surface_layer [plain_count - 1].x, surface_layer [plain_count - 1].y, 0);

		float new_pos;
		for (int c = 0; c < snow_count; c++)
		{
			// snow_sketch[c].x is actually the start possition of the snow patch.
			new_pos = start_plain_length + snow_sketch[c].x;

			// snow_sketch[c].y is actually snow length in x direction.
			// plains with snow.
			Construct_snow_chunks (new_pos, snow_sketch[c].y);
		}

		add_grass(plain_count);
		generate_clouds ();

		// Deactivates caches of prefabs read from disk.
		deactivate_caches ();

		rebuild = false;
	}

	public void reset_all()
	{
		if (first_game) {
			first_game = false;
			Home_menu.main_menu.reset_all = true;
			Player_controller.control.ground_speed = 0.4f;
		}
		else // Recreates the entire stage.
			Master_creator.rebuild = true;
	}


	// Update is called once per frame
	void Update () {
		if (rebuild) {
			Application.LoadLevel(0);
		}
	}
}
