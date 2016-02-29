using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GA : MonoBehaviour {

    /*
     * GLOBAL GENETIC ALGORITHM DEFINTION
     */


    // generation count - how often do we want to run a loop through the genetic algorithm
    private int generationCount = 100;

    // population size - e.g. how many individuals + positions do we have
    public static int popSize   = 10;

    /*
     * INDIVIDUAL GENETIC ALGORITHM DEFINTION
     */

    // natural selection percentage - this can not be below 0.5, because of our genetic algorithm
    public static float natSelectionP   = 0.8f;

	// mutation selection percentage - how many individuals do we want to mutate
	public static float mutSelectionP   = 0.1f;

	// mutation percentage - how much of the chromosome should be mutated
	public static float mutationP       = 0.3f;

	// basic setup for the genetic individual algorithm
	private static IndividualFactory iFactory         = new IndividualFactory(popSize);
	private static NaturalSelection  naturalSelection = new NaturalSelection(natSelectionP, iFactory);
	private static Crossover         crossover        = new Crossover(1, iFactory);
	private static Mutation          mutation         = new Mutation(mutSelectionP, mutationP);
	public  static Individual[]      population       = iFactory.GenIndividuals();


    /*
     * POSITION GENETIC ALGORITHM DEFINTION
     */

    // natural selection percentage - this can not be below 0.5, because of our genetic algorithm
    public static float natSelectionPosP = 0.7f;

    // mutation selection percentage - how many individuals do we want to mutate
    public static float mutSelectionPosP = 0.5f;

    // mutation percentage - how much should the X or Z position be moved in case of mutation
    public static float mutationPosP     = 1.5f;

    // basic setup for the genetic position algorithm 

    private static PositionFactory   pFactory          = new PositionFactory(popSize);
	private static PNaturalSelection pNaturalSelection = new PNaturalSelection(natSelectionPosP, pFactory);
    private static PCrossover        pCrossover        = new PCrossover(1, pFactory);
    private static PMutation         pMutation         = new PMutation(mutSelectionPosP, mutationPosP);
    public  static Position[]        pPopulation       = pFactory.GenPositions();

    /*
	 * State Machine
	 */

    // signals if we are in the start / end state
    public bool active               = false;

	// signals if a simulation is currently running
	public bool waitingForSimulation = false;

	// signals if we've evaluated the fitness for the whole population
	public bool popFitnessEvaluated  = false;

	// which individuum is currently being simulated - bounds [0..popSize-1] | (-1) so we can just trigger active and the 'RequestNextSimulation' starts with zero
	public static int  currentIndividual    = -1;
	
	// we can start the simulation and read the results through this script
	SimController simulationScript   = null;

	void Update()
	{
		if (active) {
	
			if (waitingForSimulation) {
				if (!Application.isLoadingLevel) {
			    	if (simulationScript.resultsAvailable) {
						Debug.Log("[GA] Results are available: avgFTime - " + simulationScript.avgTime.ToString());
						UpdateFitness (population [currentIndividual], pPopulation[currentIndividual]);

					}
				}

			}

			// if we are not waiting for a simulation to end, check if the whole population was already evaluated
			if (!waitingForSimulation && !popFitnessEvaluated) {
				RequestNextSimulation ();
			}

			// if we are not waiting for a simulation to end and the whole population was evaluated, start the GA
			if (!waitingForSimulation && popFitnessEvaluated) {
				Debug.Log ("[GA] Population fitness evaluated. Starting GA");
				ApplyGARound ();
			}

		} else {
			// @ TODO show some results
		}
	}

	/**
	 * updates the fitness of our current individual and resets the corresponding values in SimController
	 */
	private void UpdateFitness(Individual curI, Position curP)
	{
        //	curI.avgFerrariTime  = simulationScript.avgFerrariTime +  curI.avgFerrariTime  
        //	curI.avgMiniTime     = simulationScript.avgMiniTime;   +  curI.avgMiniTime     
        //	curI.avgTruckTime    = simulationScript.avgTruckTime;  +  curI.avgTruckTime    

        //    curP.avgFerrariTime  = simulationScript.avgFerrariTime;
        //    curP.avgMiniTime     = simulationScript.avgMiniTime;
        //    curP.avgTruckTime    = simulationScript.avgTruckTime;

        if (simulationScript.valid)
        {
            curI.avgTime  = simulationScript.avgTime  + curI.avgTime;
            curI.carCount = simulationScript.carCount + curI.carCount;
            curI.valid    = simulationScript.valid;

            curP.avgTime  = simulationScript.avgTime   + curP.avgTime;
            curP.carCount = simulationScript.carCount + curP.carCount;
            curP.valid    = simulationScript.valid;

        } else
        {
            if (curI.avgTime == 0)
            {
                curI.valid = simulationScript.valid;
            }

            curP.valid = simulationScript.valid;
        }

        simulationScript.avgFerrariTime   = 0.0f;
		simulationScript.avgMiniTime      = 0.0f;
		simulationScript.avgTruckTime     = 0.0f;
		simulationScript.avgTime          = 0.0f;
        simulationScript.carCount         = 0;
		simulationScript.resultsAvailable = false;

		waitingForSimulation = false;
	}

	/*
	 * - checks if the we already evaluated all the individuals in our population
	 * - if yes -> popFitnessEvaluated = true
	 * - if no  -> try to start a simulation for the next indivial and check if he was not already evaluated
	 *          -> waitingForSimulation = true
	 */
	private void RequestNextSimulation()
	{
		currentIndividual += 1;
		if (currentIndividual < popSize) {

			Individual curI = null;
            Position   curP = null;

			 if (currentIndividual == 0){
			
				Debug.Log ("using first");
				curI = iFactory.GenIndividual("MFTNNNNNNN".ToCharArray());
				curP = pFactory.GenPosition(-5.422888f, 2.5297f, -8.638563f, -8.653988f, 9.133038f, -6.610527f);
			} else {
				curI = population [currentIndividual];
                curP = pPopulation[currentIndividual];
			}



				waitingForSimulation = true;

				Debug.Log ("[GA]: Requesting simulation for: " + curI.chromosomeToString ());

				Ordering order = new Ordering (curI, curP);
			    simulationScript.StartSimulation (order);
			}

	    else {
			popFitnessEvaluated = true;
		}
	}

	/*
	 * main entry point for the GA
	 */
	private void ApplyGARound()
	{
		Individual[] best        = naturalSelection.Apply (population);
		Individual[] children    = crossover.Apply (best);
		Individual[] mutChildren = mutation.Apply (children);
		Individual[] newPop      = naturalSelection.Repopulate (best, mutChildren);

		population = newPop;

        Position[] pBest        = pNaturalSelection.Apply(pPopulation);
        Position[] pChildren    = pCrossover.Apply(pBest);
        Position[] pMutChildren = pMutation.Apply(pChildren);
		Position[] pMutBest     = pMutation.Apply(pBest);
        Position[] pNewPop      = pNaturalSelection.Repopulate(pMutBest, pMutChildren);

        pPopulation = pNewPop;

        RestartStateMachine();
	}

	/*
	 * if we run all generations, we are done -> active = false;
	 * otherwise means we just ran a generation and have to reset our individual index + our population is no more evaluated
	 */
	private void RestartStateMachine()
	{
		if (generationCount == 0) {
			active = false;
		} else {
			generationCount--;
			currentIndividual   = -1;
			popFitnessEvaluated = false;
		}

	}

	/* - this will be called when all the objects are initialized (safe start)
	 * - copy the gameobject to every scene we call, without resetting or destroying it
	 * - loads the SimulationController script to start simulations based on our individuals
	 */
	void Awake()
	{
		simulationScript = (SimController) GameObject.FindGameObjectWithTag("GeneticAlgorithm").GetComponent("SimController");
		DontDestroyOnLoad(gameObject);
		active = true;
		//Random.seed = 123123123;
	}


/*
	// simple lable showing us the last recorded time
	void OnGUI()
    { 
		int max = popSize;
		int height = 20;

		int width = 200;


		// requests

		GUI.Box(new Rect(0,0, width,max * height),"");

		// popsize shouldn't be lesser than 1

		for(int i = 0; i < max; i++){

            string field = i.ToString() + ". " + population[i].chromosomeToString() + " | " + population[i].Fitness().ToString();
			GUI.TextField(new Rect(0, i * height, width, height), field);
		}


		// positions 

        GUI.Box(new Rect(850, 0, 550, max * height), "");

        for (int i = 0; i < max; i++)
        {
            string pField = i.ToString() + ". " + pPopulation[i].GetMiniPos() + " | " + pPopulation[i].GetFerrariPos() + " | " + pPopulation[i].GetTruckPos() + " | "  + pPopulation[i].Fitness().ToString();
            GUI.TextField(new Rect(850, i * height, 550, height), pField);
        }



        GUI.Box(new Rect(0, 400, 200, 40), "");
        GUI.TextField(new Rect(0, 400, 200, 20), "Individual #" + currentIndividual.ToString());
        GUI.TextField(new Rect(0, 420, 200, 20), "Generation #" + generationCount.ToString());
    } */
}