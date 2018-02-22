using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidBehaviour : MonoBehaviour {
    //map borders
    private static float MAX_X = 60;
    private static float MAX_Y = 30;
    private static float MIN_X = -60;
    private static float MIN_Y = -30;

    private float updateInterval = 0.2f; //update interval (unused)
    private float minDistance = 1f; //how close boids get
    private float seperationSpeed = 1f; //how quickly do they seperate
    private float steerIntensity = 360f; //how far can boids steer
    private float steerSpeed = 1.2f; //delay in steering (unused)
    private float groupDynamic = 0f; //determines individual offsets when a boid is flocking -> Higher = more dynamic

    
    //data that boids share with eachother
    public float detectionRadius = 1f;
    public Quaternion CurrentRotation;
    public Vector2 Currentposition;
    public float boidSpeed = 25f;

    //Do not touch
    private List<GameObject> boidsInRange = new List<GameObject>(); //all boids within range as gameobject
    private Vector2 averagePosition;
    private bool isAlone; //flag for state
    private bool isDoneRotating = true; //flag for rotation
    private bool isDoneScrambling = true;
    private Vector3 targetVector;
    public Vector3 scrambleVector;
    public LayerMask lm;
    private bool MouseHeldDown = false;


    private void Start()
    {
        InitBoid();
    }
    private void Update()
    {
        CheckForMouse();
        CheckForBoids();
        Thrust();
        CheckBorder();
        HandleStates();
        
    }
    private void HandleStates() //handle the states of the boids
    {
        if (boidsInRange.Count > 0)
        {
            isAlone = false;
            Cohesion();
        }
        else
        {
            isAlone = true;
            Alone();
        }
    }
    private void CheckBorder() //check if out of range
    {
       if(transform.position.x > MAX_X)
        {
            transform.position = new Vector2(MIN_X, transform.position.y);
        }
       if(transform.position.x < MIN_X)
        {
            transform.position = new Vector2(MAX_X, transform.position.y);
        }

       if(transform.position.y > MAX_Y)
        {
            transform.position = new Vector2(transform.position.x, MIN_Y);
        }
       if(transform.position.y < MIN_Y)
        {
            transform.position = new Vector2(transform.position.x, MAX_Y);
        }
    }
    private void InitBoid() //initialize boid
    {
        transform.Rotate(new Vector3(0, 0, Random.Range(0, 180)));
        boidSpeed = Random.Range(15f, 25f);
    }

    private void Thrust() //basic thrust force for every boid
    {
        gameObject.transform.Translate(Vector2.right * boidSpeed * Time.deltaTime);
    }

    private void Alone() //our boid has no other boids in range
    {
        if(isDoneRotating)
        {
            targetVector = new Vector3(0, 0, Random.Range(-steerIntensity, steerIntensity));
            isDoneRotating = false;
        }
        if (transform.rotation != Quaternion.Euler(targetVector))
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(targetVector), steerSpeed);
        }
        else
        {
            isDoneRotating = true;
        }   
    }

    private void Cohesion() //handles bird cohesion
    {
        float avgSpeed = 0;
        Vector3 avgpos2 = new Vector3(0, 0, 0);
        Vector3 avgRotation = new Vector3(0, 0, 0);

        //average Wposition of all boids in range
        for (int i = 0; i < boidsInRange.Count; i++)
        {
            avgSpeed += boidsInRange[i].gameObject.GetComponent<BoidBehaviour>().boidSpeed;
            avgpos2 += boidsInRange[i].transform.position;
            avgRotation += boidsInRange[i].transform.rotation.eulerAngles;
        }

        //calculate all averages of this flock
        avgSpeed /= boidsInRange.Count;
        avgpos2 /= boidsInRange.Count;
        avgRotation /= boidsInRange.Count;
        
        for (int i = 0; i < boidsInRange.Count; i++)
        {
            //check if boid is close to other boid
            if(Vector2.Distance(transform.position,VectorConvert(boidsInRange[i].transform.position)) > minDistance)
            { 
                BoidSpeedControl(avgSpeed);

                //if we hold mouse, they follow it 
                if(MouseHeldDown)
                { 
                    FollowMouse();
                }
                //if not, they use their group direction mechanism
                else
                { 
                    Vector3 offset = new Vector3(0, 0, Random.Range(-groupDynamic, groupDynamic));
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(avgRotation), 2f) * Quaternion.Euler(offset);              
                }
            }
            //we move away a little to not break formation
            else
            {
                MoveAway(avgpos2);
            }
        }
    }

    private void BoidSpeedControl(float avgspeed)
    {
      boidSpeed = Mathf.Lerp(boidSpeed, avgspeed, 0.4f);
    }

    private void MoveAway(Vector3 avgpos2)
    {
        Vector3 distVector;
        distVector = avgpos2 - gameObject.transform.position;
        transform.position += -new Vector3(distVector.x, distVector.y * 0.2f, transform.position.z) * Time.deltaTime * boidSpeed * seperationSpeed;
    }

    private void FollowMouse()
    {
        Vector3 dir = Input.mousePosition - Camera.main.WorldToScreenPoint(transform.position);
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        //transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        Quaternion quat = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.Lerp(transform.rotation, quat, 0.05f);
    }

    private Vector2 VectorConvert(Vector3 vectorToConvert) //converts vector2 to vector3
    {
        Vector2 vectorToReturn = new Vector2(vectorToConvert.x, vectorToConvert.y);
        return vectorToReturn;
    }
    void CheckForBoids()
    {
        boidsInRange.Clear();

        Vector3 origin = transform.position + new Vector3(0, 1, 0);  
        Vector3 direction = Vector3.up;
        //float radius = 1f;

        Collider2D[] hit;

        hit = (Physics2D.OverlapCircleAll(origin, detectionRadius, lm, 0, 0));
        for (int i = 0; i < hit.Length; i++)
        {
          if(hit[i].gameObject != gameObject)
            {    
                boidsInRange.Add(hit[i].gameObject);
            }
        }
       
    }
    private void CheckForMouse()
    {
        if (Input.GetMouseButton(0))
        {
            MouseHeldDown = true;
        }
        else
        {
            MouseHeldDown = false;
        }
    }







}

