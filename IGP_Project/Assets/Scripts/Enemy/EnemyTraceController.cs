using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTraceController : MonoBehaviour
{
    public float moveSpeed = 0.5f;
    public float raycastDistance = 0.2f;
    public float traceDistance = 2f;

    private Transform player_transform;

    void Start()
    {
        player_transform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 direction = player_transform.position - transform.position;

        if (direction.magnitude > traceDistance)
            return;

        Vector2 directionNormalized = direction.normalized;

        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, directionNormalized, raycastDistance);

        foreach (RaycastHit2D rHit in hits)
        {
            if (rHit.collider != null && rHit.collider.CompareTag("Obstacle"))// && PlayerTest.Instance.isPlayerDirectionRight)
            {
                Vector3 alternativeDirection = Quaternion.Euler(0f, 0f, -90f) * direction;
                transform.Translate(alternativeDirection * moveSpeed * Time.deltaTime);
            }

            else
            {
                transform.Translate(direction * moveSpeed * Time.deltaTime);
            }
        }
    }
}
