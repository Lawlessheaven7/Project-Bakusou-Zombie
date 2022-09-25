using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;


public class Launcher : MonoBehaviourPunCallbacks
{

    public static Launcher instance;

    private void Awake()
    {
        instance = this;
    }

    public GameObject loadingScreen;
    public GameObject menuButtons;
    public GameObject createRoomScreen;

    public TMP_Text loadingText;
    public TMP_InputField roomNameInput;


    // Start is called before the first frame update
    void Start()
    {
        CloseMenus();

        loadingScreen.SetActive(true);
        loadingText.text = "Connecting To Network . . . ";

        //Connect to the Photon Server using our custom settings.
        PhotonNetwork.ConnectUsingSettings();
    }

    //Function to control the UI element, called in the Start() to have all the unecessary UI set to false.
    void CloseMenus()
    {
        loadingScreen.SetActive(false);
        menuButtons.SetActive(false);
        createRoomScreen.SetActive(false);
    }

    //Action that happens after connecting to the master server
    public override void OnConnectedToMaster()
    {

        PhotonNetwork.JoinLobby(); //Once connected to the master immediately join the Lobby.

        loadingText.text = "Joining Lobby . . .";
    }

    //Action that happens after joining the lobby
    public override void OnJoinedLobby()
    {
        CloseMenus();
        menuButtons.SetActive(true);
    }

    public void OpenRoomCreate()
    {
        CloseMenus();
        createRoomScreen.SetActive(true);
    }


}
