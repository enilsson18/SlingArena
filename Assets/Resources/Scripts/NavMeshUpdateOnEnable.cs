using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using NavMeshBuilder = UnityEngine.AI.NavMeshBuilder;

public class NavMeshUpdateOnEnable : MonoBehaviour
{

    public NavMeshData m_NavMeshData;
    private NavMeshDataInstance m_NavMeshInstance;

    void OnEnable()
    {
        m_NavMeshInstance = NavMesh.AddNavMeshData(m_NavMeshData);
    }

    void OnDisable()
    {
        NavMesh.RemoveNavMeshData(m_NavMeshInstance);
    }
}