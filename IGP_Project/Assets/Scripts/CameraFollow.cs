using UnityEngine;
using Fusion;

public class CameraFollow : NetworkBehaviour
{
    public string playerTag = "Player";
    public float smoothSpeed = 0f;
    public Vector3 offset;
    public GameObject[] players;

    [Networked] private Vector3 syncedCameraPosition { get; set; }

    public float GetCameraWidthPosition()
    {
        return syncedCameraPosition.x + Camera.main.orthographicSize * Screen.width / Screen.height;
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();

        CameraFollowing();
    }

    private void CameraFollowing()
    {
        if (HasStateAuthority)
        {
                players = GameObject.FindGameObjectsWithTag(playerTag);

            if (players.Length == 0) return;

            Vector3 leftPosition = Vector3.zero;
            Vector3 rightPosition = Vector3.zero;

            foreach (GameObject player in players)
            {
                if(player.transform.position.x > rightPosition.x)
                    rightPosition = player.transform.position;
                else if(player.transform.position.x < leftPosition.x)
                    leftPosition = player.transform.position;
            }

            Vector3 averagePosition = (leftPosition + rightPosition) / 2;

            Vector3 targetPosition = new Vector3(averagePosition.x + offset.x, transform.position.y, transform.position.z);

            //Vector3 smoothedPosition = Vector3.Lerp(transform.position, targetPosition, smoothSpeed);
            //transform.position = smoothedPosition;

            syncedCameraPosition = Vector3.Lerp(transform.position, targetPosition, smoothSpeed);

            transform.position = syncedCameraPosition;
        }
    }
}