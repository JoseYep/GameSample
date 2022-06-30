using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

public class VariusTesting : MonoBehaviour
{
    [SerializeField] private string chain = "etherium";
    [SerializeField] private string network = "ropstein";
    [SerializeField] private string contract = "";
    [SerializeField] private string abiContract;

    private string rpc = "https://matic-mumbai.chainstacklabs.com/";

    [ContextMenu("Test conversion")]
    public void ConvertGas()
    {
        float wei = float.Parse("2500000012");
        float decimals = 1000000000000000000;
        float eth = wei / decimals;
        string m = Convert.ToDecimal(eth).ToString();

        Debug.Log("Original: " + wei);
        Debug.Log("Converted: " + m);
    }


}
