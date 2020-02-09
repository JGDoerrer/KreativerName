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
        List<Client> clients = new List<Client>();

        World world;

        public void Join(Client client)
        {
            clients.Add(client);
        }

        public void SendAll(Packet packet)
        {
            foreach (Client client in clients)
            {
                client.Send(packet);
            }
        }
    }
}
