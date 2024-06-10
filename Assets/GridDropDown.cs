using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GridDropDown : MonoBehaviour
{

    public BeatGen lineGenerator;

    public void HandleInputData(int val)
    {
        if (val == 0)
        {
            lineGenerator.SnapInterval = 10f;
        }
        if (val == 1)
        {
            lineGenerator.SnapInterval = 5f;
        }
        if (val == 2)
        {
            lineGenerator.SnapInterval = 2.5f;
        }
        if (val == 3)
        {
            lineGenerator.SnapInterval = 1.25f;
        }
        if (val == 4)
        {
            lineGenerator.SnapInterval = 0.625f;
        }
        if (val == 5)
        {
            lineGenerator.SnapInterval = 0.3125f;
        }
        if (val == 6)
        {
            lineGenerator.SnapInterval = 0.15625f;
        }
    }
}
