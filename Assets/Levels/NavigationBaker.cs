using Unity.AI.Navigation;
using UnityEngine;

public class NavigationBaker : MonoBehaviour
{
    [SerializeField] private Transform breakableGameObjectTF;

    private NavMeshSurface[] surfaces;
    private BreakableObject[] breakableObjects;

    private void Awake()
    {
        breakableObjects = breakableGameObjectTF.GetComponentsInChildren<BreakableObject>();
        surfaces = GetComponents<NavMeshSurface>();
    }

    private void Start()
    {
        UpdateNavMesh();

        foreach (BreakableObject breakableObject in breakableObjects)
            breakableObject.DestroyBreakableObject += BreakableObjectDestroyed;
    }

    private void UpdateNavMesh()
    {
        for (int index = 0; index < surfaces.Length; index++)
        {
            NavMeshSurface navMesh = surfaces[index];
            navMesh.UpdateNavMesh(navMesh.navMeshData);
        }
    }

    private void BreakableObjectDestroyed(BreakableObject breakableObject)
    {
        UpdateNavMesh();
    }
}
