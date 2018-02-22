using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidCreator : MonoBehaviour {

    private static float MAX_X = 60;
    private static float MAX_Y = 30;
    private static float MIN_X = -60;
    private static float MIN_Y = -30;

    public GameObject BoidObject;
    public float BoidAmount;
	// Use this for initialization
	void Start () {
		
        for (int i = 0; i < BoidAmount;i++)
        {
            GameObject boidOBj = Instantiate(BoidObject) as GameObject;
            boidOBj.transform.position = new Vector2(Random.Range(MIN_X, MAX_X), Random.Range(MIN_Y, MAX_Y));
        }
	}
	

}
