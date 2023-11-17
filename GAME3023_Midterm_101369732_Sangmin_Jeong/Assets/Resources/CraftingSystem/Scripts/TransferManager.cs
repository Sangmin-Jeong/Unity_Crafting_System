using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransferManager : MonoBehaviour
{
    public static TransferManager Instance { get; private set; }
    public ItemSlot _targetSlot;

    public int ROW;
    public int COLUMN;

    private void Start()
    {
        Instance = this;
    }
}
