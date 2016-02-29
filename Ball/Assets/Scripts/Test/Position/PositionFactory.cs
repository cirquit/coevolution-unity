using UnityEngine;
using System.Collections;

public class PositionFactory { 
    
    public int amount = 0;

    public PositionFactory(int amount)
    {
        this.amount = amount;
    }


    public Position[] GenPositions()
    {
        Position[] positions = new Position[amount];

        for (int i = 0; i < amount; i++)
        {
            positions[i] = GenPosition();
        }

        return positions;
    }


    public Position GenPosition()
    {
        return new Position();
    }

    /*
     * No bounds checking, use at own risk
     */
    public Position GenPosition(float miniX, float miniZ, float ferrariX, float ferrariZ, float truckX, float truckZ)
    {
        return new Position(miniX, miniZ, ferrariX, ferrariZ, truckX, truckZ);
    }
}
