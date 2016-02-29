using UnityEngine;
using System.Collections;

public class Mutation{

	// how many individuals are going to be "selected" to be mutated
	private float mutSelectionP;

	// how much of the chromosome is going to be mutated
	private float mutationP;
	
	public Mutation(float mutSelectionP, float mutationP){
		this.mutSelectionP = mutSelectionP;
		this.mutationP     = mutationP;
	}
	
	public Individual[] Apply(Individual[] individuals)
	{
		for(int i = 0; i < individuals.Length; i++){

			float range     = Random.Range(0.0f, 1.0f);
			bool isSelected = range <= mutSelectionP;
//			Debug.Log ("MutIndiv: " + i + " range: " + range.ToString() + " bool: " + isSelected.ToString());
			if (isSelected){
				individuals[i] = MutateIndividual(individuals[i]);
			}
		}
		return individuals;
	}

	private Individual MutateIndividual(Individual individual){
		for(int i = 0; i < Individual.LENGTH; i++){
			bool mutateGene = Random.Range(0.0f, 1.0f) <= mutationP;
			if (mutateGene){
				individual.chromosome[i] = individual.GenRandomGene();
			}
		}

		return individual;
	}
}
