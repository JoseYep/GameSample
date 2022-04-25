using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIRoundHandler : MonoBehaviour
{
    [SerializeField] private Transform parent;

    public void SetResult(int round, bool winner)
    {
        if (round < parent.childCount)
        {
            Image image = parent.GetChild(round).gameObject.GetComponent<Image>();
            if (image)
            {
                if (winner)
                {
                    image.color = Color.green;
                }
                else
                {
                    image.color = Color.red;
                }
            }
        }       
    }
}
