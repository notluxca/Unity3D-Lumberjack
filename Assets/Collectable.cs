using UnityEngine;


public class Collectable : MonoBehaviour
{

    [Header("Money Prompt Settings")]
    public bool AddCash = true;
    public float flyTime = 1f;
    public float arcHeight = 2f;

    private Vector3 startPos;
    private float elapsedTime;

    private bool isFlying = false;
    private Transform Target;

    /*************  ✨ Windsurf Command ⭐  *************/
    /// <summary>
    /// Called when the trigger collider of this object starts touching
    /// another object's collider.
    /// </summary>
    /// <param name="other">The other object involved in this collision.</param>
    /// <remarks>
    /// If the other object is a player, set the target to be the player's
    /// transform and fly from the current position to the target.
    /// </remarks>
    /*******  d27e6b3a-d1f8-42f9-b7e5-973757d90164  *******/
    private void OnTriggerEnter(Collider other)
    {
        PlayerController playerController = other.GetComponent<PlayerController>();
        if (playerController && !isFlying)
        {
            Target = playerController.transform;
            FlyFrom(transform.position);
        }
    }

    void Update()
    {
        if (!isFlying || Target == null) return;

        elapsedTime += Time.deltaTime;
        float t = Mathf.Clamp01(elapsedTime / flyTime);

        Vector3 currentTarget = Target.position;

        Vector3 flatPos = Vector3.Lerp(startPos, currentTarget, t);
        float heightOffset = arcHeight * Mathf.Sin(t * Mathf.PI);
        flatPos.y += heightOffset;

        transform.position = flatPos;

        if (Vector3.Distance(transform.position, currentTarget) <= 0.5f)
        {
            if (AddCash)
                CurrencyManager.Instance.AddCash(1);

            transform.parent = Target;
            Destroy(gameObject);
        }
    }


    public void FlyFrom(Vector3 from)
    {
        if (Target == null)
        {
            Target = GameObject.FindGameObjectWithTag("Player")?.transform;
            if (Target == null)
            {
                Debug.LogWarning("FlyToPlayer: No player found!");
                return;
            }
        }

        transform.position = from;
        startPos = from;
        elapsedTime = 0f;
        isFlying = true;
    }
}

