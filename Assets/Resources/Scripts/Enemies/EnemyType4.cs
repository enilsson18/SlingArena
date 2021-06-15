using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyType4 : MonoBehaviour, Enemyinterface
{
    public GameObject target;
    public TimeManager timeManager;

    NavMeshAgent agent;

    bool alive;
    bool? active;

    static int weight = 6;

    bool slowOnDeath;

    int dashSpeed = 1;
    int dashDelay = 1;

    //enemy vars
    GameObject blade;
    float initialBladeScale;
    float bladeMaxDiameter = 1.5f;
    float bladeSpeed = 0.0004f;
    int bladeScalingDirection;

    // Start is called before the first frame update
    void Start()
    {
        //default target is itself
        if (target == null)
        {
            target = gameObject;
        }

        agent = transform.GetComponent<NavMeshAgent>();
        alive = true;

        if (!active.HasValue)
        {
            active = false;
        }

        //adjust speeds to match the change in fixed update
        agent.speed /= Time.fixedDeltaTime / 0.015f;
        agent.angularSpeed /= Time.fixedDeltaTime / 0.015f;
        agent.acceleration /= Time.fixedDeltaTime / 0.015f;

        //specific enemy info
        //agent.speed = 0;
        slowOnDeath = true;

        blade = transform.GetChild(0).gameObject;
        initialBladeScale = (blade.transform.localScale.x + blade.transform.localScale.z)/2;

        bladeScalingDirection = 1;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        updateAgentDestination();

        //update speed vars based on timemanager
        updateAgentTime();

        //enable or disable
        if (active == false)
        {
            gameObject.GetComponent<NavMeshAgent>().velocity = Vector3.zero;
        }
    }

    void updateAgentDestination()
    {
        agent.destination = target.transform.position;

        //update blade size
        blade.transform.localScale += new Vector3(1,0,1) * (bladeSpeed / Time.fixedDeltaTime) * bladeScalingDirection;
        print(new Vector3(1, 0, 1) * (bladeSpeed / Time.fixedDeltaTime) * bladeScalingDirection);

        //flip scaling direction if it passes bounds and move it in opposite direction so it does not exceed limit
        if (blade.transform.localScale.x > bladeMaxDiameter || blade.transform.localScale.x < initialBladeScale)
        {
            bladeScalingDirection *= -1;
            blade.transform.localScale += new Vector3(1, 0, 1) * (bladeSpeed / Time.fixedDeltaTime) * bladeScalingDirection;
        }

        //if the blade is within the body then deactivate it so the collider does not mess up
        if (blade.transform.localScale.x < 1)
        {
            blade.SetActive(false);
        } else
        {
            blade.SetActive(true);
        }
    }

    void updateAgentTime()
    {
        float cvt = timeManager.cvtTime(1f);
        agent.speed *= cvt;
        if (cvt != 1)
        {
            agent.velocity *= cvt;
        }
        agent.acceleration *= cvt;
        agent.angularSpeed *= cvt;
    }

    //collider
    //collision
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            alive = false;
            this.gameObject.SetActive(false);
            LevelManager.spawnDeathParticles(this.transform);
        }
    }

    //trigger method
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            alive = false;
            this.gameObject.SetActive(false);
            LevelManager.spawnDeathParticles(this.transform);
        }
    }

    //control methods made by interface
    public void SetTarget(GameObject target)
    {
        this.target = target;
    }

    public void SetTimeManager(TimeManager timeManager)
    {
        this.timeManager = timeManager;
    }

    public void SetActivity(bool activity)
    {
        active = activity;
    }

    public bool isAlive()
    {
        return alive;
    }

    public bool slowTimeOnDeath()
    {
        return slowOnDeath;
    }

    public int getWeight()
    {
        return weight;
    }
}
