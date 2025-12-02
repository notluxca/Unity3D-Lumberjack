using UnityEngine;

public class Turret : MonoBehaviour
{
    [Header("Detection")]
    public float searchRadius = 10f;

    [Header("Combat")]
    public GameObject projectile;
    public float shootInterval = 1f;
    public Transform head;
    public Transform firePoint;

    [Header("Rotation")]
    public float smoothTime = 0.3f;

    [Header("Idle Behavior")]
    public float idleLookInterval = 3f;
    public float idleLookAngleRange = 90f;
    public float maxPitchAngle = 15f;

    [Header("Shoot Pause")]
    public float shootPauseDuration = 0.2f;
    private float shootPauseEndTime = 0f;

    [Header("Aim Requirements")]
    public float aimTolerance = 5f;

    [Header("Laser")]
    public GameObject laserObject; // The laser visual

    private float nextShootTime;
    private float nextIdleLookTime;
    private Vector3 currentVelocity;
    private Quaternion targetRotation;
    private Vector3 idleTargetPoint;
    private bool playerInRange;

    private void Start()
    {
        targetRotation = head.rotation;
        ChooseNewIdlePoint();

        if (laserObject != null)
            laserObject.SetActive(false); // start off
    }

    private void Update()
    {
        playerInRange = false;

        Collider[] hits = Physics.OverlapSphere(transform.position, searchRadius);

        foreach (Collider hit in hits)
        {
            if (hit.gameObject.CompareTag("Player"))
            {
                playerInRange = true;

                Vector3 directionToPlayer = hit.transform.position - head.position;
                directionToPlayer.y += 1.8f;

                targetRotation = Quaternion.LookRotation(directionToPlayer);

                // Only shoot if aimed correctly
                if (Time.time >= nextShootTime)
                {
                    if (IsAimedAtTarget(directionToPlayer))
                    {
                        Shoot();
                        nextShootTime = Time.time + shootInterval;
                    }
                }

                break;
            }
        }

        // ✔️ Laser ON/OFF
        if (laserObject != null)
            laserObject.SetActive(playerInRange);

        if (!playerInRange)
        {
            if (Time.time > nextIdleLookTime)
            {
                ChooseNewIdlePoint();
                nextIdleLookTime = Time.time + idleLookInterval;
            }

            Vector3 directionToIdle = idleTargetPoint - head.position;
            targetRotation = Quaternion.LookRotation(directionToIdle);
        }

        if (Time.time >= shootPauseEndTime)
        {
            head.rotation = SmoothDampQuaternion(head.rotation, targetRotation, ref currentVelocity, smoothTime);
        }
    }

    private bool IsAimedAtTarget(Vector3 direction)
    {
        float angle = Vector3.Angle(head.forward, direction.normalized);
        return angle <= aimTolerance;
    }

    private void ChooseNewIdlePoint()
    {
        float randomYaw = Random.Range(-idleLookAngleRange, idleLookAngleRange);
        float randomPitch = Random.Range(-maxPitchAngle, maxPitchAngle);

        Quaternion randomRotation = Quaternion.Euler(randomPitch, randomYaw, 0f);
        Vector3 randomDirection = transform.rotation * randomRotation * Vector3.forward;

        idleTargetPoint = head.position + randomDirection * searchRadius;
    }

    private Quaternion SmoothDampQuaternion(Quaternion current, Quaternion target, ref Vector3 velocity, float smoothTime)
    {
        Vector3 currentEuler = current.eulerAngles;
        Vector3 targetEuler = target.eulerAngles;

        float x = Mathf.SmoothDampAngle(currentEuler.x, targetEuler.x, ref velocity.x, smoothTime);
        float y = Mathf.SmoothDampAngle(currentEuler.y, targetEuler.y, ref velocity.y, smoothTime);
        float z = Mathf.SmoothDampAngle(currentEuler.z, targetEuler.z, ref velocity.z, smoothTime);

        return Quaternion.Euler(x, y, z);
    }

    private void Shoot()
    {
        Debug.Log("Bang!");
        Instantiate(projectile, firePoint.position, firePoint.rotation);

        shootPauseEndTime = Time.time + shootPauseDuration;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, searchRadius);

        if (!playerInRange && Application.isPlaying)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(idleTargetPoint, 0.3f);
        }
    }
}
