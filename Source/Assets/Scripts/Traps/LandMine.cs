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

    private void Start()
    {
        if (verboseLogging)
        {
            Debug.Log(nameof(Start), this);
        }

        onTriggerEnterHandler.OnTrigger += OnColliderEntered;
    }

    private void OnColliderEntered(Collider collider)
    {
        if (verboseLogging)
        {
            Debug.Log(nameof(OnColliderEntered) + " ( " + nameof(collider) + ": " + collider.gameObject.name + " )", this);
        }

        Collider[] collidersInBlashRadius = Physics.OverlapSphere(transform.position, blastRadius);

        for (int i = 0; i < collidersInBlashRadius.Length; i++)
        {
            Ray ray = new Ray(transform.position, collidersInBlashRadius[i].transform.position - transform.position);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Enemy enemy = hit.collider.GetComponent<Enemy>();

                if (enemy)
                {
                    enemy.TakeDamage(damage);
                }

                Player player = hit.collider.GetComponent<Player>();

                if (player)
                {
                    player.DecrementHealth(damage);
                }

                Barricade barricade = hit.collider.GetComponent<Barricade>();

                if (barricade)
                {
                    barricade.Hit();
                }
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
