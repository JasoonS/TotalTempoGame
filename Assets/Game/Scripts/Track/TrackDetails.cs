using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TrackDetails : MonoBehaviour {
    public List<Waypoint> Points = new List<Waypoint>();

    //[SerializeField]
    //private float _trackLength { get; set; }

    void Start()
    {
        Waypoint[] wayPoints = GetComponentsInChildren<Waypoint>();

        Points.Clear();
        foreach (Waypoint waypoint in wayPoints)
        {
            Points.Add(waypoint);
        }
    }
}
