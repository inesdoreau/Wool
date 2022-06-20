using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YarnCreator : MonoBehaviour
{
	public int iterations = 5;
	public float gravity = 10;
	public float damping = 0.7f;
	public PathAutoEndPoints pathInfo;
	public int numPoints = 20;
	public float meshThickness = 1;
	public int cylinderResolution = 5;

	float pathLength;
	float pointSpacing;
	List<Vector3> points;
	List<Vector3> pointsOld;
	Vector3[] pointsCreator;

	bool pinStart = true;
	bool pinEnd = true;

	[SerializeField]
	GameObject meshHolder;

	MeshFilter meshFilter;
	MeshRenderer meshRenderer;
	MeshCollider meshCollider;
	Mesh mesh;

	void Start()
	{
		AssignMeshComponents();
		points = new List<Vector3>();
		pointsOld = new List<Vector3>();

		for (int i = 0; i < numPoints; i++)
        {
			points.Add(Vector3.zero);
			pointsOld.Add(Vector3.zero);
        }

		for (int i = 0; i < numPoints; i++)
		{
			float t = i / (numPoints - 1f);
			points[i] = pathInfo.pathCreator.path.GetPointAtTime(t, PathCreation.EndOfPathInstruction.Stop);
			pointsOld[i] = points[i];
		}

		for (int i = 0; i < numPoints - 1; i++)
		{
			pathLength += Vector3.Distance(points[i], points[i + 1]);
		}
		pointSpacing = pathLength / points.Count;
	}

	void LateUpdate()
	{
		pointsCreator = points.ToArray();
		CylinderGenerator.CreateMesh(ref mesh, pointsCreator, cylinderResolution, meshThickness);
		meshFilter.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity); // mesh is in worldspace
		meshFilter.mesh = mesh;
		meshCollider.sharedMesh = mesh;
	}


	void FixedUpdate()
	{
		points[0] = pathInfo.origin.position;

		for (int i = 0; i < points.Count; i++)
		{
			bool pinned = (i == 0 && pinStart) || (i == points.Count - 1 && pinEnd);
			if (!pinned)
			{
				Vector3 curr = points[i];
				points[i] = points[i] + (points[i] - pointsOld[i]) * damping + Vector3.down * gravity * Time.deltaTime * Time.deltaTime;
				pointsOld[i] = curr;
			}
		}

		for (int i = 0; i < iterations; i++)
		{
			ConstrainCollisions();
			ConstrainConnections();
		}

		//float stretchDstAtPlug = Vector3.Distance(points[numPoints - 2], points[numPoints - 1]) - pointSpacing;
		//if (stretchDstAtPlug > 0.07f)
		//{
		//	//pinEnd = false;
		//	//Debug.Log("Plug pulled out");
		//}

	}

	public void AddPoint()
    {		
		points.Add(Vector3.zero);
		pointsOld.Add(Vector3.zero);
		numPoints++;
	}

	public void RemovePoint()
    {
		points.RemoveAt(0);
		pointsOld.RemoveAt(0);
		numPoints++;
	}

	void ConstrainConnections()
	{
		for (int i = 0; i < points.Count - 1; i++)
		{
			Vector3 centre = (points[i] + points[i + 1]) / 2;
			Vector3 offset = points[i] - points[i + 1];
			float length = offset.magnitude;
			Vector3 dir = offset / length;

			//if (length > pointSpacing || length < pointSpacing * 0.5f)
			{
				//float desiredLength = Mathf.Min(length, pointSpacing);
				//desiredLength = Mathf.Lerp(desiredLength, pointSpacing, 0.25f * Time.deltaTime);
				if (i != 0 || !pinStart)
				{
					points[i] = centre + dir * pointSpacing / 2;
				}
				if (i + 1 != points.Count - 1 || !pinEnd)
				{
					points[i + 1] = centre - dir * pointSpacing / 2;
				}
			}
		}
	}

	void ConstrainCollisions()
	{
        for (int i = 0; i < points.Count; i++)
        {
            bool pinned = i == 0 || i == points.Count - 1;
            if (!pinned)
            {
                if (points[i].y < meshThickness / 2)
                {
                    points[i].Set(points[i].x, meshThickness / 2, points[i].z);
                }
            }
        }
    }
	// Add MeshRenderer and MeshFilter components to this gameobject if not already attached
	void AssignMeshComponents()
	{

		if (meshHolder == null)
		{
			meshHolder = new GameObject("Mesh Holder");
		}

		meshHolder.transform.rotation = Quaternion.identity;
		meshHolder.transform.position = Vector3.zero;
		meshHolder.transform.localScale = Vector3.one;

		// Ensure mesh renderer and filter components are assigned
		if (!meshHolder.gameObject.GetComponent<MeshFilter>())
		{
			meshHolder.gameObject.AddComponent<MeshFilter>();
		}
		if (!meshHolder.GetComponent<MeshRenderer>())
		{
			meshHolder.gameObject.AddComponent<MeshRenderer>();
		}

		meshRenderer = meshHolder.GetComponent<MeshRenderer>();
		meshFilter = meshHolder.GetComponent<MeshFilter>();
		meshCollider = meshHolder.GetComponent<MeshCollider>();
		if (mesh == null)
		{
			mesh = new Mesh();
		}
		meshFilter.sharedMesh = mesh;
		meshCollider.sharedMesh = mesh;
	}

	void OnDrawGizmos()
	{
		if (points != null)
		{
			for (int i = 0; i < points.Count; i++)
			{
				Gizmos.DrawSphere(points[i], 0.05f);
			}
		}
	}
}
