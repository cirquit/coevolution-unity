using UnityEngine;
using System.Collections;
using System.Linq;

public class NaturalSelection {

	private float             natSelectionP;
	private IndividualFactory iFactory;

	public NaturalSelection(float natSelectionP, IndividualFactory iFactory){
		this.natSelectionP = natSelectionP;
		this.iFactory      = iFactory;
	}

	/**
	 * - returns a new population with only the best individuals
	 * - length is based on the natSelectionP 
	 */
	public Individual[] Apply(Individual[] individuals)
	{
		int validCount = 0;

		foreach(Individual i in individuals){
			if (i.valid) validCount++;
		}

		Individual[] validIndividuals = new Individual[validCount];

		int j = 0;

		for(int i = 0; i < individuals.Count(); i++){
			if(individuals[i].valid){
				validIndividuals[j] = individuals[i];
				j++;
			}
		}

		float toBeRemoved = individuals.Length * natSelectionP;
		float toBeRemovedValid = toBeRemoved - (individuals.Count() - validCount);

	//	Debug.Log("[GA-NS] validIndividuals length: " + validIndividuals.Count().ToString());

		validIndividuals = validIndividuals.OrderByDescending(inv => inv.Fitness()).ToArray();

		int best = Mathf.FloorToInt(validIndividuals.Length - toBeRemovedValid);

		Individual[] selected = new Individual[best];

		for(int i = 0; i < best; i++){
			selected[i] = validIndividuals[i];
		}

		return selected;
	}
	
	/**
	 * - repopulates the individuals to the specified length in IndividualFactory in that order
	 * - [pop : (if not enough randomPop)]
	 */
	public Individual[] Repopulate(Individual[] pop)
	{
		Individual[] newPopulation = new Individual[iFactory.amount];

		for (int i = 0; i < iFactory.amount; i++){
			if (i < pop.Length){
				newPopulation[i] = pop[i];
			} else {
				newPopulation[i] = iFactory.GenIndividual();
			}
		}

       // Shuffle(newPopulation);

        return newPopulation;
	}

	/**
	 * - repopulates the individuals to the specified length in IndividualFactory in that order
	 * - [popA : popB : (if not enough randomPop)]
	 */
	public Individual[] Repopulate(Individual[] popA, Individual[] popB)
	{
		Individual[] newPopulation = new Individual[iFactory.amount];

	//	Debug.Log ("[GA-NS]: popA.Count = " + popA.Count());
	//	Debug.Log ("[GA-NS]: popB.Count = " + popB.Count());
	//	Debug.Log ("[GA-NS]: iFactory.Count = " + iFactory.amount);


		for (int i = 0; i < iFactory.amount; i++){
			if (i < popA.Length){
				newPopulation[i] = popA[i];
			} else if (i < popA.Length + popB.Length){
				newPopulation[i] = popB[i-popA.Length];
			} else {
				newPopulation[i] = iFactory.GenIndividual();
			}
		}

        // Shuffle(newPopulation);

		return newPopulation;
	}

    // Fisher Yates
    public void Shuffle(Individual[] list)
    {
        int n = list.Length;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, (n + 1));
            Individual value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}
