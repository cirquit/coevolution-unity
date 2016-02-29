using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using System.Text;

public class SocketConnection : MonoBehaviour {

	//socket setup
	UdpClient socket;
	string ip = "127.0.0.1";
	int port = 10001;

	float max = 0;
	float min = 3000;
	int valuesToSend = 2;

	//coroutine not started
	bool started = false;

	//two 'best' values from the test
	int theBest = 2;

	bool sent = false;

	//population counter, stops at theBest
	int populationCounter = 0;

	// start socket connection
	void Start () {
		socket = new UdpClient();
		socket.Connect(ip, port);
		Debug.Log ("Socket connection");
	}

	//starts coroutine if level is loaded and coroutine was not started yet
	void Update(){
		if(!Application.isLoadingLevel && !started){
				StartCoroutine ("Log");
				started = true;
			}
		}
		
		
	//sends average time values to server
	IEnumerator Log() {
		while (true) {
			string avgTime = "";

			if (sent && (GA.currentIndividual != (GA.popSize - 1))) {
				sent = false;
			}

			//if it's the last individual in the population, look for maximum
			if (GA.currentIndividual == (GA.popSize-1) && !sent) {

				//search for 2 best values
//				foreach (Individual value in GA.population) {
//					if (value.Fitness () > max) {
//						secondMax = max;
//						max = value.Fitness ();
//						Debug.Log ("max " + max);
//					} else if (value.Fitness () > secondMax && value.Fitness () < max) {
//						secondMax = value.Fitness ();
//						Debug.Log ("max " + secondMax);
//
//					}
//				}

				foreach (Individual value in GA.population) {
					if (value.Fitness () > max) {
						max = value.Fitness ();
						Debug.Log ("max " + max);
					}
				}

				foreach (Position value1 in GA.pPopulation) {
					if ((value1.Fitness () < min) && (value1.Fitness() != 0)) {
						min = value1.Fitness ();
						Debug.Log ("min " + min);
					}
				}

				for (int i = 0; i < valuesToSend; i++) {				
					StringBuilder json = new StringBuilder ();

					json.Append ("{");
					json.Append ("\"value\" : ");
					if (i == 0 ) {
						avgTime = max.ToString ();
						max = 0;
					} else if (i == 1) {
						avgTime = min.ToString ();
						min = 3000;
					}
					json.Append (avgTime);
					json.Append ("}");

					byte[] data = Encoding.UTF8.GetBytes (json.ToString ());
					socket.Send (data, data.Length);

					populationCounter += 1;
					Debug.Log ("Sent: " + json.ToString ());
				}
				sent = true;
			}
        yield return new WaitForSeconds(1f);
		}
	}

	void Awake()
	{
		DontDestroyOnLoad(gameObject);
	}
}
