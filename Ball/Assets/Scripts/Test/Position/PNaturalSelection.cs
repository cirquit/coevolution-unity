using UnityEngine;
using System.Collections;
using System.Linq;

public class PNaturalSelection {

    private float natSelectionPosP;
    private PositionFactory pFactory;

    public PNaturalSelection(float natSelectionPosP, PositionFactory pFactory)
    {
        this.natSelectionPosP = natSelectionPosP;
        this.pFactory = pFactory;
    }


    /**
     * - returns a new population with only the best Positions
    * - length is based on the natSelectionPosP 
    */
    public Position[] Apply(Position[] positions)
    {
        int validCount = 0;

        foreach (Position p in positions)
        {
            if (p.valid) validCount++;
        }

        Position[] validPositions = new Position[validCount];

        int j = 0;

        for (int i = 0; i < positions.Length; i++)
        {
            if (positions[i].valid)
            {
                validPositions[j] = positions[i];
                j++;
            }
        }

        float toBeRemoved = positions.Length * natSelectionPosP;
        float toBeRemovedValid = toBeRemoved - (positions.Length - validCount);


		// generate worst position
		validPositions = validPositions.OrderByDescending(inv => inv.Fitness()).ToArray();

		// generate best position
		// validPositions = validPositions.OrderBy(inv => inv.Fitness()).ToArray();
		
        int best = Mathf.FloorToInt(validPositions.Length - toBeRemovedValid);

        Position[] selected = new Position[best];

        for (int i = 0; i < best; i++)
        {
            selected[i] = validPositions[i];
        }

        return selected;
    }

    /**
	 * - repopulates the positions to the specified length in PositionFactory in that order
	 * - [pop : (if not enough randomPop)]
	 */
    public Position[] Repopulate(Position[] pop)
    {
        Position[] newPopulation = new Position[pFactory.amount];

        for (int i = 0; i < pFactory.amount; i++)
        {
            if (i < pop.Length)
            {
                newPopulation[i] = pop[i];
            }
            else {
                newPopulation[i] = pFactory.GenPosition();
            }
        }

        // Shuffle(newPopulation);

        return newPopulation;
    }

    /**
	 * - repopulates the positions to the specified length in PositionFactory in that order
	 * - [popA : popB : (if not enough randomPop)]
	 */
    public Position[] Repopulate(Position[] popA, Position[] popB)
    {
        Position[] newPopulation = new Position[pFactory.amount];

  //      Debug.Log("[GA-NS]: popA.Count = " + popA.Count());
  //      Debug.Log("[GA-NS]: popB.Count = " + popB.Count());
  //      Debug.Log("[GA-NS]: iFactory.Count = " + pFactory.amount);


        for (int i = 0; i < pFactory.amount; i++)
        {
            if (i < popA.Length)
            {
                newPopulation[i] = popA[i];
            }
            else if (i < popA.Length + popB.Length)
            {
                newPopulation[i] = popB[i - popA.Length];
            }
            else {
                newPopulation[i] = pFactory.GenPosition();
            }
        }

      //  Shuffle(newPopulation);

        return newPopulation;
    }

    // Fisher Yates
    public void Shuffle(Position[] list)
    {
        int n = list.Length;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, (n + 1));
            Position value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}
