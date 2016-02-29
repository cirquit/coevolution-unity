using UnityEngine;
using System.Collections;

public class PlayerScript : MonoBehaviour {

	public GameObject destinationFactory;
	public GameObject sourceFactory;

	public float     speed  = 1;
	public bool      loaded = false;

	private float    step;
	private Renderer rend   = null;

	/**
    *  - When a collision was triggered
    *  - check if collision object is source (origin-factory) or destination
    */
	void OnCollisionEnter(Collision col)
	{
		// is the factory my destination factory
		if (col.gameObject.tag == destinationFactory.tag)
		{
			loaded = true;
			rend.material.color = new Color32(195, 252, 48, 255); // green
                                                                  
            // Debug.Log("Player ist an Destination angekommen.");
        }
	}

	// initialise source and destination
	void Start () {
		//gameObject.tag = destinationFactory.tag + "Player";
		rend = GetComponent<Renderer>();
	}

	/**
	 * move forward per step 
	 */
	void MoveForward(){
		step = speed * Time.deltaTime;
		transform.position += transform.forward * step;
	}
	
	/**
	 * null checking LookAt
	 */
	void SafeLookAt(GameObject o){
		if(o != null){
			transform.LookAt(o.transform);
		}
	}

	// FixedUpdate is called once per frame
	void FixedUpdate () {

		// if (oldPosition )

		if (!loaded) {
			SafeLookAt (destinationFactory);
		} else {
			SafeLookAt (sourceFactory);
		}
		MoveForward();
	}
}
