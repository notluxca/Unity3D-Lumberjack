using UnityEngine;

public class Tree : MonoBehaviour, IDamageable
{
    private Rigidbody[] destroyableParts;
    private BoxCollider trigger;

    void Awake()
    {
        trigger = GetComponent<BoxCollider>();
        destroyableParts = GetComponentsInChildren<Rigidbody>();
        LockTree();
    }

    private void LockTree()
    {
        foreach (var body in destroyableParts)
        {
            body.constraints = RigidbodyConstraints.FreezeAll;
        }
    }

    private void Unlocktree()
    {
        foreach (var body in destroyableParts)
        {
            body.constraints = RigidbodyConstraints.None;
        }
    }

    public void TakeDamage(Transform hitPosition, int damage, float knockbackForce = 0)
    {
        Destroy(trigger);
        Unlocktree();
        Destroy(this.gameObject, 10);
    }
}
