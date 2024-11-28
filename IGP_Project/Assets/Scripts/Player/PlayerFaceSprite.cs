using Fusion;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class PlayerFaceSprite : MonoBehaviour
{
    [SerializeField] private Sprite defaultFace;
    [SerializeField] private Sprite deadFace;

    private PlayerController playerController;

    private SpriteRenderer faceSR;

    private void Awake()
    {
        faceSR = GetComponent<SpriteRenderer>();
        faceSR.sprite = defaultFace;

        playerController = this.gameObject.GetComponentInParent<PlayerController>();
    }

    private void Update()
    {
        if (playerController.isPlayerDead)
        {
            faceSR.sprite = deadFace;
        }
    }
}
