using UnityEngine;

namespace Net {

    public class ServerSend : MonoBehaviour {
        private static void SendTCPData(int toClient, Packet packet) {
            packet.WriteLength();
            Server.clients[toClient].tcp.SendData(packet);
        }

        private static void SendTCPDataToAll(Packet packet) {
            packet.WriteLength();
            for (int i = 1; i <= Server.maxPlayers; i++) {
                Server.clients[i].tcp.SendData(packet);
            }
        }

        private static void SendTCPDataToAll(int exceptClient, Packet packet) {
            packet.WriteLength();
            for (int i = 1; i <= Server.maxPlayers; i++) {
                if (i != exceptClient) {
                    Server.clients[i].tcp.SendData(packet);
                }
            }
        }

        private static void SendUDPData(int toClient, Packet packet) {
            packet.WriteLength();
            Server.clients[toClient].udp.SendData(packet);
        }

        private static void SendUDPDataToAll(Packet packet) {
            packet.WriteLength();
            for (int i = 1; i <= Server.maxPlayers; i++) {
                Server.clients[i].udp.SendData(packet);
            }
        }

        private static void SendUDPDataToAll(int exceptClient, Packet packet) {
            packet.WriteLength();
            for (int i = 1; i <= Server.maxPlayers; i++) {
                if (i != exceptClient) {
                    Server.clients[i].udp.SendData(packet);
                }
            }
        }

        #region Packets

        public static void Welcome(int toClient, string msg) {
            using (Packet packet = new Packet((int) ServerPackets.welcome)) {
                packet.Write(msg);
                packet.Write(toClient);

                SendTCPData(toClient, packet);
            }
        }

        public static void SpawnPlayer(int toClient, Player player) {
            using (Packet packet = new Packet((int) ServerPackets.spawnPlayer)) {
                packet.Write(player.id);
                packet.Write(player.username);
                packet.Write(player.transform.position);
                packet.Write(player.transform.rotation);

                SendTCPData(toClient, packet);
            }
        }

        public static void PlayerPosition(Player player) {
            using (Packet packet = new Packet((int) ServerPackets.playerPosition)) {
                packet.Write(player.id);
                packet.Write(player.transform.position);

                SendUDPDataToAll(packet);
            }
        }

        public static void PlayerDisconnected(int playerID) {
            using (Packet packet = new Packet((int) ServerPackets.playerDisconnected)) {
                packet.Write(playerID);

                SendTCPDataToAll(packet);
            }
        }
        #endregion
    }
}