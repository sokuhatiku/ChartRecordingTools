using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour {

	void Start()
	{
		var filter = GetComponent<MeshFilter>();
		if (filter == null) return;

		triangles = filter.mesh.triangles;
		filter.mesh.SetIndices(MakeIndices(), MeshTopology.Lines, 0);
	}

	int[] triangles;

	public int[] MakeIndices()
	{
		int[] indices = new int[2 * triangles.Length];
		int i = 0;
		for (int t = 0; t < triangles.Length; t += 3)
		{
			indices[i++] = triangles[t];        //start
			indices[i++] = triangles[t + 1];   //end
			indices[i++] = triangles[t + 1];   //start
			indices[i++] = triangles[t + 2];   //end
			indices[i++] = triangles[t + 2];   //start
			indices[i++] = triangles[t];        //end
		}
		return indices;
	}
}
