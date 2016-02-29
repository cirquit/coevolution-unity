using UnityEngine;
using System.Collections;

public class PMutation {

    // how many positions are going to be "selected" to be mutated
    private float mutSelectionPosP;

    // mutation percentage - how much should the X or Z position be moved in case of mutation
    private float mutationPosP;

    public PMutation(float mutSelectionPosP, float mutationPosP)
    {
        this.mutSelectionPosP = mutSelectionPosP;
        this.mutationPosP = mutationPosP;
    }

    public Position[] Apply(Position[] positions)
    {
        for (int i = 0; i < positions.Length; i++)
        {

            float range = Random.Range(0.0f, 1.0f);
            bool isSelected = range <= mutSelectionPosP;
            //			Debug.Log ("MutIndiv: " + i + " range: " + range.ToString() + " bool: " + isSelected.ToString());
            if (isSelected)
            {
                positions[i] = MutatePosition(positions[i]);
            }
        }
        return positions;
    }
    
    // @ TODO remove boilerplate...

    private Position MutatePosition(Position position)
    {
        int randomCord = UnityEngine.Random.Range(1, 6);

        float x = 0;
        float z = 0;

        switch (randomCord)
        {
            case 1:

                x = position.positions["Mini"].First;
                z = position.positions["Mini"].Second;
                if (position.IsValid(x + mutationPosP, z))
                {
                    position.positions["Mini"].First = x + mutationPosP;
                }
                else if (position.IsValid(x - mutationPosP, z))
                {
                    position.positions["Mini"].First = x - mutationPosP;
                }
                break;

            case 2:
                x = position.positions["Mini"].First;
                z = position.positions["Mini"].Second;
                if (position.IsValid(x, z + mutationPosP))
                {
                    position.positions["Mini"].Second = z + mutationPosP;
                }
                else if (position.IsValid(x, z - mutationPosP))
                {
                    position.positions["Mini"].Second = z - mutationPosP;
                }
                break;

            case 3:

                x = position.positions["Ferrari"].First;
                z = position.positions["Ferrari"].Second;
                if (position.IsValid(x + mutationPosP, z))
                {
                    position.positions["Ferrari"].First = x + mutationPosP;
                }
                else if (position.IsValid(x - mutationPosP, z))
                {
                    position.positions["Ferrari"].First = x - mutationPosP;
                }
                break;

            case 4:
                x = position.positions["Ferrari"].First;
                z = position.positions["Ferrari"].Second;
                if (position.IsValid(x, z + mutationPosP))
                {
                    position.positions["Ferrari"].Second = z + mutationPosP;
                }
                else if (position.IsValid(x, z - mutationPosP))
                {
                    position.positions["Ferrari"].Second = z - mutationPosP;
                }
                break;

            case 5:

                x = position.positions["Truck"].First;
                z = position.positions["Truck"].Second;
                if (position.IsValid(x + mutationPosP, z))
                {
                    position.positions["Truck"].First = x + mutationPosP;
                }
                else if (position.IsValid(x - mutationPosP, z))
                {
                    position.positions["Truck"].First = x - mutationPosP;
                }
                break;

            case 6:
                x = position.positions["Truck"].First;
                z = position.positions["Truck"].Second;
                if (position.IsValid(x, z + mutationPosP))
                {
                    position.positions["Truck"].Second = z + mutationPosP;
                }
                else if (position.IsValid(x, z - mutationPosP))
                {
                    position.positions["Truck"].Second = z - mutationPosP;
                }
                break;

            default: break;
        }

        return position;
    }
}
