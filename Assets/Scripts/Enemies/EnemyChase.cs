using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyChase : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2.5f;

    private Rigidbody2D rb;
    private Transform target;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player) target = player.transform;
    }

    private void FixedUpdate()
    {
        if (!target) return;

        Vector2 dir = ((Vector2)target.position - rb.position);
        if (dir.sqrMagnitude > 0.0001f)
            dir.Normalize();

        rb.MovePosition(rb.position + dir * moveSpeed * Time.fixedDeltaTime);
    }
}
