using UnityEngine;
using System.Collections;

public class Waypoint : MonoBehaviour {

    [SerializeField] [Range(0.1f, 20)] private float _width = 2;

    public float Width { get { return _width; } }

    public Quaternion Rotation { get { return transform.rotation; } }

    public Vector3 Up { get { return transform.up; } }

    public Vector3 Position { get { return transform.position; } }

    [SerializeField] [Range(-1, 1)] private float _centre = 0f;

    public float Centre { get { return _centre; } }
}
