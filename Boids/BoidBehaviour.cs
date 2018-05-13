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

    
    //data that boids share with each other
    public float detectionRadius = 1f;
    public Quaternion currentRotation;
    public Vector2 currentPosition;
    public float boidSpeed = 25f;
    public Vector3 scrambleVector;
    public LayerMask layerMask;

    //Do not touch
    private List<GameObject> boidsInRange = new List<GameObject>(); //all boids within range as gameobject
    private Vector2 averagePosition; //vector2 for storing average position
    private bool isAlone; //flag for state
    private bool isDoneRotating = true; //flag for rotation
    private bool isDoneScrambling = true; //flag for scrambling     
    private Vector3 targetVector; 
    private bool mouseHeldDown = false;


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
    private void CheckBorder() //checks if boids don't leave a predefined area
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
        Vector3 avgPos2 = new Vector3(0, 0, 0);
        Vector3 avgRotation = new Vector3(0, 0, 0);

        //average Wposition of all boids in range
        for (int i = 0; i < boidsInRange.Count; i++)
        {
            avgSpeed += boidsInRange[i].gameObject.GetComponent<BoidBehaviour>().boidSpeed;
            avgPos2 += boidsInRange[i].transform.position;
            avgRotation += boidsInRange[i].transform.rotation.eulerAngles;
        }

        //calculate all averages of this flock to determine next action
        avgSpeed /= boidsInRange.Count;
        avgPos2 /= boidsInRange.Count;
        avgRotation /= boidsInRange.Count;
        
        for (int i = 0; i < boidsInRange.Count; i++)
        {
            //check if boid is close to other boid
            if(Vector2.Distance(transform.position,VectorConvert(boidsInRange[i].transform.position)) > minDistance)
            { 
                BoidSpeedControl(avgSpeed);

                //if we hold mouse, they follow it 
                if(mouseHeldDown)
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
                MoveAway(avgPos2);
            }
        }
    }

    private void BoidSpeedControl(float avgspeed) //makes boids adjust to average speed
    {
      boidSpeed = Mathf.Lerp(boidSpeed, avgspeed, 0.4f);
    }

    private void MoveAway(Vector3 avgPos2) //makes boids disperse 
    {
        Vector3 distVector;
        distVector = avgPos2 - gameObject.transform.position;
        transform.position += -new Vector3(distVector.x, distVector.y * 0.2f, transform.position.z) * Time.deltaTime * boidSpeed * seperationSpeed;
    }

    private void FollowMouse() //function that handles boid-to-mouse movement
    {
        Vector3 dir = Input.mousePosition - Camera.main.WorldToScreenPoint(transform.position);
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        Quaternion quat = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.Lerp(transform.rotation, quat, 0.05f);
    }

    private Vector2 VectorConvert(Vector3 vectorToConvert) //converts vector2 to vector3
    {
        Vector2 vectorToReturn = new Vector2(vectorToConvert.x, vectorToConvert.y);
        return vectorToReturn;
    }


    void CheckForBoids() //detects boids around this boid
    {
        boidsInRange.Clear();

        Vector3 origin = transform.position + new Vector3(0, 1, 0);  
        Vector3 direction = Vector3.up;
        Collider2D[] hit;

        hit = (Physics2D.OverlapCircleAll(origin, detectionRadius, layerMask, 0, 0));
        for (int i = 0; i < hit.Length; i++)
        {
          if(hit[i].gameObject != gameObject)
            {    
                boidsInRange.Add(hit[i].gameObject);
            }
        }
       
    }

    private void CheckForMouse() //checks if mouse is pressed
    {
        if (Input.GetMouseButton(0))
        {
            mouseHeldDown = true;
        }
        else
        {
            mouseHeldDown = false;
        }
    }







}

