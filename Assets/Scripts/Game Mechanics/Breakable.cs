using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;

// For spawning break particles
public enum BreakType
{
    Rate, // assuming duration = 0.1, particleDensity is rate over time per square meter
    Burst,
    Independent
}

public class Breakable : MonoBehaviour
{
    public bool breakThrough = false;
    public float breakThreshold;
    public int hp = 1;

    public GameObject[] breakParticlesArray;
    public float particleDensity; 
    public BreakType breakType = BreakType.Rate;

    public GameObject breakSound;
    public GameObject crackSound;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        Rigidbody otherRb = collision.gameObject.GetComponent<Rigidbody>();

        //Debug.Log(Vector3.Dot(collision.impulse.normalized, collision.relativeVelocity));

        if (Mathf.Abs(Vector3.Dot(collision.impulse.normalized, collision.relativeVelocity)) > breakThreshold)
        {
            hp--;
            float densityMultiplier = 0.3f;
            GameObject soundToPlay = crackSound;

            if (hp <= 0)
            {
                if (breakThrough)
                {
                    otherRb.AddForce(-collision.impulse, ForceMode.Impulse);
                }

                densityMultiplier = 1;
                soundToPlay = breakSound;
                gameObject.SetActive(false);

                Destroy(gameObject, 5);

            }

            if (soundToPlay)
            {
                GameObject soundInstance = Instantiate(soundToPlay);
                Destroy(soundInstance, 5);
            }

            SpawnParticles(densityMultiplier);
            
        }

    }

    private void SpawnParticles(float densityMultiplier)
    {
        foreach (GameObject breakParticles in breakParticlesArray)
        {
            GameObject spawned = Instantiate(breakParticles);
            ParticleSystem pc = spawned.GetComponent<ParticleSystem>();

            var s = pc.shape;
            s.meshRenderer = GetComponentInChildren<MeshRenderer>();

            var e = pc.emission;

            switch (breakType)
            {
                case BreakType.Rate:
                    e.rateOverTime = (particleDensity * densityMultiplier * CalculateSurfaceArea()) / breakParticlesArray.Length;
                    break;
                case BreakType.Burst:
                    e.SetBursts(
                    new ParticleSystem.Burst[]
                    {
                        new ParticleSystem.Burst(0, (particleDensity * densityMultiplier * CalculateSurfaceArea()) / breakParticlesArray.Length)
                    });
                    break;
                default:
                    break;

            }
            spawned.SetActive(true);

            Destroy(spawned, 10);
        }
            
    }


    public bool WillCrack(Collision collision)
    {
        return Mathf.Abs(Vector3.Dot(collision.impulse.normalized, collision.relativeVelocity)) > breakThreshold;
    }

    // Assisted by ChatGPT, thanks!
    public float CalculateSurfaceArea()
    {
        Mesh mesh = gameObject.GetComponentInChildren<MeshFilter>().mesh;
        float surfaceArea = 0f;

        for (int i = 0; i < mesh.triangles.Length; i += 3)
        {
            Vector3 v0 = mesh.vertices[mesh.triangles[i]];
            Vector3 v1 = mesh.vertices[mesh.triangles[i + 1]];
            Vector3 v2 = mesh.vertices[mesh.triangles[i + 2]];

            Vector3 crossProduct = Vector3.Cross(v1 - v0, v2 - v0);
            surfaceArea += crossProduct.magnitude / 2f;
        }

        return surfaceArea;
    }
}
