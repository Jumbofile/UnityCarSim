using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSound : MonoBehaviour {
	public AudioSource idle;
	public AudioSource rev;
	public AudioSource dec;
	public AudioSource max;
	public bool revd = false;
	public bool decd = false;
	public float oldtime = 0;
	// Use this for initialization
	void Start () {
		idle.Play ();
		rev.Stop ();
		dec.Stop ();
		max.Stop ();
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetKey (KeyCode.W) && revd == false) {
			revd = true;
			idle.Stop ();
			dec.Stop ();
			rev.Play ();
			oldtime = rev.time;
		
		} else if (revd == true && Input.GetKeyUp (KeyCode.W )) {
			oldtime = rev.time;
			rev.Stop ();
			idle.Stop ();
			max.Stop ();
			dec.Play ();


			//revd = false;
		}else if(Input.GetKey (KeyCode.W) && revd == true){
			revd = false;
			rev.Stop ();
			float dectime = dec.time;
			float newtime = oldtime - dectime;
			dec.Stop ();
			rev.PlayScheduled (newtime);

		}
	}
}
