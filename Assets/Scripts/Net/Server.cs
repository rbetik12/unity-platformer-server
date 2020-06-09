using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEditor.PackageManager;
using UnityEngine;

namespace Net {
    public class Server: MonoBehaviour {
        public static int maxPlayers { get; private set; }
        public static int port { get; private set; }
        public static Dictionary<int, Client> clients = new Dictionary<int, Client>();

        public delegate void PacketHandler(int fromClient, Packet packet);

        public static Dictionary<int, PacketHandler> packetHandlers;

        private static TcpListener _tcpListener;
        private static UdpClient _udpListener;
        
        public static void Start(int maxPlayers, int port) {
            Server.maxPlayers = maxPlayers;
            Server.port = port;

            Debug.Log("Starting server...");
            InitializeServerData();

            _tcpListener = new TcpListener(IPAddress.Any, Server.port);
            _tcpListener.Start();
            _tcpListener.BeginAcceptTcpClient(TCPConnectCallback, null);

            _udpListener = new UdpClient(Server.port);
            _udpListener.BeginReceive(UDPReceiveCallback, null);

            Debug.Log($"Server started on port {Server.port}.");
        }
        
        private static void TCPConnectCallback(IAsyncResult result) {
            TcpClient client = _tcpListener.EndAcceptTcpClient(result);
            _tcpListener.BeginAcceptTcpClient(TCPConnectCallback, null);
            Debug.Log($"Incoming connection from {client.Client.RemoteEndPoint}...");

            for (int i = 1; i <= maxPlayers; i++) {
                if (clients[i].tcp.socket == null) {
                    clients[i].tcp.Connect(client);
                    return;
                }
            }

            Debug.Log($"{client.Client.RemoteEndPoint} failed to connect: Server full!");
        }
        
        private static void UDPReceiveCallback(IAsyncResult result) {
            try {
                IPEndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = _udpListener.EndReceive(result, ref clientEndPoint);
                _udpListener.BeginReceive(UDPReceiveCallback, null);

                if (data.Length < 4) {
                    return;
                }

                using (Packet packet = new Packet(data)) {
                    int clientId = packet.ReadInt();

                    if (clientId == 0) {
                        return;
                    }

                    if (clients[clientId].udp.endPoint == null) {
                        // If this is a new connection
                        clients[clientId].udp.Connect(clientEndPoint);
                        return;
                    }

                    if (clients[clientId].udp.endPoint.ToString() == clientEndPoint.ToString()) {
                        // Ensures that the client is not being impersonated by another by sending a false clientID
                        clients[clientId].udp.HandleData(packet);
                    }
                }
            }
            catch (Exception ex) {
                Debug.Log($"Error receiving UDP data: {ex}");
            }
        }
        
        public static void SendUDPData(IPEndPoint clientEndPoint, Packet packet) {
            try {
                if (clientEndPoint != null) {
                    _udpListener.BeginSend(packet.ToArray(), packet.Length(), clientEndPoint, null, null);
                }
            }
            catch (Exception ex) {
                Debug.Log($"Error sending data to {clientEndPoint} via UDP: {ex}");
            }
        }
        
        private static void InitializeServerData() {
            for (int i = 1; i <= maxPlayers; i++) {
                clients.Add(i, new Client(i));
            }

            packetHandlers = new Dictionary<int, PacketHandler>() {
                { (int) ClientPackets.welcomeReceived, ServerHandler.WelcomeReceived },
                { (int) ClientPackets.playerMovement, ServerHandler.PlayerMovement },
            };
            Debug.Log("Initialized packets.");
        }
        
        public static void Stop() {
            _tcpListener.Stop();
            _udpListener.Close();
        }
    }
}