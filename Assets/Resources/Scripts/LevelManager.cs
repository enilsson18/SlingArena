using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LevelManager : MonoBehaviour
{
    //static vars
    static GameObject DeathParticles;
    public static List<GameObject> deathParticles;

    //map info
    GameObject exit;
    GameObject entranceDoor;
    GameObject exitDoor;

    ProgressBar progressBar;
    public int progressBarScale;

    TimeManager timeManager;

    //player info
    public Player player;
    public GameObject spawn;

    //enemy info
    public GameObject[] spawnPoints;
    public GameObject[] EnemyTypes;
    public List<GameObject> enemies;

    private List<int> enemyWeights;

    public int difficulty;
    public float killTimeSlowDuration = 1;
    public float killTimeSlow = 0.1f;

    float oldTimeScale;
    float killSlowTimer;

    //status
    public int stage;
    bool timeSlow;
    bool active;

    //timer stuff
    float timer;
    float startTime = 10;
    public TextMeshPro timerMesh;


    // Start is called before the first frame update
    public void Start()
    {
        //static setup
        DeathParticles = Resources.Load<GameObject>("Prefabs/Death Particles");

        //setup player
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        //setup enemy
        //load all the enemies prefabs from resource folder in assets
        EnemyTypes = Resources.LoadAll<GameObject>("Prefabs/Enemies");

        //set all the enemy weights see createEnemyMethod for explaination
        enemyWeights = new List<int>();
        int count = 0; 
        foreach (GameObject gameObj in EnemyTypes)
        {
            enemyWeights.Add(count);
            count += gameObj.GetComponent<Enemyinterface>().getWeight();
        }


        //init timemanager
        timeManager = GameObject.Find("GameManager").GetComponent<TimeManager>();

        //init exit
        exit = GameObject.Find("Exit");

        //setup doors
        entranceDoor = GameObject.Find("Entrance Door");
        exitDoor = GameObject.Find("Exit Door");

        entranceDoor.transform.position += new Vector3(1, 0, 0);
        exitDoor.transform.position += new Vector3(1, 0, 0);

        //waiting stage for the level to start
        stage = -3;
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        //check if the player is dead or not
        if (player.isAlive == false && stage >= 0)
        {
            //destroy leftover enemies
            destroyAllEnemies();
            stage = -1;
        }
        //pause menu
        else if (stage == -2)
        {

        }
        //setup animation phase
        else if (stage == 0)
        {
            this.GetComponent<Animator>().SetTrigger("entrance");
            stage = 1;
        }
        //check start animation phase
        else if (stage == 1)
        {
            checkStartAnimationSequence();
        }
        //main game phase
        else if (stage == 2)
        {

            updateTimer(Time.fixedDeltaTime);

            calculateTimeScale();

            //if all enemies are dead then open the door and check for the player to leave
            if (enemies.Count == 0)
            {
                stage = 3;

                //open gate
                this.GetComponent<Animator>().SetTrigger("exit");
            }
            else
            {
                //check the enemies and despawn them if they die
                checkEnemyDeath();
            }
        }
        //all enemies are dead and get out
        if (stage == 3)
        {
            updateTimer(Time.fixedDeltaTime);

            calculateTimeScale();

            StartCoroutine(resetTrigger("exit"));

            //if collide with the exit
            if (GameObject.Find("Exit").GetComponent<BasicCollider>().colPlayer)
            {
                //destroy leftover enemies
                destroyAllEnemies();

                stage = 4;
            }
        }
    }

    //the strt
    public void startLevel()
    {
        stage = 0;
        timer = startTime;
        timerMesh.text = getTimer().ToString("#0.00");

        //reset player pos and velocity
        spawn = GameObject.FindGameObjectWithTag("spawn");
        player.transform.position = spawn.transform.position;
        player.gameObject.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);

        //setup progressbar
        progressBar = GameObject.FindGameObjectWithTag("progress_bar").GetComponent<ProgressBar>();
        progressBar.scale = progressBarScale;

        //make difficulty text
        TextManager tmp = GameObject.Find("Difficulty").GetComponent<TextManager>();
        tmp.SetText(difficulty.ToString());
        tmp.FadeIn();
    }

    //setup function
    public void setup(int difficulty)
    {
        this.difficulty = difficulty;

        player.isAlive = true;

        exit.GetComponent<BasicCollider>().Start();
        //entrance.GetComponent<BasicCollider>().Start();

        //make default list for enemies on screen
        enemies = new List<GameObject>();

        //default list of particles
        deathParticles = new List<GameObject>();

        //find enemy spawns
        spawnPoints = GameObject.FindGameObjectsWithTag("enemy_spawn");

        //init default varibles for time slow mechanics
        timeSlow = false;
        killSlowTimer = 0;
        timer = startTime;
        timerMesh.text = getTimer().ToString("#0.00");
        timeManager.setTimeScale(1, 0.05f);

        //player setup
        player.controllable = false;

        //make enemies
        createEnemies();
    }

    //if you run out of time in a game
    void timerExpired()
    {
        player.controllable = false;
        destroyAllEnemies();
        stage = -1;
    }

    void calculateTimeScale()
    {
        //WORK IN PROGRESS
        //check for the player firing to cancel slowmo
        //also change the slowmo to gradual increase
        if (timeSlow)
        {
            if (timeManager.timeScale == killTimeSlow)
            {
                timeManager.setTimeScale(oldTimeScale, killTimeSlowDuration);
            }
        }
    }

    //the setup animation phase of the game
    void checkStartAnimationSequence()
    {
        StartCoroutine(resetTrigger("entrance"));

        //if the animation is done
        if (this.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            stage = 2;
            player.controllable = true;

            for (int i = 0; i < enemies.Count; i++)
            {
                enemies[i].GetComponent<Enemyinterface>().SetActivity(true);
                enemies[i].GetComponent<Enemyinterface>().SetTarget(player.gameObject);
            }

        }
    }

    //this function determines what type of enemy to make and how many of them to make.
    //basic difficulty algorithm
    //every multiple of 5, the enemy types get harder
    //every single difficulty you go up, an enemy is added
    void createEnemies()
    {
        for (int i = 0; i < 1; i++)
        {
            //spawnEnemy(EnemyTypes[4]);
        }

        //normal progression
        //under the assumption that difficulty starts as 1
        //will add the corresponding number of enemies of the correct types based off of their weights (the recommended number of the enemy type for normal progression)

        progressBarScale = 0;
        for (int i = enemyWeights.Count-1; i >= 0; i--)
        {
            if (difficulty > enemyWeights[i])
            {
                for (int x = 0; x < difficulty - enemyWeights[i]; x++)
                {
                    spawnEnemy(EnemyTypes[i]);
                    //increment progress bar baseline to see how many particles are needed to advance
                    progressBarScale += DeathParticles.GetComponent<ParticleSystem>().maxParticles;
                }
                break;
            }
        }

        //basic view all
        /*
        foreach (GameObject enemyType in EnemyTypes)
        {
            for (int i = 0; i < difficulty; i++)
            {
                spawnEnemy(enemyType);
            }
        }
        */
    }

    void spawnEnemy(GameObject enemyType)
    {
        //adds an enemy in the next spawn position found
        if (enemies.Count < spawnPoints.Length)
        {
            //adds new enemy and sets the position, target, time manager, and activity for movement so it remains still
            enemies.Add(null);
            enemies[enemies.Count-1] = Instantiate(enemyType) as GameObject;
            enemies[enemies.Count - 1].SetActive(false);
            enemies[enemies.Count - 1].transform.position = spawnPoints[enemies.Count - 1].transform.position;
            enemies[enemies.Count - 1].SetActive(true);
            enemies[enemies.Count - 1].GetComponent<Enemyinterface>().SetTarget(player.gameObject);
            enemies[enemies.Count - 1].GetComponent<Enemyinterface>().SetTimeManager(timeManager);
            enemies[enemies.Count - 1].GetComponent<Enemyinterface>().SetActivity(false);
        }
    }

    void checkEnemyDeath()
    {
        for (int i = 0; i < enemies.Count; i++)
        {
            //if they are dead
            if (!enemies[i].GetComponent<Enemyinterface>().isAlive())
            {
                //temp is if they are using slow down
                bool temp;
                temp = enemies[i].GetComponent<Enemyinterface>().slowTimeOnDeath();

                Destroy(enemies[i]);
                enemies.RemoveAt(i);

                if (temp)
                {
                    //start timeSlow
                    print("slowdown");
                    //if already slowing time then don't reset the point to return to
                    if (!timeSlow)
                    {
                        oldTimeScale = timeManager.timeScale;
                    }
                    timeSlow = true;
                    timeManager.setTimeScale(killTimeSlow, 0.05f);
                }
            }
        }
    }

    //destroys all enemies and fragments of enemies on the current level
    void destroyAllEnemies()
    {
        for (int i = 0; i < enemies.Count; i++)
        {
            Destroy(enemies[i]);
        }

        //destroy death particles too
        for (int i = 0; i < deathParticles.Count; i++)
        {
            Destroy(deathParticles[i]);
        }
    }

    //spawn particles
    public static void spawnDeathParticles(Transform t)
    {
        deathParticles.Add(null);
        deathParticles[deathParticles.Count - 1] = Instantiate(DeathParticles) as GameObject;
        deathParticles[deathParticles.Count - 1].transform.position = t.position;
        deathParticles[deathParticles.Count - 1].GetComponent<ParticleAttraction>().Target = GameObject.FindGameObjectWithTag("progress_bar").transform;
    }

    //utility
    //timer
    public void updateTimer(float deltaTime)
    {
        //update time
        timer -= deltaTime * timeManager.timeScale;

        //timer = 0 condition
        if (timer <= 0)
        {
            timerExpired();
            timer = 0;
        }

        //update the mesh attatched and format it so that it is always rounded to the hundreds decimal place
        timerMesh.text = getTimer().ToString("#0.00");
    }

    public float getTimer()
    {
        return (float)System.Math.Round(timer * 100f) / 100f;
    }

    //wait one frame and then reset trigger for animations. this was to fix an issue where the animators were stoping themselves before they started.
    IEnumerator resetTrigger(string str)
    {
        yield return null;

        this.GetComponent<Animator>().ResetTrigger(str);
    }
}
