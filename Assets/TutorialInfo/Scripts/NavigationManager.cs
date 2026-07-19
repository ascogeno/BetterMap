using UnityEngine;
using UnityEngine.AI;

public class NavigationManager : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Transform destination;

    private NavMeshPath path;

    private void Awake()
    {
        path = new NavMeshPath();
    }

    private void Update()
    {
        if (player == null || destination == null)
            return;

        NavMesh.CalculatePath(
            player.position,
            destination.position,
            NavMesh.AllAreas,
            path);

        for (int i = 0; i < path.corners.Length - 1; i++)
        {
            Debug.DrawLine(
                path.corners[i],
                path.corners[i + 1],
                Color.green);
        }
    }

    public NavMeshPath GetCurrentPath()
    {
        return path;
    }

    public void SetDestination(Transform newDestination)
    {
        destination = newDestination;
    }
}