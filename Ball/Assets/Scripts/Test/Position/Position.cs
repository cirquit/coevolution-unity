using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class Position {

    // position chromosome
    //             factoryname       x      z
    //              \_______/        |      |
    //                  |            |      |
    public Dictionary<string, Tuple<float, float>> positions = new Dictionary<string, Tuple<float, float>>();

    // Fitness
    public float avgFerrariTime = 0.0f;
    public float avgTruckTime = 0.0f;
    public float avgMiniTime = 0.0f;
    public float avgTime = 0.0f;

    public int carCount = 0;

    
	private float xMin = -9.25f;
	private float xMax = 9.25f;
	
	private float zMin = -9.25f;
	private float zMax = 9.25f;
	


    // if the timeout triggers, then the constellation is not valid
    public bool valid = true;

    private LinkedList<Tuple<float, float>> factoryPositions = new LinkedList<Tuple<float, float>>();

    public Position(){
		initBoundarys();
        initRandomPositions(); 
    }

    /*
     * No bounds checking, use at own risk
     */
    public Position(float miniX, float miniZ, float ferrariX, float ferrariZ, float truckX, float truckZ)
    {
        positions["Mini"]    = new Tuple<float, float>(miniX, miniZ);
        positions["Ferrari"] = new Tuple<float, float>(ferrariX, ferrariZ);
        positions["Truck"]   = new Tuple<float, float>(truckX, truckZ);
		addFactoryPostion(miniX, miniZ);
		addFactoryPostion(ferrariX, ferrariZ);
		addFactoryPostion(truckX, truckZ);
		initBoundarys();
    }

    public float Fitness()
    {
        return carCount == 0 ? 0 : avgTime / carCount;
    }

    public void initRandomPositions()
    {
        string[] consumers = { "Mini", "Truck", "Ferrari" };

        foreach (string c in consumers)
        {

            float x = UnityEngine.Random.Range(xMin, xMax);
            float z = UnityEngine.Random.Range(zMin, zMax);

            while (!IsValid(x, z))
            {
                x = UnityEngine.Random.Range(xMin, xMax);
                z = UnityEngine.Random.Range(zMin, zMax);
            }

            positions[c] = new Tuple<float, float>(x, z);
            factoryPositions.AddFirst(new Tuple<float, float>(x, z));
        }

    }

    public bool IsValid(float x, float z)
    {

        float padding = 1.25f;  // every factroy is 1x1 so 1/2 + 0.25 real padding

        foreach (Tuple<float, float> pos in factoryPositions)
        {
            if (IsBetween(x, pos.First - padding, pos.First + padding))
            {
                return false;
            }

            if (IsBetween(z, pos.Second - padding, pos.Second + padding))
            {
                return false;
            }
        }

		if (!IsBetween(x, xMin, xMax))
		{
			return false;
		}
		
		if (!IsBetween(z, zMin, zMax))
		{
			return false;
		}

        return true;
    }

    private bool IsBetween(float x, float minX, float maxX)
    {
        return x > minX && x < maxX;
    }

    public string GetMiniPos()
    {
        // return "M:(" + Math.Round(positions["Mini"].First, 2) + ", " + Math.Round(positions["Mini"].Second, 2) + ")";
		return "M:(" + positions["Mini"].First + ", " + positions["Mini"].Second + ")";
    }

    public string GetFerrariPos()
    {
        // return "F:(" + Math.Round(positions["Ferrari"].First, 2) + ", " + Math.Round(positions["Ferrari"].Second, 2) + ")";
		return "F:(" + positions["Ferrari"].First + ", " + positions["Ferrari"].Second + ")";
    }

    public string GetTruckPos()
    {
        // return "T:(" + Math.Round(positions["Truck"].First, 2) + ", " + Math.Round(positions["Truck"].Second, 2) + ")";
		return "T:(" + positions["Truck"].First + ", " + positions["Truck"].Second + ")";
    }

	private void addFactoryPostion(float x, float z){
		factoryPositions.AddFirst(new Tuple<float, float>(x, z));
	}

	private void initBoundarys(){

		Tuple<float, float> chassiXZ   = new Tuple<float, float>(2.5f, 4.0f);
		Tuple<float, float> windowXZ   = new Tuple<float, float>(2.5f, 0.0f);
		Tuple<float, float> interiorXZ = new Tuple<float, float>(2.5f, -4.0f);
		
		Tuple<float, float> rubberXZ = new Tuple<float, float>(-3.0f, 4.0f);
		Tuple<float, float> sandXZ   = new Tuple<float, float>(-3.0f, 0.0f);
		Tuple<float, float> oreXZ    = new Tuple<float, float>(-3.0f, -4.0f);

		factoryPositions.AddFirst(chassiXZ);
		factoryPositions.AddFirst(windowXZ);
		factoryPositions.AddFirst(interiorXZ);
		
		factoryPositions.AddFirst(rubberXZ);
		factoryPositions.AddFirst(sandXZ);
		factoryPositions.AddFirst(oreXZ);
	}

}
