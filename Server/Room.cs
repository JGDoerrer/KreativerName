using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KreativerName.Grid;
using KreativerName.Networking;

namespace Server
{
    public class Room
    {
        public Room()
        { }
        public Room(uint id)
        {
            ID = id;
        }

        public List<Client> Clients = new List<Client>();
        public uint ID;

        World world;

        public void Join(Client client)
        {
            Clients.Add(client);
        }

        public void SendAll(Packet packet)
        {
            foreach (Client client in Clients)
            {
                client.Send(packet);
            }
        }
    }
}
