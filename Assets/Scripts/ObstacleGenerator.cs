using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObstacleGenerator : MonoBehaviour {

	public static ObstacleGenerator instance;

	public GameObject fish, powerPellet;
	public GameObject[] obstacleList;
	public ArrayList generatedObstacles;
	public bool generatedPowerPellet;

	float rowDistance;

	public Dictionary<int, int[,]> obstaclePatterns = new Dictionary<int, int[,]> ();

	//create obstacle patterns
	int [,]op1 = {{1, 1, -1}, {0, -1, 1}, {1, 1, -1}};
	int [,]op2 = {{1, 1, -1}, {-1, 1, 1}, {-1, 1, 0}};
	int [,]op3 = {{1, 1, -1}, {-1, 0, 1}, {1, 1, -1}};
	int [,]op4 = {{1, 1, -1}, {-1, 1, 1}, {0, 1, -1}};
	int [,]op5 = {{1, -1, 1}, {0, 1, -1}, {1, -1, 1}};
	int [,]op6 = {{1, -1, 1}, {-1, 1, -1}, {1, 0, 1}};
	int [,]op7 = {{1, -1, 1}, {0, 1, -1}, {1, 1, -1}};
	int [,]op8 = {{1, -1, 1}, {-1, 1, 0}, {1, 1, -1}};

	int currentPattern, row, mirrored, temp;
	public GameObject obstacleGeneratorTrigger, obstacleGroupGeneratorTrigger;

	Vector3 pos;
	int lane;

	void Awake(){
		Debug.Log ("Obstacle Generator AWAKE");
		instance = this;
	}

	void Start(){
		Debug.Log ("Obstacle Generator Start");
	}

	// Use this for initialization
	public void StartGenerator () {
		row = 1;
		currentPattern = 1;
		mirrored = 1;
		rowDistance = 2.5f;
		generatedPowerPellet = false;
		generatedObstacles = new ArrayList();

		if(!obstaclePatterns.ContainsKey(1))
			obstaclePatterns.Add (1, op1);
		if(!obstaclePatterns.ContainsKey(2))
			obstaclePatterns.Add (2, op2);
		if(!obstaclePatterns.ContainsKey(3))
			obstaclePatterns.Add (3, op3);
		if(!obstaclePatterns.ContainsKey(4))
			obstaclePatterns.Add (4, op4);
		if(!obstaclePatterns.ContainsKey(5))
			obstaclePatterns.Add (5, op5);
		if(!obstaclePatterns.ContainsKey(6))
			obstaclePatterns.Add (6, op6);
		if(!obstaclePatterns.ContainsKey(7))
			obstaclePatterns.Add (7, op7);
		if(!obstaclePatterns.ContainsKey(8))
			obstaclePatterns.Add (8, op8);
		
		generateObstacleGroup ();
	}

	public void generateObstacle(){
		//Debug.Log ("Obstacle Generator generateObstacle");
		//generate obstacles in current row and set the row var as next row
		if (row == 2) {
			for (int j = 0; j < 3; j++) {
				//generate some random obstacle for each 1 in the pattern
				if (obstaclePatterns [currentPattern] [1, j] == 1) {
					pos = new Vector3 (mirrored * (j-1), transform.position.y, transform.position.z);
					generatedObstacles.Add ((GameObject)Instantiate (obstacleList [Random.Range (0, obstacleList.Length)], pos, Quaternion.identity));
				}

				//generate powerPellet when in danger zone 
				//and multiplier > 6
				//the powerPellet has not to be given again in same multiplier level
				if (obstaclePatterns [currentPattern] [1, j] == 0 && KittyController.instance.inDanzerZone && !generatedPowerPellet) {
					pos = new Vector3 (mirrored * (j-1), transform.position.y, transform.position.z);
					generatedObstacles.Add ((GameObject)Instantiate (powerPellet, pos, Quaternion.identity));
					generatedObstacles.Add ((GameObject)Instantiate (powerPellet, pos, Quaternion.identity));
					generatedPowerPellet = true;
				}
				//generate a fish with a probability of 1 in 2 when a 0 is there
				else if (obstaclePatterns [currentPattern] [1, j] == 0 && temp == 1) {
					pos = new Vector3 (mirrored * (j-1), transform.position.y, transform.position.z);
					generatedObstacles.Add ((GameObject)Instantiate (fish, pos, Quaternion.identity));
				}
			}

			//generate an obstacle generator trigger here
			pos = new Vector3 (transform.position.x, transform.position.y, transform.position.z - rowDistance);
			generatedObstacles.Add ((GameObject)Instantiate (obstacleGeneratorTrigger, pos, Quaternion.identity));

			row = 3;
		}
		else if (row == 3) {
			for (int j = 0; j < 3; j++) {
				//generate some random obstacle for each 1 in the pattern
				if (obstaclePatterns [currentPattern] [2, j] == 1) {
					pos = new Vector3 (mirrored * (j-1), transform.position.y, transform.position.z);
					generatedObstacles.Add ((GameObject)Instantiate (obstacleList [Random.Range (0, obstacleList.Length)], pos, Quaternion.identity));
				}


				//generate powerPellet when in danger zone 
				//and multiplier > 6
				//the powerPellet has not to be given again in same multiplier level
				if (obstaclePatterns [currentPattern] [2, j] == 0 && KittyController.instance.inDanzerZone && !generatedPowerPellet) {
					pos = new Vector3 (mirrored * (j-1), transform.position.y, transform.position.z);
					generatedObstacles.Add ((GameObject)Instantiate (powerPellet, pos, Quaternion.identity));
					generatedObstacles.Add ((GameObject)Instantiate (powerPellet, pos, Quaternion.identity));
					generatedPowerPellet = true;
				}

				//generate a fish with a probability of 1 in 2 when a 0 is there
				else if (obstaclePatterns [currentPattern] [2, j] == 0 && temp == 1) {
					pos = new Vector3 (mirrored * (j-1), transform.position.y, transform.position.z);
					generatedObstacles.Add ((GameObject)Instantiate (fish, pos, Quaternion.identity));
				}

			}

			//generate an obstacle group generator trigger here
			pos = new Vector3 (transform.position.x, transform.position.y, transform.position.z - rowDistance);
			generatedObstacles.Add ((GameObject)Instantiate (obstacleGroupGeneratorTrigger, pos, Quaternion.identity));

			row = 1;
		}
	}

	public void generateObstacleGroup(){
		//Debug.Log ("Obstacle Generator generateObstacleGroup");
		temp = Random.Range (1, 3);
		currentPattern = Random.Range (1, 9);
		//Sets mirrored var to 1 or -1
		if (Random.Range (0, 2) == 0) {
			mirrored = 1;
		} 
		else {
			mirrored = -1;	
		}

		//generate first row obstacles in this pattern
		for (int j = 0; j < 3; j++) {
			if (obstaclePatterns [currentPattern] [0, j] == 1) {
				pos = new Vector3 (mirrored * (j-1), transform.position.y, transform.position.z);
				generatedObstacles.Add ((GameObject)Instantiate (obstacleList [Random.Range (0, obstacleList.Length)], pos, Quaternion.identity));
			}
		}
		row = 2;

		//generate an obstacle generator trigger here
		pos = new Vector3 (transform.position.x, transform.position.y, transform.position.z - rowDistance);
		generatedObstacles.Add ((GameObject)Instantiate (obstacleGeneratorTrigger, pos, Quaternion.identity));
	}
}
