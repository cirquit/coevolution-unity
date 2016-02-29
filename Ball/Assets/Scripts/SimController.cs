using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SimController : MonoBehaviour
{
    public bool  resultsAvailable = false;

	// Fitness
	public float avgTime          = 0.0f;
    public float avgMiniTime      = 0.0f;
    public float avgTruckTime     = 0.0f;
    public float avgFerrariTime   = 0.0f;
    public int   carCount = 0;

    public bool  valid            = true;

    public Dictionary<string, string[]> blueprints = new Dictionary<string, string[]>();
   
	public Ordering order = null;

	// all consumer factories
	private Consumer ferrariScript = null;
	private Consumer miniScript    = null;
	private Consumer truckScript   = null;


	public void StartSimulation(Ordering order)
    {
		Application.LoadLevel("MiniGame1");
		this.order = order;
	}

    private void FillPlan()
	{
		string[] depTruck    = { "Window",            "Chassis",            "Interior", "Interior" };   // plan to build a truck
		string[] depFerrari  = { "Window", "Window",  "Chassis",            "Interior" };               // plan to build a ferrari
		string[] depMini     = { "Window",            "Chassis", "Chassis", "Interior" };               // plan to build a mini
		string[] depWindow   = { "Sand"   };
		string[] depChassis  = { "Ore"    };
		string[] depInterior = { "Rubber" };

		blueprints.Add (   "Truck", depTruck   );
		blueprints.Add ( "Ferrari", depFerrari ); 
		blueprints.Add (    "Mini", depMini    ); 
        blueprints.Add (  "Window", depWindow  );
        blueprints.Add ( "Chassis", depChassis );
        blueprints.Add ("Interior", depInterior);

    }
	
    void FixedUpdate()
	{
		if (Application.loadedLevel == 1){  // Linux
        //if (Application.isLoadingLevel){  // Mac

			if (ferrariScript == null) {
				ferrariScript = (Consumer) GameObject.FindGameObjectWithTag ("FerrariFactory").GetComponent ("Consumer");
			}

			if (miniScript == null) {
				miniScript = (Consumer) GameObject.FindGameObjectWithTag ("MiniFactory").GetComponent ("Consumer");
			}

			if (truckScript == null) {
				truckScript = (Consumer) GameObject.FindGameObjectWithTag ("TruckFactory").GetComponent ("Consumer");
			}
				
            // is every consumer done with their order?

			if (ferrariScript.frameCounter > 10000 || miniScript.frameCounter > 10000 || truckScript.frameCounter > 10000){

				resultsAvailable  = true;
				valid             = false;

			}

			if (ferrariScript.done && miniScript.done && truckScript.done){

				resultsAvailable    = true;
				valid               = true;


				ferrariScript.done  = false;
				miniScript.done     = false;
				truckScript.done    = false;



				avgFerrariTime = order.ferrariCount == 0 ? 0 : ferrariScript.frameCounter / order.ferrariCount;
				avgMiniTime    = order.miniCount    == 0 ? 0 : miniScript.frameCounter    / order.miniCount;
				avgTruckTime   = order.truckCount   == 0 ? 0 : truckScript.frameCounter   / order.truckCount;

                // avgTime = (avgFerrariTime + avgMiniTime + avgTruckTime);

                carCount = order.ferrariCount + order.miniCount + order.truckCount;

				avgTime = ferrariScript.frameCounter + miniScript.frameCounter + truckScript.frameCounter;
            }
            
        }
    }

	void Awake()
	{
		DontDestroyOnLoad(transform.gameObject);
		FillPlan();
	}
	

}