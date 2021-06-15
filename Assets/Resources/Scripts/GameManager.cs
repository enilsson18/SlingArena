using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TMPro;

public class GameManager : MonoBehaviour
{
    //callback definitions
    public delegate void ExpiredTimerCallback();

    //normal consistant variables
    TimeManager timeManager;
    LocalNavMeshBuilder navMesh;

    //settings
    float transitionSpeed = 0.3f;
    public int LevelCount = 10;

    //status variables
    public int stage;
    public int difficulty;

    //player info
    public Player player;

    //level info
    public GameObject[] levelTypes;
    public LevelManager levelManager;

    GameObject background;
    GameObject camera;

    // Start is called before the first frame update
    void Start()
    {
        //init varibales for main stuff
        camera = GameObject.FindGameObjectWithTag("MainCamera");
        background = GameObject.Find("FullBackground");

        timeManager = this.GetComponent<TimeManager>();
        player.timeManager = this.GetComponent<TimeManager>();

        //default startup stage
        stage = -1;
        difficulty = 1;

        //init levels
        levelTypes = Resources.LoadAll<GameObject>("Prefabs/Levels");

        createLevel();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //init enter level
        if (stage == -1)
        {
            //first init
            initLevel(difficulty, false);
            stage = 0;
        }
        if (stage == 0)
        {
            //start when entrance animation of level is finished
            if (checkEntranceAnimation())
            {
                stage = 1;
                //start level
                levelManager.startLevel();
            }
        }
        //main loop
        else if (stage == 1)
        {
            //goal
            if (levelManager.stage == 4)
            {
                //increment difficulty
                difficulty += 1;

                stage = 2;

                //shift the player inbetween levels
                player.gameObject.GetComponent<Rigidbody>().velocity = player.gameObject.GetComponent<Rigidbody>().velocity.normalized * (transitionSpeed);
            }
            //dies
            if (levelManager.stage == -1)
            {
                //reset level after frame delay because unity sucks and freeze player motion again
                createLevel();
                initLevel(difficulty, true);
                StartCoroutine(delayStage(0));
                //player.controllable = false;
                
            }
        }
        //exit level
        else if (stage == 2)
        {
            //when exit animation is finished then reset
            if (checkExitAnimation())
            {
                createLevel();
                initLevel(difficulty, false);
                StartCoroutine(delayStage(0));
            }
        }
    }

    void createLevel()
    {
        if (levelManager != null)
        {
            Destroy(levelManager.gameObject);
        }

        //choose random map template
        System.Random rand = new System.Random();

        levelManager = Instantiate(levelTypes[rand.Next(levelTypes.Length)], new Vector3(0, 0, 0), new Quaternion()).GetComponent<LevelManager>();
        NavMeshSurface nm = GameObject.FindObjectOfType<NavMeshSurface>();
        nm.BuildNavMesh();
        //levelManager.gameObject.transform.position += new Vector3(0, 0, difficulty * 12);
    }

    void initLevel(int difficulty, bool death)
    { 
        levelManager.Start();
        levelManager.setup(difficulty);
        //levelManager.gameObject.transform.position = new Vector3(0, 0, 0);

        if (death)
        {
            camera.transform.position = new Vector3(0, 10, 0);
            StartCoroutine(ResetTrailRenderer(player.gameObject.GetComponent<TrailRenderer>()));
            player.transform.position = GameObject.FindGameObjectWithTag("spawn").transform.position;
        }
        else
        {
            camera.transform.position = new Vector3(0, 10, -12);

            //player.gameObject.GetComponent<TrailRenderer>().enabled = false;
            StartCoroutine(ResetTrailRenderer(player.gameObject.GetComponent<TrailRenderer>()));
            player.transform.position -= new Vector3(0, 0, 24);
            //player.gameObject.GetComponent<TrailRenderer>().enabled = true;
        }
    }

    bool checkEntranceAnimation()
    {
        //return true;

        //camera movement transition

        Vector3 target = new Vector3(0, 10, 0);

        if (camera.transform.position == target)
        //if (player.transform.position == GameObject.FindGameObjectWithTag("spawn").transform.position)
        {
            return true;
        }
        else
        {
            player.gameObject.transform.position = Vector3.MoveTowards(player.gameObject.transform.position, GameObject.FindGameObjectWithTag("spawn").transform.position, transitionSpeed);
            camera.transform.position = Vector3.MoveTowards(camera.transform.position, target, transitionSpeed);
            //background.transform.position = new Vector3(camera.transform.position.x, background.transform.position.y, camera.transform.position.z);
        }

        return false;
        
        //level movement transition
        /*
        if (levelManager.gameObject.transform.position.z <= 0)
        {
            //levelManager.Start();
            levelManager.gameObject.transform.position = new Vector3(0,0,0);
            levelManager.updateStartPositions();
            //levelManager.setup(difficulty);

            return true;
        }
        else
        {
            levelManager.gameObject.transform.position -= new Vector3(0, 0, 0.01f / Time.fixedDeltaTime);
            levelManager.updateStartPositions();
        }
        return false;
        */
    }

    bool checkExitAnimation()
    {
        //return true;

        //camera movement transition

        Vector3 target = new Vector3(0, 10, 12);

        if (camera.transform.position == target)
        {
            return true;
        }
        else
        {
            player.gameObject.transform.position = Vector3.MoveTowards(player.gameObject.transform.position, new Vector3(0,0,24), transitionSpeed/2);
            camera.transform.position = Vector3.MoveTowards(camera.transform.position, target, transitionSpeed);
            //background.transform.position = new Vector3(camera.transform.position.x, background.transform.position.y, camera.transform.position.z);
        }

        return false;

        /*
        if (levelManager.gameObject.transform.position.z <= -12)
        {
            levelManager.gameObject.transform.position = new Vector3(0, 0, 0);
            levelManager.updateStartPositions();
            //levelManager.setup(difficulty);

            return true;
        }
        else
        {
            levelManager.gameObject.transform.position -= new Vector3(0, 0, 0.01f / Time.fixedDeltaTime);
            levelManager.updateStartPositions();
        }
        return false;
        */
    }

    IEnumerator ResetTrailRenderer(TrailRenderer tr)
    {
        float trailTime = tr.time;
        tr.time = 0;
        yield return null;
        yield return null;
        tr.time = trailTime;
    }

    IEnumerator delayStage(int stg)
    {
        yield return null;
        stage = stg;
    }
}
