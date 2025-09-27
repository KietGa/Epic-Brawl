using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class ChatBox : NetworkBehaviour
{
    [SerializeField] private TMP_InputField chatInput;
    [SerializeField] private TextMeshProUGUI chatContent;

    public void SendChat()
    {
        if (chatInput.text != "")
        {
            ChatServerRpc(chatInput.text);
            chatInput.text = "";
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void ChatServerRpc(FixedString128Bytes message, ServerRpcParams srp = default)
    {
        ChatClientRpc(DataManager.DMInstance.nameList[(int)srp.Receive.SenderClientId], message);
    }

    [ClientRpc]
    private void ChatClientRpc(FixedString128Bytes playerName, FixedString128Bytes message)
    {
        string str = playerName + ": " + message;
        chatContent.text = str + "\n" + chatContent.text;
    }
}
