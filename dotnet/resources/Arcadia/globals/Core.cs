using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Text;
using Arcadia;
using Arcadia.model;
using System.Threading;
using System.Threading.Tasks;

namespace Arcadia.globals
{
    //returns target from id
    //Player target = int.TryParse(targetString, out int targetId) ? Globals.GetPlayerById(targetId) : NAPI.Player.GetPlayerFromName(targetString);
    public class Core : Script
    {
        public static List<TeleportModel> teleportsList;

        private Timer minuteTimer;
        private Timer playersCheckTimer;

        public static int GetPlayerLevel(Player target)
        {
            return target.GetSharedData<int>("level");
        }

        public static int GetTotalSeconds()
        {
            return (int)DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
        }

        private void GeneratePayDay (Player player)
        {
            NAPI.Task.Run(() =>
            {
                player.SetSharedData("bank", player.GetSharedData<int>("bank") + 250);
                player.SetSharedData("expirience", player.GetSharedData<int>("expirience") +1);
                if (player.GetSharedData<int>("expirience") >= GetPlayerLevel(player) * 4)
                {
                    player.SetSharedData("level", player.GetSharedData<int>("level") +1);
                    player.SetSharedData("expirience", 0);
                    player.SendChatMessage($"~r~LEVELUP ~w~current level: ~b~{player.GetSharedData<int>("level")}");
                }

                player.SendNotification($"~r~PAYDAY: ~w~Ваш баланс пополнен на ~g~250$, ~w~новый баналс ~g~{player.GetSharedData<int>("cash")}");
                Console.WriteLine($"Player {player.Name} recived payday");
            });
        }

        private void OnMinuteSpent(object unused)
        {
            int totalSeconds = GetTotalSeconds();
            foreach (Player player in NAPI.Pools.GetAllPlayers())
            {
                if (player.HasData("PlayerPlayed") == true)
                {
                    int TimePlayed = player.GetData<int>("PlayerPlaying");
                    if (TimePlayed > 0 && TimePlayed % 1 == 0)
                    {
                        // Generate the payday
                        GeneratePayDay(player);
                    }

                    NAPI.Task.Run(() =>
                    {
                        player.SetData("PlayerPlaying", TimePlayed + 1);
                        player.SetSharedData("totaltimeplayed", player.GetSharedData<int>("totaltimeplayed") + 1);
                    });
                }
            }
        }

        [ServerEvent(Event.ResourceStart)]
        public void OnResourceStart()
        {
            // Permanent timers
            //playersCheckTimer = new Timer(UpdatePlayerList, null, 500, 500);
            minuteTimer = new Timer(OnMinuteSpent, null, 60000, 60000);
        }

        public static Player GetPlayerById(int id)
        {
            Player target = null;
            foreach (Player player in NAPI.Pools.GetAllPlayers())
            {
                if (player.Value == id)
                {
                    target = player;
                    break;
                }
            }
            return target;
        }

        public static string CharacterNameWithId(Player player)
        {
            string CharacterName = $"{player.Name.Replace('_', ' ')}[{player.Id}]";
            return CharacterName;
        }

        public static string CharacterName(Player player)
        {
            string CharacterName = player.Name.Replace('_', ' ');
            return CharacterName;
        }
    }
}
