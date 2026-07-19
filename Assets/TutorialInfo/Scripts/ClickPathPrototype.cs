using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ClickPathPrototype : MonoBehaviour
{
    [Header("Required References")]
    [SerializeField] private Transform startPoint;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private Camera mainCamera;

    [Header("Arrow Settings")]
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private float arrowSpacing = 2.0f;
    [SerializeField] private float arrowHeight = 0.12f;

    [Header("Line Settings")]
    [SerializeField] private float lineHeight = 0.08f;

    private NavMeshPath path;
    private readonly List<GameObject> arrows = new();

    private void Awake()
    {
        path = new NavMeshPath();

        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TryCreatePathFromClick();
        }
    }

    private void TryCreatePathFromClick()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (!Physics.Raycast(ray, out RaycastHit hit))
        {
            return;
        }

        Vector3 clickedPoint = hit.point;

        if (!NavMesh.SamplePosition(clickedPoint, out NavMeshHit navHit, 2.0f, NavMesh.AllAreas))
        {
            ClearPath();
            Debug.Log("Clicked point is not close enough to the NavMesh.");
            return;
        }

        bool pathFound = NavMesh.CalculatePath(
            startPoint.position,
            navHit.position,
            NavMesh.AllAreas,
            path
        );

        if (pathFound && path.status == NavMeshPathStatus.PathComplete)
        {
            DrawPath(path);
        }
        else
        {
            ClearPath();
            Debug.Log("No complete path found.");
        }
    }

    private void DrawPath(NavMeshPath navPath)
    {
        lineRenderer.positionCount = navPath.corners.Length;

        for (int i = 0; i < navPath.corners.Length; i++)
        {
            Vector3 raisedPoint = navPath.corners[i] + Vector3.up * lineHeight;
            lineRenderer.SetPosition(i, raisedPoint);
        }

        DrawArrows(navPath);
    }

    private void DrawArrows(NavMeshPath navPath)
    {
        ClearArrows();

        if (arrowPrefab == null)
        {
            Debug.LogWarning("Arrow prefab is not assigned.");
            return;
        }

        for (int i = 0; i < navPath.corners.Length - 1; i++)
        {
            Vector3 segmentStart = navPath.corners[i];
            Vector3 segmentEnd = navPath.corners[i + 1];

            Vector3 direction = segmentEnd - segmentStart;
            float distance = direction.magnitude;

            if (distance < 0.1f)
            {
                continue;
            }

            direction.Normalize();

            for (float d = arrowSpacing; d < distance; d += arrowSpacing)
            {
                Vector3 arrowPosition = segmentStart + direction * d;
                arrowPosition += Vector3.up * arrowHeight;

                Quaternion arrowRotation = Quaternion.LookRotation(direction, Vector3.up) * Quaternion.Euler(90f, 0f, 0f);

                GameObject arrow = Instantiate(arrowPrefab, arrowPosition, arrowRotation);
                arrows.Add(arrow);
            }
        }
    }

    private void ClearPath()
    {
        lineRenderer.positionCount = 0;
        ClearArrows();
    }

    private void ClearArrows()
    {
        foreach (GameObject arrow in arrows)
        {
            Destroy(arrow);
        }

        arrows.Clear();
    }
}