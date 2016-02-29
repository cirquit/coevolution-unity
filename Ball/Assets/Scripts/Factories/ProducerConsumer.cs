using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class ProducerConsumer : MonoBehaviour {
	
	// how fast can we create a part
	public int 		time;

	// this is the part we are shipping
	public string   output;

	// is there an item available for shipping?
	public bool     available   = false;

	// do I have an item ready?
	private bool    done        = false;

	// how much do we still need to produce
	private int     outputCount = 0;

	// Scripts to access the factory with the parts we need
	private LinkedList<Producer> producerScripts = new LinkedList<Producer>();

	// blueprints to look up the resources needed to create a part - this will be set from SimController
	public Dictionary<string,string[]>  blueprints             = null;
	
	// Keeping track of how much resources are already there
	private Dictionary<string,int>      inventory              = new Dictionary<string, int>();

	// Minimum dependencies to create 1 car
	private Dictionary<string, int>     minimumCarDependencies = new Dictionary<string, int> ();

	// Am I currently collecting something
	private bool moving        = false;

	// factory will only be activated when somebody places an order
	// with 'SetPartOrder'
	private bool factoryActive = false;

	// renderer is used to change the color of the factory
	private Renderer rend      = null;
	
	// custom time management
	private int  busy          = 50 * 2;  // @ TODO substitue 2 with time

	// 
	private bool loadedConfigs = false;

	//
	SimController simulationScript = null;


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
	 * - This will be called from other scripts
	 * - look up the resources that are needed to produce my part
	 * - look up the factorys that produce said resources and request the needed parts
	 */
	public void SetPartOrder (int i)
	{
		outputCount += i;
		string[] resources = blueprints[output];

		foreach (string r in resources){
			GameObject factory = GameObject.FindGameObjectWithTag(r + "Factory");
			Producer   ps      = (Producer) factory.GetComponent("Producer");

			// request parts
			ps.SetPartOrder(i);

			// save the script to check later for availability
			if (!producerScripts.Contains(ps)){
				producerScripts.AddFirst(ps);
			}

		}

		factoryActive = true;
	}

	/**
	 *  - if the the player comes to get an item from me, start producing again
	 *  - if i'm the source of the player, update inventory and destroy player
	 */
	void OnCollisionEnter(Collision col)
	{
		PlayerScript ps = (PlayerScript) col.gameObject.GetComponent<PlayerScript>();

		bool imSourceFactory = gameObject.tag == ps.sourceFactory.tag;
		bool imTargetFactory = gameObject.tag == ps.destinationFactory.tag;

		if (imSourceFactory && ps.loaded){
			string part = ps.destinationFactory.tag.Replace("Factory", "");
			UpdateInventoryWith(part);
			Destroy(col.gameObject);
			moving = false;
		}

		if (imTargetFactory) {
			done = false;
			if(!ps.loaded){
				ps.loaded = true;
			}
		}
    }


	/**
	* - increment the value of our inventory dict with the key 'part' (+ 1)
	*/
	private void UpdateInventoryWith(string part)
	{
		if (inventory.ContainsKey (part)) {
			inventory [part] = inventory [part] + 1;
		} else {
			inventory [part] = 1;
		}
	}


    /**
	 * - creates a player in front of the factory
 	 * - sends him to the targetFactory
 	 */
    private void CreatePlayer(string sourceFactory, string targetFactory)
    {
		moving = true;

        GameObject srcObj = GameObject.FindGameObjectWithTag(sourceFactory);
		GameObject trgObj = GameObject.FindGameObjectWithTag(targetFactory);

		// create player and set position in front of the src factory
        GameObject player = (GameObject) Instantiate(Resources.Load("Player"));
        player.transform.parent   = srcObj.transform;
		Vector3 forward           = new Vector3 (-1, 0, 0);
        player.transform.position = srcObj.transform.position + forward;

		// set source and target for the player
        PlayerScript playerScript       = (PlayerScript) player.GetComponent<PlayerScript>();
		playerScript.sourceFactory      = srcObj;
		playerScript.destinationFactory = trgObj;
    }

	/**
	 * - Do I have anything left to produce
	 * - Is a part already done? -> false
	 * - Do I have all the needed parts to prodcue?
	 */
	private bool PartsAvailable()
	{
		if (outputCount == 0 && !done && factoryActive) factoryActive = false;

		if (!done && factoryActive && (outputCount > 0)) {

			// if we didn't get any item at all, we can't produce

			// if (inventory.Count != minimumCarDependencies.Count) { return false; }

			Dictionary<string,int> tempDeps = new Dictionary<string, int> (minimumCarDependencies);

			// if the amount of any needed part it greater than 0, we don't have enough to produce
			foreach(KeyValuePair<string, int> kvp in inventory){
				if ((tempDeps[kvp.Key] - kvp.Value) > 0) {
					return false;
				}
			}
			return true;
		}
		return false;
	}


	/**
	 * - will be startet only if partsAvailable
	 * - create part every 'time'-interval
	 * - decrement every part in inventory
	 */
	private void CreatePart()
	{
			List<string> keys = new List<string>(inventory.Keys);
		
			foreach(string key in keys)
			{
				inventory[key] = inventory[key] - minimumCarDependencies[key];
			}


			outputCount--;
			busy = time * 50;
			available = true;
        	done      = true;
		//	Debug.Log ("[" + output + "Factory]: To be created - " + outputCount);

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

	// create the dictionary for the minimal dependencies for one part
	private void CreateMinimalDependencies()
	{
		foreach(string r in blueprints[output])
		{
			if(minimumCarDependencies.ContainsKey(r)){
				minimumCarDependencies[r] = minimumCarDependencies[r] + 1;
			} else {
				minimumCarDependencies[r] = 1;
			}

			if(!inventory.ContainsKey(r)){
				inventory [r] = 0;
			}
		}


	}


	/**
	 * - if we don't have a player on the road, check if there are available parts
	 * - if there is a part, create a reservation at said factory and send player
	 */
	void FixedUpdate ()
	{
		if(!loadedConfigs){
			blueprints = new Dictionary<string, string[]> (simulationScript.blueprints);
			CreateMinimalDependencies();
			loadedConfigs = true;
		}

		if (factoryActive && !moving) {
			foreach (Producer p in producerScripts){
				if (p.CreateReservation()){
					CreatePlayer(gameObject.tag, p.output + "Factory");
					break;
				}
			}
		}

		if(PartsAvailable()){
			// wait for 50 * time frames to create a part
			if(busy > 0){
				busy--;
			} else {
				CreatePart();
			}

		}

		ChangeColor();
	}

	void Start ()
	{
		rend = GetComponent<Renderer>();
	}

	// - safe start when all the objects are already initialized
	void Awake()
	{
		simulationScript = (SimController) GameObject.FindGameObjectWithTag ("GeneticAlgorithm").GetComponent ("SimController");
	}

}

