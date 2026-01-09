using UnityEngine;
using Random = UnityEngine.Random;

public class Boids : MonoBehaviour
{
    [Header("Container")]
    [SerializeField] private Vector3 bounds = Vector3.one;
    [Range(0, 3000)]
    [SerializeField] private int boidsAmount;

    [Header("Boids")]
    [SerializeField] private float contextRadius = 1f;
    [SerializeField] private float avoidanceRadius = 0.5f;
    [SerializeField] private float speed = 1f;


    [Range(0, 1)] [SerializeField] private float cohesionWeight = 0.25f;
    [Range(0, 1)] [SerializeField] private float avoidanceWeight = 0.25f;
    [Range(0, 1)] [SerializeField] private float alignmentWeight = 0.25f;
    
    private readonly Boid[] boids = new Boid[3000];

    public class Boid
    {
        public Vector3 Position;
        public Vector3 Velocity;
        public bool IsEnabled = false;
        public int neighbourCount = 0;

        public void Simulate(Boids container)
        {
            Vector3 neighboursCenterOfMass = new();
            Vector3 closeNeighboursVectors = new();
            Vector3 neighboursAverageVelocity = new();
            
            neighbourCount = 0;
            
            foreach (Boid boid in container.boids)
            {
                if (!boid.IsEnabled) break;
                if (Vector3.Distance(Position, boid.Position) > container.contextRadius || boid == this) continue;
                
                // Rule 1: Boids try to fly towards the center of mass of its neighbours
                neighboursCenterOfMass += boid.Position;

                // Rule 2: Boids try to keep a small distance from the neighbours
                if (Vector3.Distance(Position, boid.Position) < container.avoidanceRadius)
                {
                    closeNeighboursVectors -= boid.Position - Position;
                }
                
                // Rule 3: Boids try to match the direction (velocity?) of its neighbours
                neighboursAverageVelocity += boid.Velocity;
                
                neighbourCount++;
            }
            neighboursCenterOfMass /= neighbourCount;
            neighboursAverageVelocity /= neighbourCount;
            
            Vector3 cohesion = (neighboursCenterOfMass - Position).normalized * container.cohesionWeight;
            Vector3 avoidance = closeNeighboursVectors.normalized * container.avoidanceWeight;
            Vector3 alignment = neighboursAverageVelocity.normalized * container.alignmentWeight;
            
            Velocity = (Velocity.normalized + cohesion + avoidance + alignment) * container.speed;
            Position += Velocity * Time.deltaTime ;
            
            Position = new Vector3(
                Position.x = Position.x > 0 ?
                    (Position.x + container.bounds.x / 2) % container.bounds.x - container.bounds.x / 2 :
                    (Position.x - container.bounds.x / 2) % container.bounds.x + container.bounds.x / 2,
                Position.y = Position.y > 0 ?
                    (Position.y + container.bounds.y / 2) % container.bounds.y - container.bounds.y / 2 :
                    (Position.y - container.bounds.y / 2) % container.bounds.y + container.bounds.y / 2,
                Position.z = Position.z > 0 ?
                    (Position.z + container.bounds.z / 2) % container.bounds.z - container.bounds.z / 2 :
                    (Position.z - container.bounds.z / 2) % container.bounds.z + container.bounds.z / 2);
        }
    }

    private void Start()
    {
        for (int i = 0; i < boids.Length; i++)
        {
            boids[i] = new Boid
            {
                Position = new Vector3(Random.Range(0, bounds.x) - bounds.x / 2, Random.Range(0, bounds.y) - bounds.y / 2, Random.Range(0, bounds.z) - bounds.z / 2),
                Velocity = Vector3.forward,
                IsEnabled = i < boidsAmount
            };
        }
    }

    private void Update()
    {
        foreach (Boid boid in boids)
        {
            if (!boid.IsEnabled) break;
            boid.Simulate(this);
        }
    }
    
    private void OnValidate()
    {
        if (!Application.isPlaying) return;
        
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
            
            // Gizmos.color = Color.Lerp(Color.darkGreen, Color.limeGreen, Mathf.InverseLerp(-bounds.y / 2, bounds.y / 2, boid.Position.y));
            Gizmos.color = Color.Lerp(Color.orangeRed, Color.dodgerBlue, Mathf.InverseLerp(0, 60, boid.neighbourCount));
            Gizmos.DrawSphere(boid.Position, 0.1f);
            
            // Gizmos.color = Color.darkOrange;
            // Gizmos.DrawWireSphere(boid.Position, contextRadius);
            
            // Gizmos.color = Color.limeGreen;
            // Gizmos.DrawRay(new Ray(boid.Position, boid.Velocity));
        }
    }
}
