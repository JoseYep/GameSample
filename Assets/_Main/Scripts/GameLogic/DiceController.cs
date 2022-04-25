using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DiceData
{
    [SerializeField] private int number;
    [SerializeField] private Sprite sprite;

    public int Number { get => number; set => number = value; }
    public Sprite Sprite { get => sprite; set => sprite = value; }
}

public class DiceController : MonoBehaviour
{
    [Header("Options pool")]
    [SerializeField] private DiceData[] dicesData;

    [Header("Settings")]
    [SerializeField] private SpriteRenderer sRenderer;

    private int diceResult;
    private bool roll = false;
    private float count = 0;
    private float rollTime = 0;

    private void Update()
    {
        if (roll)
        {
            count++;
            int randFace = Random.Range(1, dicesData.Length + 1);
            sRenderer.sprite = GetFace(randFace);
            if (count >= rollTime)
            {
                count = 0;
                sRenderer.sprite = GetFace(diceResult);
                roll = false;
            }
        }
    }

    public void Roll(int resul, float rollingTime)
    {
        diceResult = resul;
        rollTime = rollingTime;
        roll = true;
    }

    public Sprite GetFace(int resul)
    {
        foreach (DiceData data in dicesData)
        {
            if (data.Number == resul)
            {
                return data.Sprite;
            }
        }

        return null;
    }
 
    [ContextMenu("Test dice roll")]
    private void TestRoll()
    {
        Roll(Random.Range(1, dicesData.Length + 1), 5);
    }
}
