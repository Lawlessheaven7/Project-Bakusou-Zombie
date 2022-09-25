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
    public GameObject roomScreen;
    public GameObject errorScreen;

    public TMP_Text loadingText;
    public TMP_InputField roomNameInput;
    public TMP_Text roomText;
    public TMP_Text errorText;

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
        roomScreen.SetActive(false);
        errorScreen.SetActive(false);
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

    //Function to create a room in the lobby.
    public void CreateRoom()
    {
        //if the room name input field is not empty, players are allowed to create a room with the option settings in the scripts.
        if (!string.IsNullOrEmpty(roomNameInput.text))
        {
            RoomOptions options = new RoomOptions();
            options.MaxPlayers = 20;

       
            PhotonNetwork.CreateRoom(roomNameInput.text, options);

            CloseMenus();
            loadingText.text = "Creating Room . . . ";
            loadingScreen.SetActive(true);
        }
    }

    //Actions that happened once player joins the room
    public override void OnJoinedRoom()
    {
        CloseMenus();
        roomScreen.SetActive(true);

        roomText.text = PhotonNetwork.CurrentRoom.Name;
    }

    // WHen creating the room failed
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        errorText.text = "Failed To Create Room: " + message;
        CloseMenus();
        errorScreen.SetActive(true);
    }

    //Error screen utility function to reopen the menu
    public void closeErrorScreen()
    {
        CloseMenus();
        menuButtons.SetActive(true);
    }

    //Leaving the room
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        CloseMenus();
        loadingText.text = "Leaving Room . . . ";
        loadingScreen.SetActive(true);
    }

    public override void OnLeftRoom()
    {
        CloseMenus();
        menuButtons.SetActive(true);
    }
}
