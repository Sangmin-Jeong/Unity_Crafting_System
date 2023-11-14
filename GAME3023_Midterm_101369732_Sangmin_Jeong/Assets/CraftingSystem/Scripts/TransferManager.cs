using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransferManager : MonoBehaviour
{
    public static TransferManager Instance;
    public ItemSlot _targetSlot;

    private void Start()
    {
        Instance = this;
    }
}
