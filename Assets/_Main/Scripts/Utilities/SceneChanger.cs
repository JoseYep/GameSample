using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneChanger : MonoBehaviour
{
    [SerializeField] private float changeDelay;
    [SerializeField] private Animator transitionAnim;

    private IEnumerator changeSceneC;

    public static SceneChanger instance;

    private void Awake()
    {
        //Singleton creation
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
  
    public void LeaveAndCahngeScene(int sceneIndex)
    {
        PhotonNetwork.LeaveRoom();
        ChangeScene(sceneIndex);
    }
    private IEnumerator DisconnectAndLoad(int indx)
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            TransitionOff();
            PhotonNetwork.LeaveRoom();
            while (PhotonNetwork.InRoom)
                yield return null;

            ChangeScene(indx);
        }
    }

    public void ChangeScene(int sceneIndex)
    {
        TransitionOn();

        if (changeSceneC == null)
        {
            changeSceneC = CallScene(sceneIndex);
            StartCoroutine(changeSceneC);
        }
    }

    private IEnumerator CallScene(int index)
    {
        yield return new WaitForSeconds(changeDelay);
        PhotonNetwork.LoadLevel(index);
        changeSceneC = null;
    }

    private void TransitionOn()
    {
        transitionAnim.SetBool("On", true);
    }

    private void TransitionOff()
    {
        transitionAnim.SetBool("On", false);
    }

    [ContextMenu("Move to game")]
    private void Test()
    {
        ChangeScene(0);
    }
}
