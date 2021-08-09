using Mirror;
using UnityEngine;

namespace FirstGearGames.Mirrors.ServerCommandConsole
{

    /* //Notes
     * This class is where you will process commands typed into your console.
     * Commands are issued into the Run method below. You may organized and sort
     * them however you please. */
    public class CommandHandler
    {

        public void Run(string command)
        {
            command = command.ToLower();

            //Sample quit command.
            if (command == "quit")
            {
                Debug.Log("Quitting application.");
                Application.Quit();
            }
            //Kick all players.
            else if (command == "kickall")
            {
                Debug.Log("Kicking all clients.");
                foreach (var item in NetworkServer.connections)
                    item.Value.Disconnect();
            }
            //Unknown command.
            else
            {
                Debug.LogWarning("Unhandled command of " + command);
            }
        }

    }


}