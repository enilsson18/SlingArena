﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyType6 : MonoBehaviour, Enemyinterface
{
    public GameObject target;
    public TimeManager timeManager;

    NavMeshAgent agent;

    bool alive;
    bool? active;

    static int weight = 6;

    bool slowOnDeath;

    //enemy vars
    public float angSpeed = 1.5f;
    float currentBladeRot;

    GameObject blade;

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

        currentBladeRot = 0;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        updateAgentDestination();

        //counter rotation
        blade.transform.rotation = Quaternion.Euler(0.0f, currentBladeRot, gameObject.transform.rotation.z * -1.0f);

        //rotate blade by speed
        currentBladeRot += angSpeed;
        currentBladeRot %= 360f;

        print(currentBladeRot);

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
