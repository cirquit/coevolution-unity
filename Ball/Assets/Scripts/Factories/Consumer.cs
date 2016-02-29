using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


public class Consumer : MonoBehaviour
{

    public int    time;

    public string output;

    public bool   done           = false;

	public  bool  loadedConfigs  = false;


	private bool factoryActive  = false;

	private bool moving         = false;

	private int  busy           = 50 * 2; // @ TODO refactor out to use with time

	private int  outputCount    = 0;

	public  int  frameCounter   = 0;


	public  Dictionary<string, string[]> blueprints             = null;

    private Dictionary<string, int>      inventory              = new Dictionary<string, int>();

	// Minimum dependencies to create 1 car
	private Dictionary<string, int>      minimumCarDependencies = new Dictionary<string, int> ();

    private LinkedList<ProducerConsumer> producerScripts        = new LinkedList<ProducerConsumer>();


	private Renderer rend = null;

	SimController simulationScript = null;


    /**
     * - look up the parts which we need to produce the part
     * - search for all the factorys that create said parts
     * - add them to producerScripts
	 */
	
    public void SetPartOrder(int i)
    {
        outputCount += i;

        string[] resources = blueprints[output];

        foreach (string r in resources)
		{
            GameObject factory = GameObject.FindGameObjectWithTag(r + "Factory");
            ProducerConsumer ps = (ProducerConsumer)factory.GetComponent("ProducerConsumer");

            // request parts from ProducerConsumer factorys
            ps.SetPartOrder(i);
            
            // save the script to check later for availability
            if (!producerScripts.Contains(ps))
            {
                producerScripts.AddFirst(ps);
            }

        }

		factoryActive = true;
	}


	/**
	 * - Do I have anything left to produce
	 * - Is a part already done? -> false
	 * - Do I have all the needed parts to prodcue?
	 */
	private bool PartsAvailable()
	{
		if (outputCount == 0 && !done && factoryActive) {
			done          = true;
			factoryActive = false;
//			Debug.Log ("[" + output + "Factory]: I'm done");
		}

		if (!done && factoryActive && (outputCount > 0)) {

				Dictionary<string,int> tempDeps = new Dictionary<string, int> (minimumCarDependencies);

				// if the amount of any needed part it greater than 0, we don't have enough to produce
				foreach (KeyValuePair<string, int> kvp in inventory) {
				if ((tempDeps [kvp.Key] - kvp.Value) > 0) {
						return false;
					}
				}
				return true;
		}
		return false;
	}

	/**
	 * - will be startet only if PartsAvailable
	 * - create part every 'time'-interval
	 * - decrement every part in inventory
	 */
	private void CreatePart()
	{
		List<string> keys = new List<string>(inventory.Keys);

		foreach(string key in keys){
			inventory[key] = inventory[key] - minimumCarDependencies[key];
		}

		outputCount--;
		busy = 2 * 50;

		if (outputCount == 0) {
			done          = true;
			factoryActive = false;
	//		Debug.Log ("[" + output + "Factory]: I'm done");
		}

	}


	void FixedUpdate()
	{
		if(!loadedConfigs){
			blueprints = new Dictionary<string, string[]>(simulationScript.blueprints);
			CreateMinimalDependencies();
			loadedConfigs = true;
		}

		// greedy request with a semaphore in Ordering
		simulationScript.order.StartRequesting();

		if (factoryActive && !moving){
			foreach (ProducerConsumer pc in producerScripts){

				bool partsNeeded = inventory [pc.output] < minimumCarDependencies [pc.output];

				if (partsNeeded && pc.CreateReservation()){
					//Debug.Log ("[" + output + "Factory]: Sending player. Need" + pc.output + " | have " + inventory[pc.output].ToString ());
					CreatePlayer(gameObject.tag, pc.output + "Factory");
					break;
				}
			}
		}
			
		if (PartsAvailable()) {
			if(busy > 0){
				busy--;
			} else {
				CreatePart();
			}
		}


		if (factoryActive) {
			frameCounter++;
		}

		SetColor ();
	}

	/**
     * - increment the value of our inventory map with the key 'part' (+ 1)
     */
	private void UpdateInventoryWith(string part)
	{
		
		if (inventory.ContainsKey (part)) {
			inventory [part] = inventory [part] + 1;
	//		Debug.Log ("[" + output + "Factory]: Got: " + part + " : " + inventory[part].ToString ());
		}
	}

	// create the dictionary for the minimal dependencies for one part and initializes our inventory to zero at all fields
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


	void OnCollisionEnter(Collision col)
	{
		PlayerScript ps = (PlayerScript)col.gameObject.GetComponent<PlayerScript>();

		bool imSourceFactory = gameObject.tag == ps.sourceFactory.tag;

		if (imSourceFactory && ps.loaded)
		{
			string part = ps.destinationFactory.tag.Replace("Factory", "");
			UpdateInventoryWith(part);
			Destroy(col.gameObject);
			moving = false;

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
		GameObject player = (GameObject)Instantiate(Resources.Load("Player"));
		// Vector3 forward = new Vector3(-1, 0, 0);
		player.transform.parent = srcObj.transform;
		player.transform.position = srcObj.transform.position; //+ forward;

		// set source and target for the player
		PlayerScript playerScript = (PlayerScript)player.GetComponent<PlayerScript>();
		playerScript.sourceFactory = srcObj;
		playerScript.destinationFactory = trgObj;

	}


	void Awake()
	{
        simulationScript = (SimController) GameObject.FindGameObjectWithTag("GeneticAlgorithm").GetComponent("SimController");
		rend = GetComponent<Renderer>();
    }


	// @ TODO change the color based on outputcount
    private void SetColor()
	{
        /*
        if (factoryActive){
            if (done){
                rend.material.color = new Color32(195, 252, 48, 255); // green
            }
            else {
                rend.material.color = new Color32(252, 126, 48, 255); // red
            }
        } else {
                rend.material.color = new Color32(141, 188, 205, 255); // blue
		}
        */
	}
    /*
    void OnGUI()
    {
        string stringToEdit = output;
        float middleX = 300;
        float middleY = 80;
        float posX = middleX + (Math.Abs(gameObject.transform.position.x)*18);
        float posY = middleY - (Math.Abs(gameObject.transform.position.z)*10);
        float posZ = gameObject.transform.position.z;
        //stringToEdit = stringToEdit + ": x: " + posX + ", y: " + posY;
        stringToEdit = stringToEdit + ": localPosx: " + posX + ", localPosy: " + posY;

        GUI.TextField(new Rect(posX, posY, 220, 20), stringToEdit, 25);
    }
    */
}