using UnityEngine;

public class DynamicMeshScript : MonoBehaviour
{
    public Vector3 VertLeftTopFront = new Vector3(-2, 0, 4);
	public Vector3 VertRightTopFront = new Vector3(2, 0, 4);
	public Vector3 VertRightTopBack = new Vector3(2,0,0);
	public Vector3 VertLeftTopBack = new Vector3(-2,0,0);

	private float WaitN = 1f;
	public float WaitTime = 1f;
	public int ShapeN = 0;

	// Use this for initialization

	void Start ()
    {

		MeshFilter mf = GetComponent<MeshFilter>();
		Mesh mesh = mf.mesh;

		//Vertices//

		Vector3[] vertices = new Vector3[]
		{
			//front face//

			VertLeftTopFront,//left top front, 0
			VertRightTopFront,//right top front, 1
			new Vector3(-2,0,4),//left bottom front, 2
			new Vector3(2,0,4),//right bottom front, 3

			//back face//

			VertRightTopBack,//right top back, 4
			VertLeftTopBack,//left top back, 5
			new Vector3(2,0,-4),//right bottom back, 6
			new Vector3(-2,0,-4),//left bottom back, 7

		};

		//Triangles// 3 points, clockwise determines which side is visible

		int[] triangles = new int[]
		{
			//front face//

			0,2,3,//first triangle
			3,1,0,//second triangle

			//back face//

			4,6,7,//first triangle
			7,5,4,//second triangle

			//left face//

			5,7,2,//first triangle
			2,0,5,//second triangle

			////right face//

			3,4,1,//first triangle
			3,6,4,//second triangle

			////top face//

			1,5,0,//first triangle
			5,1,4,//second triangle

			////bottom face//

			2,1,3,//first triangle
			3,7,6//second triangle
		};

        mesh.Clear ();
		mesh.vertices = vertices;
		mesh.triangles = triangles;

        //mesh.uv = uvs; // can add later if necessary (ie if using textures...)

        mesh.Optimize();
		mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        MeshCollider MCollider = GetComponent<MeshCollider>();
        MCollider.sharedMesh = mesh;
    }

    public void nextRampState()
    {
        ShapeN = ++ShapeN % 2;
    }
	
	// Update is called once per frame

	void Update ()
    {
		//if(waitN > 0f)
		//{
		//	waitN -= Time.deltaTime;
		//}
		//else
		//{
		//	waitN = waitTime;
		//	shapeN = ++shapeN % 2;
		//}

        //morph to cube//

        if (ShapeN == 0)
        {
            VertLeftTopFront = Vector3.Lerp(VertLeftTopFront, new Vector3(-2, 2, 4), Time.deltaTime);
            VertRightTopFront = Vector3.Lerp(VertRightTopFront, new Vector3(2, 2, 4), Time.deltaTime);
            VertRightTopBack = Vector3.Lerp(VertRightTopBack, new Vector3(2, 0.75f, 0), Time.deltaTime);
            VertLeftTopBack = Vector3.Lerp(VertLeftTopBack, new Vector3(-2, 0.75f, 0), Time.deltaTime);
        }

        //morph to pyramid//

        if (ShapeN == 1)
        {
            VertLeftTopFront = Vector3.Lerp(VertLeftTopFront, new Vector3(-2, 0, 4), Time.deltaTime);
            VertRightTopFront = Vector3.Lerp(VertRightTopFront, new Vector3(2, 0, 4), Time.deltaTime);
            VertRightTopBack = Vector3.Lerp(VertRightTopBack, new Vector3(2, 0, 4), Time.deltaTime);
            VertLeftTopBack = Vector3.Lerp(VertLeftTopBack, new Vector3(-2, 0, 4), Time.deltaTime);
        }

        Start();
	}
}