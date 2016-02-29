using UnityEngine;
using System.Collections;

public class Producer : MonoBehaviour {

    public string    output;
    public int       time;
    public bool      available        = false;

	private bool     done             = false;
	private bool     factoryActive    = false;
    private int      outputCount      = 0;
    private Renderer rend;
	private int      busy             = 50 * 2; // @ TODO substitute 2 with time


	/**
	 * - Do I have anything left to produce
	 * - Is a part already done? -> false
	 */
	private bool partsAvailable(){

		if (outputCount == 0 && !done) factoryActive = false;

		return (!done && factoryActive && (outputCount > 0));	
	}

	/**
	 * - will be startet only if partsAvailable
	 * - create part every 'time'-interval
	 */
	private void CreatePart()
	{
		outputCount--;
		busy      = time * 49;
	    available = true;
		done      = true;
	}
	
    // Use this for initialization
    void Start()
	{
		// Renderer
		rend = GetComponent<Renderer>();
    }

	void FixedUpdate()
	{
		if(partsAvailable()){
			if(busy > 0){
				busy--;
			} else {
				CreatePart();
			}
		}

		ChangeColor();
	}


	/**
	 * - changes color of the factory based on
	 *   * factoryActive
	 *   * done
	 */
	private void ChangeColor()
	{
		if (factoryActive){
			if(done){
				rend.material.color = new Color32(195, 252, 48, 255); // green
			} else {
				rend.material.color = new Color32(252, 126, 48, 255); // red
			}
		} else {
			rend.material.color = new Color32(141, 188, 205, 255); // blue
		}
	}
	
			// Bestellung aufgeben
    public void SetPartOrder(int i)
    {
        outputCount  += i;
		factoryActive = true;
    }

	/**
	 *  - This will be called from other scripts
	 *  - tries to make a reservation for an item, so nobody else can access it
	 *  - returns if the reservation was successful
	 */
	public bool CreateReservation()
	{
		if (available){
			available = false;
			return true;
		} else {
			return false;
		}
	}


	/**
	 *  - if the the player comes to get an item from me, start producing again
	 *  - if i'm the source of the player, update inventory and destroy player
	 */
	void OnCollisionEnter(Collision col)
	{
		PlayerScript ps = (PlayerScript) col.gameObject.GetComponent<PlayerScript>();

		bool imTargetFactory = gameObject.tag == ps.destinationFactory.tag;

		if (imTargetFactory){
			done = false;
			if(!ps.loaded){
				ps.loaded = true;
			}

		}

	}
}
