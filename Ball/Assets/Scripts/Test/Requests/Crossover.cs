using UnityEngine;
using System.Collections;

public class Crossover {

	// defines the type of crossover we are using (no higher order functions available...)
	private int mode;

	private IndividualFactory iFactory = null;

	/**
	 * 1 <-> OnePointCrossover
	 * 2 <-> TwoPointCrossover
	 * 3 <-> UniformCrossover
	 */
	public Crossover(int mode, IndividualFactory iFactory)
	{
		this.mode = mode;
		this.iFactory = iFactory;
	}

	/**
	 *  - Sliding window method with length 2
	 *  - Custom crossover for i = 0 & i = parents.Length-1, because we want everybody to
	 *    be used twice
	 *  - returns the same amount of children as the parents
	 */

	public Individual[] Apply(Individual[] parents)
	{
		Individual[] children = new Individual[parents.Length];

		if (parents.Length > 1) {

			for(int i = 0; i < parents.Length-1; i++){

				Individual a = parents[i];
				Individual b = parents[i+1];

				switch(mode){
					case 1: children[i] = OnePointCrossover(a,b); 				  break;
					case 2: children[i] = TwoPointCrossover(a,b);                 break;
					case 3:  Debug.Log("UniformCrossover not implemented yet.");  break;
					default: Debug.Log("Crossover was set to a mode that is not implemented yet: " + mode); break;
				};
			}
		}

		if (parents.Length > 0){

			Individual first = parents[0];
			Individual last  = parents[parents.Length-1];

			switch(mode){
				case 1: children[children.Length-1] = OnePointCrossover(first, last); break;
				case 2: children[children.Length-1] = TwoPointCrossover(first, last); break;
				case 3:  Debug.Log("UniformCrossover not implemented yet.");  		 break;
				default: Debug.Log("Crossover was set to a mode that is not implemented yet: " + mode); break;
			};
		}

		return children;

	}

	/**
	 * AAAAAA
	 * BBBBBB
	 * 
	 * x = Random.Range[0,5]
	 * x = 2
	 * 
	 * AA | AAAA
	 * BB | BBBB
	 * 
	 * => AA BBBB
	 * => BB AAAA // picking the first one, because c# doesn't support tuples...
	 * 
	 */
	
	public Individual OnePointCrossover (Individual a, Individual b)
	{
		int x = Random.Range(0, Individual.LENGTH);
		
		char[] chromosome = new char[Individual.LENGTH];
		
		for (int i = 0; i < Individual.LENGTH; i++){
			chromosome[i] = (i < x) ? a.getGeneAt(i) : b.getGeneAt(i);
		}

		return iFactory.GenIndividual(chromosome);
	}

	/**
	 * AAAAAA
	 * BBBBBB
	 * 
	 * x = Random.Range[0,5]
	 * x = 2
	 * 
	 * AA | AAAA
	 * BB | BBBB
	 * 
	 * y = Random.Range[x,5]
	 * y = 4
	 * 
	 * AA | AA | AA
	 * BB | BB | BB
	 * 
	 * => AA BB AA
	 * => BB AA BB // picking the first one, because c# doesn't support tuples...
	 * 
	 */

	public Individual TwoPointCrossover (Individual a, Individual b)
	{
		int x = Random.Range(0, Individual.LENGTH);
		int y = Random.Range(x, Individual.LENGTH);

		char[] chromosome = new char[Individual.LENGTH];

		for (int i = 0; i < Individual.LENGTH; i++){
			chromosome[i] = (i < x || i > y) ? a.getGeneAt(i) : b.getGeneAt(i);
		}

		return iFactory.GenIndividual(chromosome);
	}
}
