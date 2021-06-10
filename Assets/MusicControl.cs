using UnityEngine;
using System.Collections;
using UnityEngine.Audio;

public class MusicControl : MonoBehaviour {

	public static MusicControl MC;

	public AudioMixerSnapshot musicOff;
	public AudioMixerSnapshot outOfMenu;
	public AudioMixerSnapshot inMenu;
	public float bpm = 130;
	public bool enable_sound;


	private float m_TransitionIn;
	private float m_TransitionOut;
	private float m_QuarterNote;

	void Awake () {
		if (MC == null)
			MC = this;
		else if (MC != this)
			Destroy (gameObject);
	}

	// Use this for initialization
	void Start () {
		m_QuarterNote 	= 60 / bpm;
		m_TransitionIn 	= m_QuarterNote;
		m_TransitionOut = m_QuarterNote;
		enable_sound = false;
	}

	void OnMenuEnter()
	{
		if (Home_menu.main_menu.menu_active && enable_sound) {
			inMenu.TransitionTo(m_TransitionIn);
		}
	}

	void OnMenuExit()
	{
		inMenu.TransitionTo(m_TransitionOut);
	}


	// Update is called once per frame
	void Update () {
		if (Home_menu.main_menu.menu_active && enable_sound)
		{
			inMenu.TransitionTo(m_TransitionIn);
		}
		else if(enable_sound)
		{
			outOfMenu.TransitionTo(m_TransitionIn);
		}
		else
		{
			musicOff.TransitionTo(m_TransitionIn);
		}
	}
}

