using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class NavigationBaker : MonoBehaviour
{
    // [SerializeField] private float updateRate = 0.1f;
    [SerializeField] private Transform breakableGameObjectTF;

    private NavMeshSurface[] surfaces;
    [SerializeField] private BreakableObject[] breakableObjects;

    private void Awake()
    {
        breakableObjects = breakableGameObjectTF.GetComponentsInChildren<BreakableObject>();
        surfaces = GetComponents<NavMeshSurface>();
    }

    private void Start()
    {
        foreach (BreakableObject breakableObject in breakableObjects)
        {
            breakableObject.OnDestroy += BreakableObjectDestroyed;
        }
    }

    private IEnumerator RebakeNavMesh()
    {
        for (int index = 0; index < surfaces.Length; index++)
        {
            surfaces[index].BuildNavMesh();
        }
        yield return null;
    }

    private void BreakableObjectDestroyed(BreakableObject breakableObject)
    {
        StartCoroutine(RebakeNavMesh());
    }
}
