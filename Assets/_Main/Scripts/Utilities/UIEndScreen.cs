using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class UIEndScreen : MonoBehaviour
{
    [SerializeField] private GameObject content;
    [SerializeField] private GameSmartContract smartContract;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private Button endButton;
    [SerializeField] private TextMeshProUGUI buttonText;

    private string winner;
    private string loser;

    public string Winner { get => winner; set => winner = value; }
    public string Loser { get => loser; set => loser = value; }

    public void EndMatch(bool winner)
    {
        endButton.onClick.RemoveAllListeners();
        content.SetActive(true);

        if (winner)
        {
            titleText.text = "YOU WIN";
            buttonText.text = "CLAIM REWARD";
            endButton.onClick.AddListener(() => Claim());

            if (SceneChanger.instance)
            {
                smartContract.onComplete += Quit;
            }
        }
        else
        {
            titleText.text = "YOU LOSE";
            buttonText.text = "EXIT";

            if (SceneChanger.instance)
            {
                endButton.onClick.AddListener(() => Quit());
            }
        }
    }

    private void Claim()
    {
        smartContract.Claim(winner, loser);
        smartContract.onComplete += Quit;
    }
    
    private void Quit()
    {
        SceneChanger.instance.LeaveAndCahngeScene(0);
        smartContract.onComplete -= Quit;
    }
}
