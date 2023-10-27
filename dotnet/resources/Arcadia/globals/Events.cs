using Arcadia.model;
using GTANetworkAPI;
using System;
using System.IO;
using Newtonsoft.Json;
using Arcadia.globals;
using System.Threading;
using System.Globalization;
using System.Collections.Generic;
using Arcadia.database;
using System.Linq;

namespace Arcadia.globals
{
	class Events : Script
	{
		private static Random Rnd = new Random();

		[ServerEvent(Event.PlayerConnected)]
		public void OnPlayerConnected(Player player)
		{
			Console.WriteLine($"[INFO] New connection - Name: {player.Name}, Social Club [Name: {player.SocialClubName}] [ID:{player.SocialClubId}], IP: {player.Address}");
			player.Name = string.Empty;
			player.SetSkin(71929310);
			player.RemoveAllWeapons();
			player.Transparency = 255;
			player.Health = 100;
			player.SetData<int>("PasswordErrors", 0);
			player.Dimension = 1;
			player.TriggerEvent("unfreeze");
		}

		[ServerEvent(Event.PlayerDisconnected)]
		public void OnPlayerDisconnected(Player player, DisconnectionType type, string reason)
		{
			foreach (Vehicle item in NAPI.Pools.GetAllVehicles()) // some shit
            {
				if (item.GetData<string>("VehOwner") == player.Name)
					item.Delete();
			}

			Console.WriteLine($"[INFO] Player disconected. Type {type}, reason {reason}");
			if (!CharacterModel.IsCharacterLoggedIn(player)) return;

			CharacterModel character = new CharacterModel();
			character.SqlId = player.GetData<int>("sqlid");
			character.Account = player.GetData<string>("account");
			character.FirstName = player.GetData<string>("firstname");
			character.SecondName = player.GetData<string>("secondname");
			character.Level = player.GetSharedData<int>("level");
			character.Expirience = player.GetSharedData<int>("expirience");
			character.Gender = player.GetData<string>("gender");
			character.Skin = player.GetData<uint>("skin");
			character.AdminLevel = player.GetData<int>("adminlevel");
			character.Cash = player.GetSharedData<int>("cash");
			character.Bank = player.GetSharedData<int>("bank");
			character.Faction = player.GetData<int>("faction");
			character.Rank = player.GetData<int>("rank");
			character.Health = player.Health;
			character.Armor = player.Armor;
			character.TotalTimePlayed = player.GetSharedData<int>("totaltimeplayed");
			character.PosX = player.Position.X;
			character.PosY = player.Position.Y;
			character.PosZ = player.Position.Z;

			character.Disconect();
		}

		[ServerEvent(Event.ResourceStart)]
		public void OnResourceStart()
		{
			NAPI.Server.SetDefaultSpawnLocation(new Vector3(788.5620f + Rnd.Next(0, 5), 1278.1571f + Rnd.Next(0, 5), 360.2968f + Rnd.Next(0, 5)), 200.0f);
			NAPI.Server.SetAutoRespawnAfterDeath(false);
			NAPI.Server.SetCommandErrorMessage("Unknown command, use /help for more info.");
			NAPI.World.SetTime(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
			//NAPI.Server.SetGlobalServerChat(false);
			
		}

		[ServerEvent(Event.PlayerDeath)]
		public void OnPlayerDeath(Player player, Player killer, uint reason)
		{
			NAPI.Task.Run(() =>
			{
				NAPI.Player.SpawnPlayer(player, new Vector3(317.50003f, -1376.808f, 31.928207f));
				player.Health = 20;
			}, delayTime: 4000);
		}

		[RemoteEvent("fpsync.update")]
		public static void FingerPoint(Player sender, float camPitch, float camHeading)
		{
			NAPI.ClientEvent.TriggerClientEventInRange(sender.Position, 100, "fpsync.update", sender.Handle, camPitch, camHeading);
		}
    }
}
