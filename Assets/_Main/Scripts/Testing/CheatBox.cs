using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CheatBox : MonoBehaviour
{
    [SerializeField] private TMP_InputField winnerIF;
    [SerializeField] private TMP_InputField loserIF;
    [SerializeField] private GameSmartContract gameSmartContract;
    [SerializeField] private Button create;
    [SerializeField] private Button join;
    [SerializeField] private Button claim;

    private void Start()
    {
        create.onClick.AddListener(() => Create());
        join.onClick.AddListener(() => Join());
        claim.onClick.AddListener(() => Claim());
    }

    public void Claim()
    {
        gameSmartContract.ClaimReward(winnerIF.text, loserIF.text);
    }

    public void Create()
    {
        gameSmartContract.CreateGame(10000000000);
    }

    public void Join()
    {
        gameSmartContract.JoinGame(10000000000);
    }
}
