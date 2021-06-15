using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyType1 : MonoBehaviour, Enemyinterface
{
    public GameObject target;
    public TimeManager timeManager;

    NavMeshAgent agent;
    Rigidbody rb;

    bool alive;
    bool? active;

    static int weight = 8;

    bool slowOnDeath;

    Vector3[] corners;

    // Start is called before the first frame update
    void Start()
    {
        //default target is itself
        if (target == null)
        {
            target = gameObject;
        }

        agent = gameObject.GetComponent<NavMeshAgent>();
        rb = gameObject.GetComponent<Rigidbody>();
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
        slowOnDeath = false;

        //find corners and set vectors
        Transform[] cornerTrans;
        cornerTrans = GameObject.Find("Corners").GetComponentsInChildren<Transform>();

        corners = new Vector3[cornerTrans.Length];
        for (int i = 0; i < corners.Length; i++)
        {
            corners[i] = cornerTrans[i].position;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //opposite position from player
        updateAgentDestination();

        //update speed vars based on timemanager
        updateAgentTime();

        //enable or disable
        if (active == false)
        {
            agent.velocity = Vector3.zero;
        }
        else
        {
            
        }
    }

    void updateAgentDestination()
    {
        //follow player method
        //agent.destination = -1 * (target.transform.position - agent.transform.position) + agent.transform.position;

        //run away from player
        //Find the corner furthest from the player but where the enemy is still closer to the corner
        Vector3 chosenCorner = corners[0];
        for (int i = 0; i < corners.Length; i++)
        {
            if (Vector3.Distance(corners[i], target.transform.position) > Vector3.Distance(chosenCorner, target.transform.position) &&
                Vector3.Distance(corners[i], agent.transform.position) <= Vector3.Distance(corners[i], target.transform.position))
            {
                chosenCorner = corners[i];
            }
        }

        //set agent destination
        agent.destination = chosenCorner;
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
