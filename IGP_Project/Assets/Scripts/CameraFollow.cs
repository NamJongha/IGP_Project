using UnityEngine;
using Fusion;

public class CameraFollow : NetworkBehaviour
{
    public string playerTag = "Player";
    public float smoothSpeed = 0.125f;
    public Vector3 offset;

    [Networked] private Vector3 syncedCameraPostion { get; set; }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();

        CameraFollowing();
    }

    private void CameraFollowing()
    {
        if (HasStateAuthority)
        {

            GameObject[] players = GameObject.FindGameObjectsWithTag(playerTag);

            if (players.Length == 0) return;

            Vector3 totalPosition = Vector3.zero;

            foreach (GameObject player in players)
            {
                totalPosition += player.transform.position;
            }

            Vector3 averagePosition = totalPosition / players.Length;

            Vector3 targetPosition = new Vector3(averagePosition.x + offset.x, transform.position.y, transform.position.z);

            //Vector3 smoothedPosition = Vector3.Lerp(transform.position, targetPosition, smoothSpeed);
            //transform.position = smoothedPosition;

            syncedCameraPostion = Vector3.Lerp(transform.position, targetPosition, smoothSpeed);

            transform.position = syncedCameraPostion;
        }
    }
}