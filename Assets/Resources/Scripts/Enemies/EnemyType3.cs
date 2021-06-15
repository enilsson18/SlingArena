using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyType3 : MonoBehaviour, Enemyinterface
{
    public GameObject target;
    public TimeManager timeManager;

    NavMeshAgent agent;

    bool alive;
    bool? active;

    static int weight = 4;

    bool slowOnDeath;

    public int dashSpeed = 9;
    public int dashDelay = 2;
    IEnumerator dashEnumerator;

    //character specific attack status things
    int attackStage;

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

        attackStage = 0;

        dashEnumerator = dash().GetEnumerator();
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

        dashEnumerator.MoveNext();

        //stop at edge
        NavMeshHit edge;
        agent.FindClosestEdge(out edge);

        if (edge.distance == 0)
        {
            agent.velocity = Vector3.zero;
        }
    }

    //dash coroutine
    IEnumerable dash()
    {
        while (true)
        {
            for (int i = 0; i < dashDelay / Time.fixedDeltaTime; i++)
            {
                yield return null;
            }

            agent.velocity += (agent.transform.rotation) * Vector3.forward * dashSpeed;
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
