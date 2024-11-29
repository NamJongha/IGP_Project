using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerInputHandler : MonoBehaviour
{
    PlayerController playerController;
    ShowEmotion playerEmotion;

    Vector2 inputVector = Vector2.zero;
    float portalInput = 0;
    float useItemInput = 0;
    float emotionInput = 0;

    // Start is called before the first frame update
    void Start()
    {
        playerController = GetComponent<PlayerController>();
        playerEmotion = GetComponent<ShowEmotion>();
    }

    //Recognition of player's input
    void Update()
    {
        inputVector = Vector2.zero;
        portalInput = 0;
        useItemInput = 0;
        emotionInput = 0;

        //inputVector.x = Input.GetAxisRaw("Horizontal");
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            Debug.Log("PressedLeft");
            inputVector.x = -1;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            inputVector.x = 1;
        }

        //jump
        if (Input.GetKey(KeyCode.Space))
        {
            inputVector.y = 1;
        }

        //enter portal
        if (Input.GetKey(KeyCode.DownArrow))
        {
            portalInput = 1;
        }
        else if (Input.GetKey(KeyCode.UpArrow))
        {
            portalInput = 2;
        }

        //use Item
        if (Input.GetKey(KeyCode.A))
        {
            useItemInput = 1;
        }

        if (Input.GetKey(KeyCode.D))
        {
            useItemInput = 2;
        }

        if (Input.GetKey(KeyCode.Alpha1))
        {
            emotionInput = 1;
        }

        if (Input.GetKey(KeyCode.Alpha2))
        {
            emotionInput = 2;
        }

        if (Input.GetKey(KeyCode.Alpha3))
        {
            emotionInput = 3;
        }

        playerController.SetInputVector(inputVector);
        playerController.SetEnterPortal(portalInput);
        playerController.SetUseItem(useItemInput);
        playerEmotion.SetEmotion(emotionInput);
    }

    //send player's input data to host
    public NetworkInputData GetNetworkInput()
    {
        NetworkInputData networkInputData = new NetworkInputData();
        networkInputData.direction = inputVector;
        networkInputData.enterPortal = portalInput;
        networkInputData.useItem = useItemInput;
        networkInputData.emotion = emotionInput;

        return networkInputData;
    }
}
