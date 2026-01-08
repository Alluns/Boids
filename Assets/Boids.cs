using UnityEngine;

public class Boids : MonoBehaviour
{
    [SerializeField] private Vector3 bounds = Vector3.one;
    [Range(0, 1000)]
    [SerializeField] private int boidsAmount;

    private Boid[] boids = new Boid[1000];

    public class Boid
    {
        
    }
    
    

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(transform.position, bounds);
    }
}
