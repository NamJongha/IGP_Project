using UnityEngine;
using Fusion;

public class CameraFollow : NetworkBehaviour
{
    public string playerTag = "Player";
    public float smoothSpeed = 0f;
    public Vector3 offset;
    public GameObject[] players;
    public Vector3 averagePosition = Vector3.zero;
    public GameObject leftPosition = null;
    public GameObject rightPosition = null;
    public GameObject topPosition = null;
    public GameObject bottomPosition = null;

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

            if (players.Length == 1)
            {
                averagePosition = players[0].transform.position;
            }
            else
            {
                if (leftPosition == null)
                {
                    leftPosition = players[0];
                }
                if (rightPosition == null)
                {
                    rightPosition = players[0];
                }
                if (topPosition == null)
                {
                    topPosition = players[0];
                }
                if (bottomPosition == null)
                {
                    bottomPosition = players[0];
                }
                foreach (GameObject player in players)
                {
                    if (rightPosition.transform.position.x <= player.transform.position.x)
                        rightPosition = player;
                    if (leftPosition.transform.position.x > player.transform.position.x)
                        leftPosition = player;
                    if (topPosition.transform.position.y > player.transform.position.y)
                        topPosition = player;
                    if (bottomPosition.transform.position.y > player.transform.position.y)
                        bottomPosition = player;
                }

                averagePosition.x = (leftPosition.transform.position.x + rightPosition.transform.position.x) / 2;
                averagePosition.y = (topPosition.transform.position.y + bottomPosition.transform.position.y) / 2;
            }
            Vector3 targetPosition = new Vector3(averagePosition.x + offset.x, averagePosition.y, 2);

            //Vector3 smoothedPosition = Vector3.Lerp(transform.position, targetPosition, smoothSpeed);
            //transform.position = smoothedPosition;

            syncedCameraPosition = Vector3.Lerp(transform.position, targetPosition, smoothSpeed);

            transform.position = syncedCameraPosition;
        }
    }
}