using UnityEngine;

namespace Net {
    public class ServerHandler : MonoBehaviour {
        public static void WelcomeReceived(int fromClient, Packet packet) {
            int clientIdCheck = packet.ReadInt();
            string username = packet.ReadString();

            Debug.Log(
                $"{Server.clients[fromClient].tcp.socket.Client.RemoteEndPoint} connected successfully and is now player {fromClient}");
            if (fromClient != clientIdCheck) {
                Debug.Log(
                    $"Player \"{username}\" (ID: {fromClient}) has assumed the wrong client ID ({clientIdCheck})");
            }

            Server.clients[fromClient].SendIntoGame(username);
        }
    }
}