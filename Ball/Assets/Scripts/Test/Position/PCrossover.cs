using UnityEngine;
using System.Collections;

public class PCrossover {

    // defines the type of crossover we are using (no higher order functions available...)
    private int mode;

    private PositionFactory pFactory = null;

    /**
	 * 1 <-> OnePointCrossover
	 * 2 <-> TwoPointCrossover (not implemented)
	 */
    public PCrossover(int mode, PositionFactory pFactory)
    {
        this.mode = mode;
        this.pFactory = pFactory;
    }

    /**
	 *  - Sliding window method with length 2
	 *  - Custom crossover for i = 0 & i = parents.Length-1, because we want everybody to
	 *    be used twice
	 *  - returns the same amount of children as the parents
	 */

    public Position[] Apply(Position[] parents)
    {
        Position[] children = new Position[parents.Length];

        if (parents.Length > 1)
        {
            for (int i = 0; i < parents.Length - 1; i++)
            {
                Position a = parents[i];
                Position b = parents[i + 1];

                switch (mode)
                {
                    case 1: children[i] = OnePointCrossover(a, b); break;
                    case 2: Debug.Log("TwoPointCrossover not implemented yet."); break;
                    case 3: Debug.Log("UniformCrossover not implemented yet."); break;
                    default: Debug.Log("Crossover was set to a mode that is not implemented yet: " + mode); break;
                };
            }
        }

        if (parents.Length > 0)
        {

            Position first = parents[0];
            Position last = parents[parents.Length - 1];

            switch (mode)
            {
                case 1: children[children.Length - 1] = OnePointCrossover(first, last); break;
                case 2: Debug.Log("TwoPointCrossover not implemented yet."); break;
                case 3: Debug.Log("UniformCrossover not implemented yet."); break;
                default: Debug.Log("Crossover was set to a mode that is not implemented yet: " + mode); break;
            };
        }

        return children;

    }

    /**
	 * (aX1, aZ1), (aX2, aZ2), (aX3, aZ3)
	 * (bX1, bZ1), (bX2, bZ2), (bX3, bZ3)
	 * 
	 * x = Random.Range[1,2]
	 * x = 1
	 * 
     * (aX1, aZ1), | (bX2, bZ2), (bX3, bZ3)  => picking the first one
	 * (bX1, bZ1), | (aX2, aZ2), (aX3, aZ3)
	 * 
     * if the above are not valid, return random position A or B
	 */

    public Position OnePointCrossover(Position a, Position b)
    {
        int randomCar = Random.Range(1, 2);

        float aX1 = a.positions["Mini"].First;
        float aZ1 = a.positions["Mini"].Second;

        float aX2 = a.positions["Ferrari"].First;
        float aZ2 = a.positions["Ferrari"].Second;

        float aX3 = a.positions["Truck"].First;
        float aZ3 = a.positions["Truck"].Second;

        float bX1 = b.positions["Mini"].First;
        float bZ1 = b.positions["Mini"].Second;

        float bX2 = b.positions["Ferrari"].First;
        float bZ2 = b.positions["Ferrari"].Second;

        float bX3 = b.positions["Truck"].First;
        float bZ3 = b.positions["Truck"].Second;

        switch (randomCar)
        {
            case 1:
                if (b.IsValid(aX1, aZ1))
                {
                    return pFactory.GenPosition(aX1, aZ1, bX2, bZ2, bX3, bZ3);
                }
                break;

            case 2:
                if (a.IsValid(bX3, bZ3))
                {
                    return pFactory.GenPosition(aX1, aZ1, aX2, aZ2, bX3, bZ3);
                }
                break;

            default: break;

        }

        if (Random.Range(1, 2) == 1)
        {
            return pFactory.GenPosition(aX1, aZ1, aX2, aZ2, aX3, aZ3);
        }
        else
        {
            return pFactory.GenPosition(bX1, bZ1, bX2, bZ2, bX3, bZ3);
        }
    }

}
