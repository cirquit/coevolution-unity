using UnityEngine;
using System.Collections;

public class IndividualFactory {

	public int amount;

	public IndividualFactory(int amount)
	{
		this.amount = amount;
	}


	public Individual[] GenIndividuals()
	{
		Individual[] individuals = new Individual[amount];

		for(int i = 0; i < amount; i++){
			individuals[i] = GenIndividual();
		}

		return individuals;
	}

	public Individual GenIndividual(){
		return new Individual();
	}

	public Individual GenIndividual(char[] chromosome){
		return new Individual(chromosome);
	}


}
