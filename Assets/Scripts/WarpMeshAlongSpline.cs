using System.Collections.Generic;
using UnityEngine;
using Handout;
using System;
using System.Reflection;

public class WarpMeshAlongSpline : MeshCreator
{
    public Mesh InputMesh;
    public Vector3 MeshOrigin;
    public float MeshScale;
    public Vector2 TextureScale;
    public bool ComputeUVs;
    public bool ModifySharedMesh;

    public override void RecalculateMesh()
    {
        Curve curve = GetComponent<Curve>();
        if (curve == null)
            return;
        List<Vector3> points = curve.points;

        Debug.Log("Recalculating spline mesh");

        MeshBuilder builder = new MeshBuilder();
        if (points.Count < 2)
        {
            GetComponent<MeshFilter>().mesh = builder.CreateMesh(true);
            return;
        }

        Bounds bounds = InputMesh.bounds;
        Vector3 max = bounds.max;
        Vector3 min = bounds.min;

        var pointOrientations = new List<Quaternion>(points.Count);

        // Handle the special case for the first point
        if (points.Count > 1)
        {
            Vector3 firstSegmentDirection = (points[1] - points[0]).normalized;
            pointOrientations.Add(Quaternion.LookRotation(firstSegmentDirection, Vector3.up));
        }

        // Finding the average direction
        for (int i = 1; i < points.Count - 1; i++)
        {
            Vector3 firstSegment = (points[i] - points[i - 1]).normalized;
            Vector3 secondSegment = (points[i + 1] - points[i]).normalized;
            Vector3 averageDirection = ((firstSegment + secondSegment) / 2).normalized;

            // Add the averaged orientation to the list
            pointOrientations.Add(Quaternion.LookRotation(averageDirection, Vector3.up));
        }

        // Handle the special case for the last point
        if (points.Count > 2)
        {
            Vector3 lastSegmentDirection = (points[points.Count - 1] - points[points.Count - 2]).normalized;
            pointOrientations.Add(Quaternion.LookRotation(lastSegmentDirection, Vector3.up));
        }

        // Loop over all line segments in the curve
        for (int i = 0; i < points.Count - 1; i++)
        {
            int numVerts = InputMesh.vertexCount;
            for (int j = 0; j < InputMesh.vertexCount; j++)
            {
                // Map z coordinate to a number t from 0 to 1 (assuming the mesh bounds are correct):
                float t = (InputMesh.vertices[j].z - min.z) / (max.z - min.z);

                // Center and scale the input mesh vertices, using the values given in the inspector:
                Vector3 inputV = (InputMesh.vertices[j] - MeshOrigin) * MeshScale;
                // Set the z-coordinate to zero:
                inputV.Scale(new Vector3(1, 1, 0));

                Vector3 interpolatedLineSegmentPoint = Vector3.Lerp(points[i], points[i + 1], t);

                // Interpolate the orientations as well, not just the points!
                Quaternion interpolatedOrientation = Quaternion.Slerp(pointOrientations[i], pointOrientations[Math.Min(i + 1, pointOrientations.Count - 1)], t);
                Vector3 rotatedXYModelCoordinate = interpolatedOrientation * inputV;

                builder.AddVertex(
                    interpolatedLineSegmentPoint + rotatedXYModelCoordinate,
                    InputMesh.uv[j] / TextureScale
                );
            }

            // Process triangles
            int numTris = InputMesh.triangles.Length;
            for (int j = 0; j < numTris; j += 3)
            {
                builder.AddTriangle(
                    InputMesh.triangles[j] + numVerts * i,
                    InputMesh.triangles[j + 1] + numVerts * i,
                    InputMesh.triangles[j + 2] + numVerts * i
                );
            }
        }

        Mesh mesh = builder.CreateMesh(true);
        var autoUV = GetComponent<AutoUv>();
        if (autoUV != null && autoUV.enabled && ComputeUVs)
        {
            autoUV.UpdateUVs(mesh);
        }
        ReplaceMesh(mesh, ModifySharedMesh);
    }
}
