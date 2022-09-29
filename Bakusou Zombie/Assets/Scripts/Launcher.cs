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
    public GameObject roomBrowserScreen;
    public GameObject nameInputScreen;

    public TMP_Text loadingText;
    public TMP_Text roomText, playerNameLabel;
    public TMP_Text errorText;


    public TMP_InputField roomNameInput;
    public TMP_InputField NameInput;

    public RoomButton theRoomButton;

    private List<RoomButton> allRoomButtons = new List<RoomButton>();
    private List<TMP_Text> allPlayerNames = new List<TMP_Text>();

    private bool hasSetUsername;

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
        roomBrowserScreen.SetActive(false);
        nameInputScreen.SetActive(false);
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

        //Temporally player name, will change later
        PhotonNetwork.NickName = Random.Range(0, 1000f).ToString();

        if (!hasSetUsername)
        {
            CloseMenus();
            nameInputScreen.SetActive(true);

            //If there are something stored in the playername, then set the username to the playername value
            if (PlayerPrefs.HasKey("playerName"))
            {
                NameInput.text = PlayerPrefs.GetString("playerName");
            }
        }
        else
        {
            PhotonNetwork.NickName = PlayerPrefs.GetString("playerName");
        }
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

        ListAllPlayers();
    }

    //List all the players, get the information from the network and store it for display.
    private void ListAllPlayers()
    {
        foreach(TMP_Text player in allPlayerNames)
        {
            Destroy(player.gameObject);
        }
        allPlayerNames.Clear();

        Player[] players = PhotonNetwork.PlayerList;
        for(int i = 0; i<players.Length; i++)
        {
            TMP_Text newPlayerLabel = Instantiate(playerNameLabel, playerNameLabel.transform.parent);
            newPlayerLabel.text = players[i].NickName;
            newPlayerLabel.gameObject.SetActive(true);

            allPlayerNames.Clear();
        }

    }

    //When a player joins the room display their name
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        TMP_Text newPlayerLabel = Instantiate(playerNameLabel, playerNameLabel.transform.parent);
        newPlayerLabel.text = newPlayer.NickName;
        newPlayerLabel.gameObject.SetActive(true);

        allPlayerNames.Clear();
    }

    //Refresh and list all current players
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        ListAllPlayers();
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

    //Open the room browser
    public void OpenRoomBrowser()
    {
        CloseMenus();
        roomBrowserScreen.SetActive(true);
    }

    //Close the room browser
    public void CloseRoomBrowser()
    {
        CloseMenus();
        menuButtons.SetActive(true);
    }

    //Called at anytime when there is a change of the room while in the lobby.
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach(RoomButton rb in allRoomButtons)
        {
            Destroy(rb.gameObject);
        }
        allRoomButtons.Clear();

        theRoomButton.gameObject.SetActive(false);

        //Loop through all the roomlist information, then regulate the rooms.
        for(int i = 0; i < roomList.Count; i++)
        {
            //As long as the players aren't full and it hasn't been removed from the list, then display the room.
            if(roomList[i].PlayerCount != roomList[i].MaxPlayers && !roomList[i].RemovedFromList)
            {
                RoomButton newButton = Instantiate(theRoomButton, theRoomButton.transform.parent);
                newButton.SetButtonDetails(roomList[i]);
                newButton.gameObject.SetActive(true);

                allRoomButtons.Add(newButton);
            }
        }
    }
    // Tell Photon Network to let player join the room after clicking the room name
    public void JoinRoom(RoomInfo inputInfo)
    {
        PhotonNetwork.JoinRoom(inputInfo.Name);

        CloseMenus();
        loadingText.text = "Joining Room ";
        loadingScreen.SetActive(true);
    }

    //set player username
    public void setUsername()
    {
        if (!string.IsNullOrEmpty(NameInput.text))
        {
            PhotonNetwork.NickName = NameInput.text;

            //save the username into a string
            PlayerPrefs.SetString("playerName", NameInput.text);

            CloseMenus();
            menuButtons.SetActive(true);

            hasSetUsername = true;
        }
    }

    //Close the build Version of the game, does nothing to the editor version
    public void QuitGame()
    {
        Application.Quit();
    }


}