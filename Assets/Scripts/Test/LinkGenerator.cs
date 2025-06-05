using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LinkGenerator : MonoBehaviour
{
    List<Vector3> edgePoints = new List<Vector3>();
    void Start()
    {
        GenerateLinks();
    }

    void GenerateLinks()
    {
        NavMeshTriangulation navMeshData = NavMesh.CalculateTriangulation();

        // Extract all edges from navmesh
        for (int i = 0; i < navMeshData.indices.Length; i += 3)
        {
            Vector3 v0 = navMeshData.vertices[navMeshData.indices[i]];
            Vector3 v1 = navMeshData.vertices[navMeshData.indices[i + 1]];
            Vector3 v2 = navMeshData.vertices[navMeshData.indices[i + 2]];

            edgePoints.Add(v0);
            edgePoints.Add(v1);
            edgePoints.Add(v1);
            edgePoints.Add(v2);
            edgePoints.Add(v2);
            edgePoints.Add(v0);
        }

        for (int i = 0; i < edgePoints.Count; i += 2)
        {
            Vector3 start = edgePoints[i];
            Vector3 end = edgePoints[i + 1];
            Vector3 mid = (start + end) * 0.5f;

            // Check if there's another surface nearby
            if (Physics.Raycast(mid, -Vector3.up, out RaycastHit hit, 100))
            {
                //CreateLink(mid, hit.point, hit.normal);
            }
        }
    }

    void CreateLink(Vector3 start, Vector3 end, Vector3 normal)
    {
        GameObject linkObj = new GameObject("AutoOffMeshLink");
        OffMeshLink link = linkObj.AddComponent<OffMeshLink>();
        link.startTransform = CreateTransform("Start", start);
        link.endTransform = CreateTransform("End", end);
        link.costOverride = -1;
        link.biDirectional = true;
        link.activated = true;
    }

    Transform CreateTransform(string name, Vector3 position)
    {
        GameObject go = new GameObject(name);
        go.transform.position = position;
        return go.transform;
    }

    private void OnDrawGizmos()
    {
        for (int i = 0; i < edgePoints.Count; i += 2)
        {
            Vector3 start = edgePoints[i];
            Vector3 end = edgePoints[i + 1];
            Vector3 mid = (start + end) * 0.5f;
            Gizmos.DrawLine(start, end);
            // Check if there's another surface nearby
            if (Physics.Raycast(mid, -Vector3.up, out RaycastHit hit, 100))
            {
                //CreateLink(mid, hit.point, hit.normal);
            }
        }
    }
}
