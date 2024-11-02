using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputHandler : MonoBehaviour
{
    PlayerController playerController;

    Vector2 inputVector = Vector2.zero;

    // Start is called before the first frame update
    void Start()
    {
        playerController = GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        inputVector = Vector2.zero;

        //inputVector.x = Input.GetAxisRaw("Horizontal");
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            inputVector.x = -1;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            inputVector.x = 1;
        }

        if (Input.GetKey(KeyCode.Space))
        {
            inputVector.y = 1;
        }

        playerController.SetInputVector(inputVector);
    }

    public NetworkInputData GetNetworkInput()
    {
        NetworkInputData networkInputData = new NetworkInputData();
        networkInputData.direction = inputVector;

        return networkInputData;
    }
}
