using ExitGames.Client.Photon;
using Photon.Chat;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace DIPProject {
	public class PhotonChatManager : MonoBehaviour, IChatClientListener
	{
		#region Variables
		ChatClient chatClient;
		[Tooltip("The InputField for sending a message")]
		[SerializeField] InputField chatInput;
		[Tooltip("The TextWindow")]
		[SerializeField] Text chatWindow;
		[Tooltip("The Button to send a message")]
		[SerializeField] Button chatBtn;
		#endregion

		#region IChatClientListener Callbacks

		public void DebugReturn(DebugLevel level, string message) { }

		public void OnChatStateChange(ChatState state) { }

		public void OnConnected()
		{
			chatClient.Subscribe(new string[] { PhotonNetwork.CurrentRoom.Name });
			ToggleChat(true);
		}

		public void OnDisconnected()
		{
			chatClient.Unsubscribe(new string[] { PhotonNetwork.CurrentRoom.Name });
			ToggleChat(false);
		}

		public void OnGetMessages(string channelName, string[] senders, object[] messages)
		{
			if (channelName == PhotonNetwork.CurrentRoom.Name) {
				AppendChat(senders, messages);
				CreateChatPopUp(senders, messages);
			}
		}

		public void OnPrivateMessage(string sender, object message, string channelName)
		{
			//throw new System.NotImplementedException();
		}

		public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
		{
			//throw new System.NotImplementedException();
		}

		public void OnSubscribed(string[] channels, bool[] results)
		{
			MsgRoom(PhotonNetwork.NickName + " has connected to the channel!");
		}

		public void OnUnsubscribed(string[] channels)
		{
			MsgRoom(PhotonNetwork.NickName + " has disconnected from the channel!");
		}

		public void OnUserSubscribed(string channel, string user)
		{
			Debug.Log(user + " has subscribed to " + channel);
		}

		public void OnUserUnsubscribed(string channel, string user)
		{
			Debug.Log(user + " has unsubscribed from " + channel);
		}

		#endregion

		#region MonoBehavior Callbacks

		// Start is called before the first frame update
		void Start()
		{
			ToggleChat(false);
		}

		// Update is called once per frame
		void Update()
		{
			if (PhotonNetwork.InRoom)
			{
				if (chatClient == null) Connect();

				if (Input.GetKeyDown(KeyCode.Return)) SendChatMsg();
				chatClient.Service();
			}

		}

		#endregion

		#region Public Methods
		public void Connect()
		{
			Debug.Log("Photon Chat attempting to Connect");
			chatClient = new ChatClient(this);
			chatClient.Connect(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat, PhotonNetwork.AppVersion, new AuthenticationValues(PhotonNetwork.NickName));
		}

		void MsgRoom(string msg)
		{
			bool result = chatClient.PublishMessage(PhotonNetwork.CurrentRoom.Name, msg);

			Debug.Log("Attempting to send message " + msg + " to " + PhotonNetwork.CurrentRoom.Name);
			if (!result) Debug.Log("Failed to send message");
		}

		public void SendChatMsg()
		{
			if (chatInput.text.Trim() == "") return;
			MsgRoom(chatInput.text);
			chatInput.text = "";
		}

		void ToggleChat(bool status)
		{
			chatBtn.enabled = status;
			chatInput.enabled = status;
			chatWindow.enabled = status;
		}

		public void AppendChat(string[] senders, object[] messages)
		{
			for (int i = 0; i < senders.Length; i++)
			{
				chatWindow.text += "\n" + senders[i] + ": " + messages[i];
			}
		}

		public void CreateChatPopUp(string[] senders, object[] messages)
		{
			GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
			foreach (GameObject player in players)
			{
				for (int i = 0; i < senders.Length; i ++)
				{
					if (player.GetPhotonView().Owner.NickName == senders[i])
						StartCoroutine(player.GetComponent<Player2D>().ChatPopup(messages[i]));
				}
			}
		}

		#endregion
	}
}