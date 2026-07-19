using UnityEngine;
using UnityEngine.AI;

public class NavMeshPlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float navMeshCheckDistance = 1f;

    private void Update()
    {
        Vector3 inputDirection = new Vector3(
            Input.GetAxisRaw("Horizontal"),
            0f,
            Input.GetAxisRaw("Vertical")
        ).normalized;

        if (inputDirection == Vector3.zero)
        {
            return;
        }

        Vector3 desiredMove = inputDirection * moveSpeed * Time.deltaTime;
        Vector3 desiredPosition = transform.position + desiredMove;

        if (NavMesh.SamplePosition(desiredPosition, out NavMeshHit hit, navMeshCheckDistance, NavMesh.AllAreas))
        {
            transform.position = hit.position;

            if (inputDirection != Vector3.zero)
            {
                transform.forward = inputDirection;
            }
        }
    }
}