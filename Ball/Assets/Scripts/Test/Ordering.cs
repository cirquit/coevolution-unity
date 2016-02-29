using UnityEngine;
using System.Collections;
using System.Linq;

public class Ordering {

	public Consumer ferrariScript;
	public Consumer miniScript;
	public Consumer truckScript;

	public int truckCount   = 0;
	public int ferrariCount = 0;
	public int miniCount    = 0;

	// current individual
	private Individual curI = null;
    private Position   curP = null;


    // semaphore so the order will be only executed once
    private object semaphore = new object();

	// is the order already set
	public bool running = false;
	
	public Ordering(Individual curI, Position curP)
	{
		this.curI = curI;
        this.curP = curP;
	}

	// main entry point, will be called from Consumer
	public void StartRequesting()
	{
		this.ferrariScript = (Consumer) GameObject.FindGameObjectWithTag("FerrariFactory").GetComponent("Consumer");
		this.miniScript    = (Consumer) GameObject.FindGameObjectWithTag("MiniFactory").GetComponent("Consumer");
		this.truckScript   = (Consumer) GameObject.FindGameObjectWithTag("TruckFactory").GetComponent("Consumer");

		lock(semaphore){
			if (!running && ferrariScript.loadedConfigs && truckScript.loadedConfigs && miniScript.loadedConfigs){
				running = true;
				RequestOrders();
			}
		}
	}

    // will be called atomically from StartRequesting
	private void RequestOrders()
	{
		foreach(char c in curI.chromosome){
			switch(c){
				case 'T': truckCount++;   break;
				case 'F': ferrariCount++; break;
				case 'M': miniCount++;    break;
				default:                  break;
			}
		}
			
		// Debug.Log("[Ordering]: Trucks requested: " + truckCount);
		// Debug.Log("[Ordering]: Ferrari requested: " + ferrariCount);
		// Debug.Log("[Ordering]: Mini requested: " + miniCount);

		truckScript.transform.position   = new Vector3(curP.positions["Truck"].First, 0.5f, curP.positions["Truck"].Second);
		ferrariScript.transform.position = new Vector3(curP.positions["Ferrari"].First, 0.5f, curP.positions["Ferrari"].Second);
		miniScript.transform.position    = new Vector3(curP.positions["Mini"].First, 0.5f, curP.positions["Mini"].Second);


		if (truckCount > 0) {
			truckScript.SetPartOrder (truckCount);
		} else {
			truckScript.done = true;
		}

		if (ferrariCount > 0) {
			ferrariScript.SetPartOrder (ferrariCount);
		} else {
			ferrariScript.done = true;
		}

		if (miniCount > 0) {
			miniScript.SetPartOrder (miniCount);
		} else {
			miniScript.done = true;
		}

	}
	
}
