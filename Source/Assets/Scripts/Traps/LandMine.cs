using UnityEngine;

public class LandMine : MonoBehaviour
{
    [SerializeField] private int damage = 20;
    [SerializeField] private float blastRadius = 4f;
    [SerializeField] private OnTriggerEnterHandler onTriggerEnterHandler = null;
    [SerializeField] private GameObject setInactiveOnTrigger = null;
    [SerializeField] private GameObject setActiveOnTrigger = null;
    [SerializeField] private float timeToDestroyAfterTrigger = 5f;
    [SerializeField] private bool verboseLogging = false;


    private void Awake()
    {
        onTriggerEnterHandler.OnTrigger += OnColliderEntered;
    }

    private void OnDestroy()
    {
        onTriggerEnterHandler.OnTrigger -= OnColliderEntered;
    }

    private void OnColliderEntered(Collider collider)
    {
        // Ignore player
        if (collider.CompareTag(GameConstants.TagConstants.PlayerTag)) return;

        if (verboseLogging)
        {
            Debug.Log(nameof(OnColliderEntered) + " ( " + nameof(collider) + ": " + collider.gameObject.name + " )", this);
        }

        Collider[] collidersInBlashRadius = Physics.OverlapSphere(transform.position, blastRadius);

        for (int i = 0; i < collidersInBlashRadius.Length; i++)
        {
            Enemy enemy = collidersInBlashRadius[i].GetComponent<Enemy>();

            if (enemy)
            {
                enemy.TakeDamage(damage);
            }
        }

        setActiveOnTrigger.SetActive(true);
        setInactiveOnTrigger.SetActive(false);

        Destroy(gameObject, timeToDestroyAfterTrigger);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, blastRadius);
    }
}
