using System.Collections.Generic;
using UnityEngine;


public class line : MonoBehaviour
{
    private LineRenderer LR;

    private void Start()
    {
        LR = GetComponent<LineRenderer>();
    }

    public void SetPositions(List<Transform> WinLine)
    {
        if(WinLine == null)
        {
            LR.positionCount = 0;
            return;
        }
        if (LR == null)
            LR = GetComponent<LineRenderer>();
        LR.positionCount = WinLine.Count;

        for (int i = WinLine.Count-1; i >= 0; i--)
        {
            Vector3 pos = WinLine[i].position;
            pos.z = 50;
            LR.SetPosition(i, pos);
        }

    }
}
