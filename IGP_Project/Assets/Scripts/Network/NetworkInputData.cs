using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

//script for handle input
public struct NetworkInputData : INetworkInput
{
    public Vector2 direction;
    public float enterPortal;
    public float useItem;
    public int emotion;
}
