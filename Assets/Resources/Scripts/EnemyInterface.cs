using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public interface Enemyinterface
{
    void SetTarget(GameObject target);
    void SetTimeManager(TimeManager timeManager);
    void SetActivity(bool activity);
    bool isAlive();
    bool slowTimeOnDeath();

    //the max number of enemies recommended per stage
    int getWeight();
}
