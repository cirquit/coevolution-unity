using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Individual {

	// encoded chromosome
	public static int    LENGTH     = 10;
	public        char[] chromosome = new char[LENGTH];

	// Fitness
	public float avgFerrariTime = 0.0f;           
	public float avgTruckTime   = 0.0f;           
	public float avgMiniTime    = 0.0f;           
	public float avgTime        = 0.0f;

    public int   carCount       = 0;

    // was the simulation a success?  // should be refactored with a simple pathfinding algorithm
    public bool   valid = true;

	/** Encoding
	 * 
	 * F = Ferrari, T = Truck, M = Mini, N = None
	 */ 
	private static char[] ENCODING = new char[4] { 'F', 'M', 'T', 'N' };

	public Dictionary<char, string> chromDecoding = new Dictionary<char, string>();

	public Individual()
	{
		for(int i = 0; i < LENGTH; i++){
			chromosome[i] = GenRandomGene();
		}
	}

	public Individual(char[] chromosome)
	{
		this.chromosome = chromosome;
    }

    public float Fitness()
    {
        return carCount == 0 ? 0 : avgTime / carCount;
    }


	/**
 	* - this *should* throw an error, because ENCODING.Length is 4
 	*   and Random.Range(int min, int max) uses inclusive boundarys
 	* - somehow the last possibility 'N' won't show up if we use ENCODING.Length - 1
 	*/
	public char GenRandomGene()
	{
		return ENCODING[Random.Range(0, ENCODING.Length)];
	}

	public char getGeneAt(int i)
	{
		return chromosome[i];
	}

	private void initDict()
	{
		chromDecoding.Add('F', "Ferrari");
		chromDecoding.Add('M', "Mini");
		chromDecoding.Add('T', "Truck");
		chromDecoding.Add('N', "None");
	}

	public string chromosomeToString()
	{
		int miniCount = 0;
		int truckCount = 0;
		int ferrariCount = 0;

		foreach(char c in chromosome){
			switch (c){
			case 'M': miniCount++; break;
			case 'T': truckCount++; break;
			case 'F': ferrariCount++; break;
			default: break;
			}
		}

		return "M: " + miniCount.ToString() + ", F: " + ferrariCount.ToString() + ", T: " + truckCount.ToString();
	}
}








