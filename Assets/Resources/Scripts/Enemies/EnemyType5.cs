using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyType5 : MonoBehaviour, Enemyinterface
{
    public GameObject target;
    public TimeManager timeManager;

    NavMeshAgent agent;

    bool alive;
    bool? active;

    static int weight = 3;

    bool slowOnDeath;

    //enemy vars
    public float minSpeed = 0.02f;
    float initialAngularSpeed;

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

        initialAngularSpeed = gameObject.GetComponent<NavMeshAgent>().angularSpeed;
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

        //freeze if the player does not move
        if (target.GetComponent<Rigidbody>().velocity.sqrMagnitude <= minSpeed)
        {
            gameObject.GetComponent<NavMeshAgent>().velocity = Vector3.zero;
            gameObject.GetComponent<NavMeshAgent>().angularSpeed = 0;
        } else
        {
            gameObject.GetComponent<NavMeshAgent>().angularSpeed = initialAngularSpeed;
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
