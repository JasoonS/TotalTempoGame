using UnityEngine;

using System.Collections.Generic;
using System.Collections;

[RequireComponent(typeof(MeshFilter))]
[ExecuteInEditMode()]
public class CurveImplementation : MonoBehaviour
{
    public int CurveResolution = 500;
    // this is important if you only want a track segment not a closed loop. (Not relevant for our game)
    public bool ClosedLoop = false;

    private float[] _distances; // distances from start up to the end of that segment.
    private float[] _secDistances; // the distance for each segment (effectively just pre-computed: _distance[i] - _distance[i-1]

    public float TotalLength; // same as _distances[_distances.Length - 1];

    private Renderer _trackRenderer;

    public Material[] materials; // List of available track colours.
    private int lastColourIndex = 0; // this is set randomly each time to choose a colour for a track. Triggered by the event system.

    // Variables used for catmul rom. Set each time by the calculatePandM(segmentIndex) function.
    Vector3 p0;
    Vector3 p1;
    Vector3 m0;
    Vector3 m1;

    // Use the static object pattern to guarantee that this object is correctly assigned and pressent in the scene.
    private static CurveImplementation _curve;
    public static CurveImplementation Instance
    {
        get
        {
            if (!_curve)
            {
                _curve = FindObjectOfType(typeof(CurveImplementation)) as CurveImplementation;

                if (!_curve)
                {
                    Debug.LogError("You need to have at least one active 'CurveImplementation.cs' script in your scene.");
                } else {
                  _curve.Init();
                }
            }
            return _curve;
        }
    }

    void Start()
    {
        Init();
    }

    // Keep this, useful for live update, or editing. Currently the function UpdateTrack() is only called once on startup.
    void FixedUpdate()
    {
        UpdateTrack();
    }

    void Init()
    {
        FillAccumalativeDistances();
        TotalLength = _distances[_distances.Length - 1];

        _trackRenderer = GetComponent<Renderer>();

        UpdateTrack();

        EventManager.StartListening("k_changeColour", ChangeColour);
    }

    void OnDisable()
    {
        EventManager.StopListening("k_changeColour", ChangeColour);
    }

    // randomly generate a colour for the track. Never keep the same colour when called.
    public void ChangeColour()
    {
        //Debug.Log("A colour change just occured.");
        int index = Random.Range(0, materials.Length);
        if (index == lastColourIndex) // makes sure that index changes each time.
        {
            index = (1 + index) % materials.Length;
        }
        _trackRenderer.material = materials[index];

        lastColourIndex = index;
    }

    // Function to set _distances, _secDistances
    private void FillAccumalativeDistances()
    {
        _distances = new float[TrackManager.Track.Points.Count + 1];
        _secDistances = new float[TrackManager.Track.Points.Count + 1];

        float accumulateDistance = 0;

        if (TrackManager.Track.Points.Count > 0)
        {
            // Debug.Log("FillingAccumalativeDistances: track has " + TrackManager.Track.Points.Count + " points.");
            for (int i = 0; i < TrackManager.Track.Points.Count + 1; ++i)
            {
                var t1 = TrackManager.Track.Points[(i) % TrackManager.Track.Points.Count].Position;
                var t2 = TrackManager.Track.Points[(i + 1) % TrackManager.Track.Points.Count].Position;

                if (t1 != null && t2 != null)
                {
                    _distances[i] = accumulateDistance;
                    _secDistances[i] = (t1 - t2).magnitude;

                    accumulateDistance += _secDistances[i];
                }
            }
        }
    }

    // Fill the mesh with the correct vertices according to the path.
    void UpdateTrack()
    {
        MeshFilter mf = GetComponent<MeshFilter>();
        Mesh mesh = mf.sharedMesh;

        int closedAdjustment = ClosedLoop ? 0 : 1;

        int pointsToMake = CurveResolution + 1;

        Vector3[] vertices = new Vector3[pointsToMake * 2 + 2];

        int[] triangles = new int[pointsToMake * 6];

        float currentDistance = 0.0f;
        float step = TotalLength / CurveResolution;

        int ind = 0;

        // First for loop goes through each individual control point and connects it to the next, so 0-1, 1-2, 2-3 and so on
        for (int i = 0; i < TrackManager.Track.Points.Count - closedAdjustment; i++)
        {
            calculatePandM(i);

            Vector3 position;

            float t;

            // Second for loop actually creates the spline for this particular segment
            while (currentDistance <= _distances[i + 1])
            {
                t = Mathf.InverseLerp(_distances[i], _distances[i + 1], currentDistance);

                Vector3 tangent;

                float percentThrough = t;

                position = CatmullRom.Interpolate(p0, p1, m0, m1, t, out tangent);

                // Debug.Log("Track details:: Num points: " + TrackManager.Track.Points.Count + "; i is: " + i + "; capacity: " + TrackManager.Track.Points.Count);
                Vector3 normal;
                float centre;
                float width;
                CalculateNormalCenterWidth(i, percentThrough, out normal, out centre, out width);

                vertices[ind * 2] = position + Vector3.Cross(tangent, normal).normalized * width * (1 + centre);
                vertices[ind * 2 + 1] = position - Vector3.Cross(tangent, normal).normalized * width * (1 - centre);

                triangles[ind * 6] = ind * 2 + 1;
                triangles[ind * 6 + 1] = ind * 2;
                triangles[ind * 6 + 2] = ind * 2 + 2;
                triangles[ind * 6 + 3] = ind * 2 + 1;
                triangles[ind * 6 + 4] = ind * 2 + 2;
                triangles[ind * 6 + 5] = ind * 2 + 3;

                ++ind;
                currentDistance += step;
            }
        }

        // close the loop, so the track is complete. (NOTE:: there seems to be a subtle bug with this that I haven't had the patience to fix).
        vertices[pointsToMake * 2] = vertices[0];
        vertices[pointsToMake * 2 + 1] = vertices[1];

        // populate and sort out the mesh. (the following +/- 10 lines)
        mesh.Clear();
        mesh.vertices = vertices;

        mesh.triangles = triangles;

        mesh.Optimize();

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        MeshCollider MCollider = GetComponent<MeshCollider>();
        MCollider.sharedMesh = mesh;
    }

    // Some Cat Mul Rom magic. Just leave alone.
    private void calculatePandM(int i)
    {
        p0 = TrackManager.Track.Points[i].Position;
        p1 = (ClosedLoop == true && i == TrackManager.Track.Points.Count - 1) ? TrackManager.Track.Points[0].Position : TrackManager.Track.Points[i + 1].Position;

        // Tangent calculation for each control point
        // Tangent M[k] = (P[k+1] - P[k-1]) / 2
        // With [] indicating subscript
        // m0
        if (i == 0)
        {
            m0 = ClosedLoop ? 0.5f * (p1 - TrackManager.Track.Points[TrackManager.Track.Points.Count - 1].Position) : p1 - p0;
        }
        else
        {
            m0 = 0.5f * (p1 - TrackManager.Track.Points[i - 1].Position);
        }
        // m1
        if (ClosedLoop)
        {
            if (i == TrackManager.Track.Points.Count - 1)
            {
                m1 = 0.5f * (TrackManager.Track.Points[(i + 2) % TrackManager.Track.Points.Count].Position - p0);
            }
            else if (i == 0)
            {
                m1 = 0.5f * (TrackManager.Track.Points[i + 2].Position - p0);
            }
            else
            {
                m1 = 0.5f * (TrackManager.Track.Points[(i + 2) % TrackManager.Track.Points.Count].Position - p0);
            }
        }
        else
        {
            if (i < TrackManager.Track.Points.Count - 2)
            {
                m1 = 0.5f * (TrackManager.Track.Points[(i + 2) % TrackManager.Track.Points.Count].Position - p0);
            }
            else
            {
                m1 = p1 - p0;
            }
        }
    }

    // TODO:: simplify this function.
    // Set where the vehicle will spawn, according to how far it has traveled through the track.
    public static void SetSpawnTransform(Transform car, float dist, int height)
    {
        int point = 0;

        //Keep the distance in the correct range. Even if the player has done multiple laps.
        dist = Mathf.Repeat(dist, Instance.TotalLength);

        // go through distances until at correct segment index. Works, and is simple. ;)
        while (Instance._distances[point] < dist)
        {
            ++point;
        }

        if (dist == 0)
            ++point;

        Instance.calculatePandM(--point);

        float t = Mathf.InverseLerp(Instance._distances[point], Instance._distances[point + 1], dist);

        Vector3 tangent;

        float percentThrough = t;

        Vector3 position = CatmullRom.Interpolate(Instance.p0, Instance.p1, Instance.m0, Instance.m1, t, out tangent);

        Vector3 normal;
        float centre;
        float width;
        Instance.CalculateNormalCenterWidth(point, percentThrough, out normal, out centre, out width);

        car.position = position + Vector3.Cross(tangent, normal).normalized * width * centre + height * Vector3.up;
        car.up = normal;
        car.forward = tangent;
    }

    //TODO:: this function doesn't do well on slopes or loops. Make it look at the normal.
    // Called for the entire track to generate all the tokens for the track. (randomly) Self explanitory...
    public static void SetTokenPositions(int numTokens)
    {
        int closedAdjustment = Instance.ClosedLoop ? 0 : 1;

        float currentDistance = 0.0f;
        float step = Instance.TotalLength / numTokens;

        int ind = 0;

        // First for loop goes through each individual control point and connects it to the next, so 0-1, 1-2, 2-3 and so on
        for (int i = 0; i < TrackManager.Track.Points.Count - closedAdjustment; i++)
        {
            Instance.calculatePandM(i);

            Vector3 position;

            float t;

            while (currentDistance < Instance._distances[i + 1])
            {
                t = Mathf.InverseLerp(Instance._distances[i], Instance._distances[i + 1], currentDistance);

                Vector3 tangent;

                float percentThrough = t;

                position = CatmullRom.Interpolate(Instance.p0, Instance.p1, Instance.m0, Instance.m1, t, out tangent);

                Vector3 normal;
                float centre;
                float width;
                Instance.CalculateNormalCenterWidth(i, percentThrough, out normal, out centre, out width);

                float centerOffset = Random.Range(-1.0f, 1.0f);

                TokenSpawner.SpawnTokensAtPoint(2 * normal + position + Vector3.Cross(tangent, normal).normalized * width * (centre + centerOffset));

                ++ind;
                currentDistance += step;
            }
        }
    }

    // calculates the normal, centre, width given how far through a track segment it is.
    void CalculateNormalCenterWidth(int segmentIndx, float percentThrough, out Vector3 normal, out float centre, out float width)
    {
        normal = Vector3.Lerp(TrackManager.Track.Points[segmentIndx].Up, TrackManager.Track.Points[(segmentIndx + 1) % TrackManager.Track.Points.Count].Up, percentThrough);
        centre = Mathf.Lerp(TrackManager.Track.Points[segmentIndx].Centre, TrackManager.Track.Points[(segmentIndx + 1) % TrackManager.Track.Points.Count].Centre, percentThrough);
        width = Mathf.Lerp(TrackManager.Track.Points[segmentIndx].Width, TrackManager.Track.Points[(segmentIndx + 1) % TrackManager.Track.Points.Count].Width, percentThrough);
    }

    // Basically the same as UpdateTrack() just drawing pretty balls and stuff...
    void OnDrawGizmos()
    {
        int closedAdjustment = ClosedLoop ? 0 : 1;

        float currentDistance = 0.0f;
        float step = TotalLength / CurveResolution;

        int ind = 0;

        //// First for loop goes through each individual control point and connects it to the next, so 0-1, 1-2, 2-3 and so on
        for (int i = 0; i < TrackManager.Track.Points.Count - closedAdjustment; i++)
        {
            calculatePandM(i);

            Vector3 position;

            float t;

            // Second for loop actually creates the spline for this particular segment
            while (currentDistance < _distances[i + 1])
            {
                t = Mathf.InverseLerp(_distances[i], _distances[i + 1], currentDistance);

                Vector3 tangent;

                float percentThrough = t;

                position = CatmullRom.Interpolate(p0, p1, m0, m1, t, out tangent);

                Gizmos.color = Color.red;
                Gizmos.DrawSphere(position, 0.25f);

                Vector3 normal;
                float centre;
                float width;
                CalculateNormalCenterWidth(i, percentThrough, out normal, out centre, out width);

                Gizmos.color = Color.green;
                Gizmos.DrawSphere(position + Vector3.Cross(tangent, normal).normalized * width * (1 + centre), 0.5f);
                Gizmos.DrawSphere(position - Vector3.Cross(tangent, normal).normalized * width * (1 - centre), 0.5f);

                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(position + Vector3.Cross(tangent, normal).normalized * width * centre, 0.8f);

                ++ind;
                currentDistance += step;
            }
        }
    }
}
