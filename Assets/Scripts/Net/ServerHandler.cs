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

        public static void PlayerMovement(int fromClient, Packet packet) {
            float isMoving = packet.ReadFloat();
            float movementDir = packet.ReadFloat();
            float isJumping = packet.ReadFloat();
            Quaternion rotation = packet.ReadQuaternion();

            Server.clients[fromClient].player.SetInput(movementDir, isMoving, isJumping);
            Server.clients[fromClient].player.rotation = rotation;
        }
    }
}