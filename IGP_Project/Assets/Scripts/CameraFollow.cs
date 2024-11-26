using UnityEngine;
using Fusion;

public class CameraFollow : NetworkBehaviour
{
    public string playerTag = "Player";
    public float smoothSpeed = 0.125f;
    public Vector3 offset;

    private void LateUpdate()
    {
        if (!HasStateAuthority)
            return;

        GameObject[] players = GameObject.FindGameObjectsWithTag(playerTag);

        if (players.Length == 0) return;

        Vector3 totalPosition = Vector3.zero;

        foreach (GameObject player in players)
        {
            totalPosition += player.transform.position;
        }

        Vector3 averagePosition = totalPosition / players.Length;

        Vector3 targetPosition = new Vector3(averagePosition.x + offset.x, transform.position.y, transform.position.z);

        Vector3 smoothedPosition = Vector3.Lerp(transform.position, targetPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }
}