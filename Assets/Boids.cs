using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Boids : MonoBehaviour
{
    [Header("Container")]
    [SerializeField] private Vector3 bounds = Vector3.one;
    [Range(0, 200)]
    [SerializeField] private int boidsAmount;
    
    private readonly Boid[] boids = new Boid[200];

    public class Boid
    {
        public Vector3 Position;
        public Vector3 Direction;
        public bool IsEnabled = false;

        private const float TurnRate = 90f;
        private const float ContextRadius = 1f;

        public void Simulate(Boid[] boids)
        {
            List<Boid> neighbours = new List<Boid>();
            
            foreach (Boid boid in boids)
            {
                if (!boid.IsEnabled) break;

                if (Vector3.Distance(Position, boid.Position) < ContextRadius && boid != this)
                {
                    neighbours.Add(boid);
                }
            }

            if (neighbours.Count > 0)
            {
                // separation: steer to avoid crowding local flockmates
                //TODO: Steer away from the average point of all neighbours
            
                // alignment: steer towards the average heading of local flockmates
                //TODO: Steer towards the average direction of all neighbours
            
                // cohesion: steer to move towards the average position (center of mass) of local flockmates
                //TODO: Steer towards the average point of all neighbours
                // Vector3 averagePosition = new();
                //
                // foreach (Boid neighbour in neighbours)
                // {
                //     averagePosition += neighbour.Position;
                // }
                // averagePosition /= neighbours.Count;
                //
                // Direction = Vector3.RotateTowards(Direction, averagePosition, TurnRate * Mathf.Deg2Rad * Time.deltaTime, 0.0f);
            }
            
            Position += Direction * Time.deltaTime;
        }
    }

    private void Start()
    {
        for (int i = 0; i < boids.Length; i++)
        {
            boids[i] = new Boid
            {
                Position = new Vector3(Random.Range(0, bounds.x) - bounds.x / 2, Random.Range(0, bounds.y) - bounds.y / 2, Random.Range(0, bounds.z) - bounds.z / 2),
                Direction = Vector3.forward,
                IsEnabled = i < boidsAmount
            };
        }
    }

    private void Update()
    {
        foreach (Boid boid in boids)
        {
            if (!boid.IsEnabled) break;
            boid.Simulate(boids);
        }
    }

    private void OnValidate()
    {
        for (int i = 0; i < boids.Length; i++)
        {
            boids[i].IsEnabled = i < boidsAmount;
        }
    }


    private void OnDrawGizmos()
    {
        // Bounding box
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(transform.position, bounds);

        // Boids
        foreach (Boid boid in boids)
        {
            if (boid is not { IsEnabled: true }) break;
            
            Gizmos.color = Color.dodgerBlue;
            Gizmos.DrawSphere(boid.Position, 0.05f);
            
            Gizmos.color = Color.darkOrange;
            Gizmos.DrawWireSphere(boid.Position, 1f);
            
            Gizmos.color = Color.limeGreen;
            Gizmos.DrawRay(new Ray(boid.Position, boid.Direction));
        }
    }
}
