using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10f;
    public float lifespan = 5f;
    public int damage = 10;

    private float timer = 0f;

    // Update is called once per frame
    void Update()
    {
        transform.Translate(-Vector3.forward * speed * Time.deltaTime);

        timer += Time.deltaTime;

        if (timer >= lifespan)
        {
            Destroy();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        IDamageable damageable = other.gameObject.GetComponent<IDamageable>();
        Debug.Log("projectile hit another " + other.gameObject.name);

        if (damageable != null)
        {
            damageable.TakeDamage(transform, damage);
            Destroy();
        }
    }

    private void Destroy()
    {
        Destroy(gameObject);
    }
}

