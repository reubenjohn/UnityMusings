using System;
using UnityEngine;
using static Utils;

public class Bird : MonoBehaviour
{
    [SerializeField] private float maxSpeed = 4f;
    [SerializeField] private float personalSpaceCoefficient = 4f;
    [SerializeField] private float alignmentCoefficient = .1f;
    [SerializeField] private float healthSpeed = 1e-8f;
    [SerializeField] private float explosionRadius = 1f;
    [SerializeField] private float explosionPower = 10f;
    [SerializeField] private Transform front;
    [SerializeField] private Transform spawnPoint;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private float health = 0;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    void Update()
    {
        LerpHealth(+1);
        rb.velocity = transform.up.normalized * maxSpeed;   // Only move in forward direction
        spriteRenderer.color = Color.Lerp(Color.red, Color.blue, (.5f + health * 500f));
        float random = UnityEngine.Random.Range(0, 1f);
        if (health > random)
            Reproduce();
        else if (health < -random)
            Die();
    }
    void Reproduce()
    {
        this.health = 0;
        Instantiate(gameObject, spawnPoint.position, spawnPoint.rotation, transform.parent).GetComponent<Bird>().health = 0;
    }
    void Die()
    {
        Vector3 explosionPos = transform.position;
        foreach (Collider2D hit in Physics2D.OverlapCircleAll(explosionPos, explosionRadius))
            if (hit.TryGetComponent<Rigidbody2D>(out Rigidbody2D nearbyRigidBody))
                nearbyRigidBody.AddForce((hit.transform.position - explosionPos).normalized * explosionPower, ForceMode2D.Impulse);
        Destroy(gameObject);
    }

    void OnCollisionStay2D(Collision2D other) => LerpHealth(-10);

    void OnTriggerStay2D(Collider2D other)  // obstacle in range
    {
        AvoidCollision(other);
        if (other.TryGetComponent(out Bird bird))
            TryAlignWithBird(bird);
    }

    void AvoidCollision(Collider2D other)
    {
        Vector2 vectorToTarget = other.ClosestPoint(front.position) - (Vector2)front.position;
        rb.AddForceAtPosition(vectorToTarget.normalized * -personalSpaceCoefficient / (vectorToTarget.magnitude + .1f), front.position);
    }

    void TryAlignWithBird(Bird bird) => transform.rotation = Quaternion.Slerp(transform.rotation, bird.transform.rotation, alignmentCoefficient);
    void LerpHealth(float target) => health = Lerp(health, target, healthSpeed);
}
