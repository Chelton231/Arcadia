using GTANetworkAPI;
using System;
using System.IO;
using Newtonsoft.Json;
using Arcadia.model;
using Arcadia.globals;
using System.Threading;
using System.Globalization;
using System.Collections.Generic;
using Arcadia.database;
using System.Linq;
using System.Data;

namespace Arcadia.admin
{
    class ACommands : Script
    {
        private static Random Rnd = new Random();

       /* public static void PayDay(Player player)
        {
            player.SetSharedData("cash", player.GetSharedData<int>("cash") + 250);
            player.SendChatMessage($"~b~[BANK] PAYDAY: ~w~Ваш баланс пополнен на ~g~250$, ~w~новый баналс ~g~{player.GetSharedData<int>("cash")}");
            Console.WriteLine($"Player {player.Name} recived payday");
        }*/

        public static bool DoesPlayerAccessToAdminCommand(Player sender, int level)
        {
            if (sender.GetData<int>("adminlevel") < level)
            {
                //sender.SendChatMessage($"Access denied. The command is available from level 5.");
                return false;
            }
            return true;
        }

        [Command("a", GreedyArg = true)]
        public void CommandAdminChat(Player sender, string message)
        {
            if (!CharacterModel.IsCharacterLoggedIn(sender)) return;
            DoesPlayerAccessToAdminCommand(sender, 1);

            foreach (Player player in NAPI.Pools.GetAllPlayers())
            {
                if (player.GetData<int>("adminlevel") >= 0)
                    player.SendChatMessage($"~r~[A] {Core.CharacterName(sender)}~w~: {message}");
            }
        }

        [Command("getpos", GreedyArg = true)]
        public void CommandGetPlayerPosition(Player sender, string positionName = "")
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

            if (positionName == "")
                NAPI.Chat.SendChatMessageToPlayer(sender, "[X: " + sender.Position.X + "] [Y: " + sender.Position.Y + "] [Z: " + sender.Position.Z + "]" + sender.Rotation.Z);
            else
            {
                var pos = (sender.IsInVehicle) ? sender.Vehicle.Position : sender.Position;
                var rot = (sender.IsInVehicle) ? sender.Vehicle.Rotation : sender.Rotation;

                using (var stream = File.AppendText("SavePos.txt"))
                {
                    if (sender.IsInVehicle)
                        stream.WriteLine($"[VEHICLE] {positionName} = [X: {pos.X}] [Y: {pos.Y}] [Z: {pos.Z}] [ROTATION: {rot.X} {rot.Y} {rot.Z}]");
                    else
                        stream.WriteLine($"[PLAYER] {positionName} = [X: {pos.X}] [Y: {pos.Y}] [Z: {pos.Z}] [ROTATION: {rot.X} {rot.Y} {rot.Z}]");
                    stream.Close();
                }
            }
        }

   //     [Command("savetp")]
   //     public void CommandSaveTeleport(Player sender, string tpname)
   //     {
   //         if (tpname == "list")
			//{
   //             sender.SendChatMessage("You can't use rhis word.");
   //             return;
			//}

   //         TeleportModel place = new TeleportModel
   //         {
   //             X = sender.Position.X,
   //             Y = sender.Position.Y,
   //             Z = sender.Position.Z,
   //             Creator = sender.Name
   //         };

   //         //serialize JSON to a string and then write string to a file
   //         File.WriteAllText(@$"C:\RAGEMP\server-files\tp points\{tpname}.json", JsonConvert.SerializeObject(place));
   //         sender.SendNotification($"~b~Succsess\n~w~New teleport point: {tpname}!");
   //     }

   //     [Command("tpdel")]
   //     public void CommandDeleteTeleportPoint(Player sender, string tpname)
   //     {
   //         File.Delete(@$"C:\RAGEMP\server-files\tp points\{tpname}.json");
   //         sender.SendNotification($"~b~Succsess\n~w~Teleport point ~g~{tpname} ~w~was deleted!");
   //     }

        [Command("tp")]
        public void CommandTeleport(Player sender, string pointName)
        {
            if (pointName == "list")
            {
                string message = $"~r~[A] ~w~Доступные телепорты:{Environment.NewLine}";
                foreach (TeleportModel item in Core.teleportsList)
                {
                    message += $"~b~{item.Name} ~w~- ";
                }
                //message = message[..^1];
                sender.SendChatMessage($"{message}");
            }
            else
            {
                if (!Core.teleportsList.Exists(x => x.Name == pointName))
                {
                    sender.SendChatMessage($"~r~[A] ~w~Телепорта ~b~{pointName} ~w~не существует.");
                    return;
                }

                foreach(TeleportModel item in Core.teleportsList)
                {
                    if (item.Name == pointName)
                    {
                        if (sender.IsInVehicle)
                        {
                            Vehicle vehicle = sender.Vehicle;
                            vehicle.Position = new Vector3(item.X, item.Y, item.Z);
                            sender.Position = new Vector3(item.X, item.Y, item.Z);
                            sender.SetIntoVehicle(vehicle, 0);
                        }
                        sender.Position = new Vector3(item.X, item.Y, item.Z);
                    }
                }
            }
        }

        [Command("tpinfo")]
        public void CmdTpIfo(Player sender, string pointname)
        {
            foreach (TeleportModel item in Core.teleportsList)
            {
                if (item.Name == pointname)
                {
                    sender.SendChatMessage($"~r~[A] Point info: ~b~Name: ~w~{item.Name}, ~b~Creator: ~w~{item.Creator}, ~b~DateTime: ~w~{item.DateTime}");
                }
            }
        }

        [Command("savetp")]
        public void CommandSaveNewJumpPoint (Player sender, string pointName)
        {
            if (Core.teleportsList.Exists(x => x.Name == pointName))
            {
                sender.SendChatMessage($"{Messages.MESSAGE_INFO} ~w~ Точка телепорта ~b~{pointName} ~w~уже существует.");
                return;
            }

            if (pointName == "list")
            {
                sender.SendChatMessage($"~r~[A] ~w~Данное имя нельзя использовать.");
                return;
            }

            TeleportModel jumpPoints = new TeleportModel();
                          jumpPoints.Name = pointName;
                          jumpPoints.X = sender.Position.X;
                          jumpPoints.Y = sender.Position.Y;
                          jumpPoints.Z = sender.Position.Z;
                          jumpPoints.Creator = sender.Name;
                          jumpPoints.DateTime = $"{DateTime.Now.Day}-{DateTime.Now.Month}-{DateTime.Now.Year} {DateTime.Now.Hour}:{DateTime.Now.Minute}:{DateTime.Now.Second}";

            Core.teleportsList.Add(jumpPoints);

            sender.SendChatMessage($"~r~[A] ~w~Точка телепорта ~b~{pointName} ~w~успешно создана.");
        }

        [Command("tpdel")]
        public void CommandDeleteJumpPoint(Player sender, string pointName)
        {
            if(!Core.teleportsList.Exists(x => x.Name == pointName))
            {
                sender.SendChatMessage($"{Messages.MESSAGE_INFO} ~w~ Точки телепорта~b~{pointName} ~w~не существует.");
                return;
            }

            foreach (TeleportModel item in Core.teleportsList)
            {
                if (item.Name == pointName)
                {
                    Core.teleportsList.Remove(item);
                    Database.DeleteJumpPoint(pointName);
                    sender.SendChatMessage($"~r~[A] ~w~ Точка телепорта ~b~{pointName} ~w~успешно удалена.");
                    return;
                }
            }
        }

        [Command("apply")]
        public void StopCommand(Player sender)
        {
            Database.UpdateAllJumpPoints(Core.teleportsList);
            sender.SendChatMessage($"~g~Success");
            Console.WriteLine($"[SAVE] {Core.teleportsList.Count} jump points was saved.");
        }

        [Command("getstats")] // криво
        public void CommandGetPlayerStats(Player sender, string target)
        {
            if (!AccountModel.IsAccountLoggedIn(sender)) return;
            Player targetString = int.TryParse(target, out int targetId) ? Core.GetPlayerById(targetId) : NAPI.Player.GetPlayerFromName(target);

            if (target != null)
            {
                CharacterModel character = new CharacterModel();
                character.Level = targetString.GetSharedData<int>("level");
                character.Account = targetString.GetData<string>("account");
                character.Expirience = targetString.GetSharedData<int>("expirience");
                character.Gender = targetString.GetData<string>("gender");
                character.Cash = targetString.GetSharedData<int>("cash");
                character.Bank = targetString.GetSharedData<int>("bank");
                character.Faction = targetString.GetData<int>("faction");
                character.TotalTimePlayed = targetString.GetSharedData<int>("totaltimeplayed");


                sender.SendChatMessage($"~b~{Core.CharacterName(targetString)} ~w~statistics:");
                sender.SendChatMessage($"Level: ~b~{character.Level}~w~, Expirience ~b~{character.Expirience}~w~, Gender ~b~{character.Gender}, ~w~Faction ~b~{character.Faction}");
                sender.SendChatMessage($"~w~AdminLevel ~b~{character.AdminLevel}~w~, Cash ~b~{character.Cash}~w~, Bank ~b~{character.Bank}, TTP ~b~{character.TotalTimePlayed}");
            }
            else sender.SendChatMessage(Messages.PLAYER_NOT_FOUND);
        }

        [Command("ahelp")] //hyita
        public void CommandGetAdminCommands(Player sender)
		{
            string[] commands = NAPI.Resource.GetResourceCommands("arcadia");
            string message = "";
            foreach (string cmd in commands)
			{
                message += cmd.Split(' ');
			}
            sender.SendChatMessage($"{message}");
        }

        [Command("veh")]
        public void CommandSpawnVehicle(Player sender, VehicleHash model, int color1 = -1, int color2 = -1)
        {
            if (!AccountModel.IsAccountLoggedIn(sender)) return;

            //if (!doesplayeraccesstoadmincommand(sender, 0))
			//{
            //    sender.sendchatmessage($"access denied. the command is available from level 5.");
            //    return;
            //}

            if (color1 == -1 && color2 == -1)
            {
                color1 = Rnd.Next(0, 158);
                color2 = Rnd.Next(0, 158);
            }

            DeleteDuplicateVehicle(sender);

            Vehicle vehicle = NAPI.Vehicle.CreateVehicle(model, sender.Position, sender.Rotation, color1, color2);
            vehicle.SetData("VehOwner", sender.Name);
            vehicle.SetData("Fuel", 60);
            vehicle.NumberPlate = "stuff";

            NAPI.Task.Run(() =>
            {
                sender.SetIntoVehicle(vehicle, 0);
            }, delayTime: 100);
        }

        [Command("dveh")]
        public void CommandDeleteCreatedVehicle(Player sender)
        {
            DeleteDuplicateVehicle(sender);
        }

        public void DeleteDuplicateVehicle(Player sender)
		{
            foreach (Vehicle item in NAPI.Pools.GetAllVehicles())
            {
                if (item.GetData<string>("VehOwner") == sender.Name)
                    item.Delete();
            };
        }

        [Command("gveh")]
        public void CommandSpawnVehiclesAsGrid(Player sender, VehicleHash model, int cols, int rows)
        {
            Vector3 pos = sender.Position;
            pos.X += (2 * (float)Math.Sin(-sender.Rotation.Z * Math.PI / 180.0));
            pos.Y += (2 * (float)Math.Cos(-sender.Rotation.Z * Math.PI / 180.0));

            for (int a = 0; a < rows; a++)
            {
                for (int i = 0; i < cols; i++)
                {
                    Vehicle vehicle = NAPI.Vehicle.CreateVehicle(model, pos, sender.Rotation, Rnd.Next(0, 158), Rnd.Next(0, 158));
                    vehicle.SetData("GridVehicle", sender.Name);

                    pos.X += 4 * (float)Math.Cos(sender.Rotation.Z * Math.PI / 180.0);
                    pos.Y += 4 * (float)Math.Sin(sender.Rotation.Z * Math.PI / 180.0);
                }

                pos = sender.Position;
                pos.X += (6 * (a + 1) * (float)Math.Sin(-sender.Rotation.Z * Math.PI / 180.0));
                pos.Y += (6 * (a + 1) * (float)Math.Cos(-sender.Rotation.Z * Math.PI / 180.0));
            }
        }

        [Command("dgveh")]
        public void CommandDeleteAllGridVehicles(Player sender, bool owner = true)
        {
            foreach (Vehicle item in NAPI.Pools.GetAllVehicles())
            {
                if (owner == true)
                {
                    if (item.GetData<string>("GridVehicle") == sender.Name)
                        item.Delete();
                }
                if (item.HasData("GridVehicle"))
                    item.Delete();
            };
        }

        [Command("fixveh")]
        public void CommandRepaierVehicle(Player sender, string targetString = "sender")
        {
            if (targetString == "sender")
            {
                if (sender.IsInVehicle)
                {
                    Vehicle vehicle = sender.Vehicle;
                    vehicle.Repair();
                    vehicle.Health = 100;
                    sender.SendChatMessage($"~r~[A] ~w~Вы починили своё транспортное средство.");
                }
                else 
                    sender.SendChatMessage($"~r~[A] ~w~Необходимо находиться в транспортном средстве.");
            }
            else
            {
                Player target = int.TryParse(targetString, out int targetId) ? Core.GetPlayerById(targetId) : NAPI.Player.GetPlayerFromName(targetString);

                if (target != null)
                {
                    if (target.IsInVehicle)
                    {
                        Vehicle vehicle = target.Vehicle;
                        vehicle.Repair();
                        vehicle.Health = 100;
                        target.SendChatMessage($"~r~[A] ~b~{Core.CharacterName(sender)} ~w~починил Ваше транспортное средство.");
                    }
                    else
                    {
                        target.SendChatMessage("~o~[info]~w~ Необходимо находиться в транспортном средстве, что бы администратор смог починить его Вам.");
                        sender.SendChatMessage($"~o~[info]~w~ Игрок ~b~{Core.CharacterNameWithId(target)} ~w~находится вне транспортного средства.");
                    }
                }
                else sender.SendChatMessage(Messages.PLAYER_NOT_FOUND);
            }
        }

        [Command("sethp")]
        public void CommandSetHP(Player sender, string targetString, int health = 100)
        {
            Player target = int.TryParse(targetString, out int targetId) ? Core.GetPlayerById(targetId) : NAPI.Player.GetPlayerFromName(targetString);

            if (target != null)
            {
                target.Health = health;
                target.SendChatMessage($"~r~[A] ~b~{Core.CharacterName(sender)}~w~ set to you ~t~({health}) ~w~health.");
                sender.SendChatMessage($"~r~[A] ~w~You set to ~t~{Core.CharacterNameWithId(target)}~t~ ({health}) ~w~health points.");
            }
            else sender.SendChatMessage(Messages.PLAYER_NOT_FOUND);
        }

        [Command("forward")]
        public void CommandGoForward(Player sender, float distance)
        {
            Vector3 pos = sender.Position;
            pos.X += distance * (float)Math.Sin(-sender.Rotation.Z * Math.PI / 180.0);
            pos.Y += distance * (float)Math.Cos(-sender.Rotation.Z * Math.PI / 180.0);

            sender.Position = new Vector3(pos.X, pos.Y, sender.Position.Z);
        }

        [Command("setmoney")]
        public void SetCharacterMoney(Player sender, string target, int amount)
		{
            Player targetString = int.TryParse(target, out int targetId) ? Core.GetPlayerById(targetId) : NAPI.Player.GetPlayerFromName(target);
            if (!CharacterModel.IsCharacterLoggedIn(targetString)) return;

            if (target != null)
            {
                targetString.SetSharedData("cash", targetString.GetSharedData<int>("cash") + amount);
                //targetString.SetSharedData("cash", 850);
                targetString.SendChatMessage($"~r~[A] ~w~{sender.Name} add {amount} bucks to you. New balance {targetString.GetSharedData<int>("cash")}");
            }
            else sender.SendChatMessage(Messages.PLAYER_NOT_FOUND);
        }

        [Command("aduty")]
        public void CommandADuty(Player sender, bool invisibility = false)
		{
            if (!invisibility)
			{
                if (sender.Transparency == 255)
                {
                    sender.Transparency = 127;
                    sender.TriggerEvent("SetPlayerInvincible");
                    foreach (Player item in NAPI.Pools.GetAllPlayers())
					{
                        if (item.GetData<int>("adminlevel") >= 1)
                            item.SendChatMessage($"~r~[A] ~b~{sender.Name} ~w~Came out on duty.");
					}
                }
                else
                {
                    sender.Transparency = 255;
                    sender.TriggerEvent("DafaultPlayerInvincible");
                    foreach (Player item in NAPI.Pools.GetAllPlayers())
                    {
                        if (item.GetData<int>("adminlevel") >= 1)
                            item.SendChatMessage($"~r~[A] ~b~{sender.Name} ~w~Came off dutyа.");
                    }
                }
            } 
            else
			{
                if (sender.Transparency == 255)
                {
                    sender.Transparency = 0;
                    sender.TriggerEvent("SetPlayerInvincible");
                    foreach (Player item in NAPI.Pools.GetAllPlayers())
                    {
                        if (item.GetData<int>("adminlevel") >= 1)
                            item.SendChatMessage($"~r~[A] ~b~{sender.Name} ~w~Came out on duty.");
                    }
                }
                else
                {
                    sender.Transparency = 255;
                    sender.TriggerEvent("DafaultPlayerInvincible");
                    foreach (Player item in NAPI.Pools.GetAllPlayers())
                    {
                        if (item.GetData<int>("adminlevel") >= 1)
                            item.SendChatMessage($"~r~[A] ~b~{sender.Name} ~w~Came off duty.");
                    }
                }
            }
		}

        [Command("setpower")]
        public void CommandSetEnginePowerMultiplier(Player sender, float multiplier)
		{
            sender.TriggerEvent("SetEnginePowerMultiplier", multiplier);
		}

        [Command("settorque")]
        public void CommandSetEngineTorqueMultiplier(Player sender, float multiplier)
        {
            sender.TriggerEvent("SetEnginePowerMultiplier", multiplier);
        }

        [Command("findplayer")]
        public void CommandFindPlayer(Player sender, string partOfName)
        {
            var message = $"Found matches:{Environment.NewLine}";
            foreach (Player item in NAPI.Pools.GetAllPlayers())
            {
                if (item.Name.Contains(partOfName))
                    message += Core.CharacterNameWithId(item) + Environment.NewLine;
            };
            sender.SendChatMessage(message);
        }

        [Command("slap", GreedyArg = true)]
        public void CommandSlap(Player sender, string targetString, string reason = "")
        {
            Player target = int.TryParse(targetString, out int targetId) ? Core.GetPlayerById(targetId) : NAPI.Player.GetPlayerFromName(targetString);

            if (target != null)
            {
                if (reason == "")
                    target.SendChatMessage($"~r~[A] ~b~{Core.CharacterName(sender)} ~w~slapped you.");
                else
                    target.SendChatMessage($"~r~[A] ~b~{Core.CharacterName(sender)}~w~: {reason}");

                target.Position = new Vector3(target.Position.X, target.Position.Y, target.Position.Z + 3);
                sender.SendChatMessage($"~r~[A] ~w~You slapped ~t~{Core.CharacterNameWithId(target)}");
            }
            else sender.SendChatMessage(Messages.PLAYER_NOT_FOUND);
        }

        [Command("givegun")]
        public void CommandGiveWeapon(Player sender, string targetString, WeaponHash hash, int ammo = 150)
        {
            Player target = int.TryParse(targetString, out int targetId) ? Core.GetPlayerById(targetId) : NAPI.Player.GetPlayerFromName(targetString);

            if (target != null)
            {
                //if (ammo <= 0)
                //    ammo = 150;
                //if (Constants.SingleAmmoWeapons.Contains(hash))
                //    ammo = 1;
                //if (ammo == 150 && Constants.SingleAmmoWeapons.Contains(hash))
                //    ammo = 1;
                target.GiveWeapon(hash, ammo);
                target.SendChatMessage($"~r~[A] ~b~{Core.CharacterName(sender)} ~w~gave to you ~t~{hash} ({ammo})");
                sender.SendChatMessage($"~r~[A] ~w~You gave weapon to ~b~{Core.CharacterNameWithId(target)} {Constants.COLOR_ORANGE}{hash}({ammo})");
            }
            else sender.SendChatMessage(Messages.PLAYER_NOT_FOUND);
        }

        [Command("setskin")]
        public void CommandSetSkin(Player sender, string targetString, PedHash model, bool save = false)
        {
            Player target = int.TryParse(targetString, out int targetId) ? Core.GetPlayerById(targetId) : NAPI.Player.GetPlayerFromName(targetString);

            if (target != null)
            {
                target.SetSkin(model);
                target.SendChatMessage($"~r~[A] ~b~{Core.CharacterName(sender)} ~w~set to you new skin ~b~{model}");
                sender.SendChatMessage($"~r~[A] ~w~You set new skin ~b~{model} ~w~to ~b~{Core.CharacterNameWithId(target)}");

                if (save == true)
                    target.SetData("skin", model);
            }
            else sender.SendChatMessage(Messages.PLAYER_NOT_FOUND);
        }

        [Command("kick", GreedyArg = true)] // huinya
        public void CommandKick(Player sender, string targetString, string reason)
        {
            Player target = int.TryParse(targetString, out int targetId) ? Core.GetPlayerById(targetId) : NAPI.Player.GetPlayerFromName(targetString);

            if (target != null)
            {
                target.Kick();
                NAPI.Chat.SendChatMessageToAll($"~r~[A] ~b~{Core.CharacterName(sender)} ~w~kick player ~b~{Core.CharacterName(target)}~w~.");
                NAPI.Chat.SendChatMessageToAll($"~t~Reason~w~: {reason}");
            }
            else sender.SendChatMessage(Messages.PLAYER_NOT_FOUND);
        }

        [Command("setweather")]
        public void CommandSetWeather(Player sender, Weather id)
        {
            if (Constants.BannedWeathers.Contains(id))
                sender.SendChatMessage($"~r~[A] {Constants.COLOR_WHITE}The entered id is not available.");
            else
            {
                NAPI.World.SetWeather(id);
                NAPI.Chat.SendChatMessageToAll($"~r~[A] ~b~{Core.CharacterName(sender)}{Constants.COLOR_WHITE} change weather on the server.");
            }
        }

        [Command("setcomp")]
        public void CommandSetPlayerClothes(Player sender, int slot, int drawable, int texture)
        {
            sender.SetClothes(slot, drawable, texture);
        }

        [Command("setacs")]
        public void CommandSetPlayerAccessory(Player sender, int slot, int drawable, int texture)
        {
            sender.SetAccessories(slot, drawable, texture);
        }

        [Command("dim")]
        public void CommandGetEntityDimension(Player sender, string targetString)
		{
            Player target = int.TryParse(targetString, out int targetId) ? Core.GetPlayerById(targetId) : NAPI.Player.GetPlayerFromName(targetString);

            if (target != null)
            {
                uint dimension = target.Dimension;
                sender.SendChatMessage($"{Messages.MESSAGE_INFO}~w~ У игрока ~o~{Core.CharacterNameWithId(target)} ~b~{dimension}~w~ dimension.");
            }
            else sender.SendChatMessage(Messages.PLAYER_NOT_FOUND);
        }

        [Command("setdim")]
        public void CommandSetEntityDimension(Player sender, string targetString, uint dimension)
		{
            Player target = int.TryParse(targetString, out int targetId) ? Core.GetPlayerById(targetId) : NAPI.Player.GetPlayerFromName(targetString);

            if (target != null)
            {
                target.Dimension = dimension;
                sender.SendChatMessage($"{Messages.MESSAGE_INFO}~w~ You set to player~o~ {Core.CharacterNameWithId(target)}~b~ {dimension}~w~ dimenson.");
                target.SendChatMessage($"~w~Administratot ~r~{Core.CharacterName(sender)} ~w~ set to you ~b~{dimension} ~w~dimension.");
            }
            else sender.SendChatMessage(Messages.PLAYER_NOT_FOUND);
        }

        [Command("flip")]
        public void CommandFlipPlayer(Player sender, string targetString)
		{
            Player target = int.TryParse(targetString, out int targetId) ? Core.GetPlayerById(targetId) : NAPI.Player.GetPlayerFromName(targetString);

            if (target != null)
            {
                if (target.IsInVehicle)
				{
                    Vehicle vehicle = target.Vehicle;
                    vehicle.Rotation = new Vector3(0, 0, vehicle.Rotation.Z);
                    sender.SendChatMessage($"{Messages.MESSAGE_INFO}~w~ You flipped vehicle to player ~o~{Core.CharacterNameWithId(target)}");
                    target.SendChatMessage($"~w~Administratot ~r~{Core.CharacterName(sender)} ~w~flipped your vehicle.");
                }
            }
            else sender.SendChatMessage(Messages.PLAYER_NOT_FOUND);
        }

        [Command("freeze")]
        public void FreezePos(Player sender, string targetString)
		{
            Player target = int.TryParse(targetString, out int targetId) ? Core.GetPlayerById(targetId) : NAPI.Player.GetPlayerFromName(targetString);

            if (target != null)
            {
                target.TriggerEvent("freeze");
                target.SendChatMessage($"~r~[A] ~b~{sender.Name} ~w~заморозил Вас.");
                sender.SendChatMessage($"~r~[A] ~w~Вы заморозили ~b~{Core.CharacterNameWithId(target)}");
            }
            else sender.SendChatMessage(Messages.PLAYER_NOT_FOUND);
		}

        [Command("unfreeze")]
        public void UnFreezePos(Player sender, string targetString)
        {
            Player target = int.TryParse(targetString, out int targetId) ? Core.GetPlayerById(targetId) : NAPI.Player.GetPlayerFromName(targetString);

            if (target != null)
            {
                target.TriggerEvent("unfreeze");
                target.SendChatMessage($"~r~[A] ~b~{sender.Name} ~w~разморозил Вас.");
                sender.SendChatMessage($"~r~[A] ~w~Вы разморозили ~b~{Core.CharacterNameWithId(target)}");
            }
            else sender.SendChatMessage(Messages.PLAYER_NOT_FOUND);
        }

        [Command("anim")]
        public void CommandPlayAnim(Player sender, int flag, string animDictm, string animName, float speed = 8f)
		{
            sender.Position = new Vector3(sender.Position.X, sender.Position.Y, sender.Position.Z - 0.5);
            sender.PlayAnimation(animDictm, animName, flag);
		}

        [Command("sanim")]
        public void CommandStopPlayAnim(Player sender)
        {
            sender.StopAnimation();
        }

        [Command("goto")]
        public void CommandGoToPlayer(Player sender, string targetString)
		{
            Player target = int.TryParse(targetString, out int targetId) ? Core.GetPlayerById(targetId) : NAPI.Player.GetPlayerFromName(targetString);

            if (target != null)
            {
                sender.Position = target.Position;
                sender.Dimension = target.Dimension;
                sender.SendChatMessage($"{Messages.MESSAGE_INFO} ~w~You go to  ~o~{Core.CharacterNameWithId(target)}");
                sender.SendChatMessage($"~b~Dimension: ~w~{target.Dimension}");
            }
            else sender.SendChatMessage(Messages.PLAYER_NOT_FOUND);
        }

        [Command("gethere")]
        public void CommandGetHerePlayer(Player sender, string targetString)
        {
            Player target = int.TryParse(targetString, out int targetId) ? Core.GetPlayerById(targetId) : NAPI.Player.GetPlayerFromName(targetString);

            if (target != null)
            {
                target.Position = sender.Position;
                target.Dimension = sender.Dimension;
                sender.SendChatMessage($"{Messages.MESSAGE_INFO} ~w~Teleported the player ~o~{Core.CharacterNameWithId(target)} ~w~to you.");
                target.SendChatMessage($"~w~Administrator ~r~{Core.CharacterName(sender)} ~w~teleported you to his place.");
            }
            else sender.SendChatMessage(Messages.PLAYER_NOT_FOUND);
        }

        [Command("g", GreedyArg = true)]
        public void CommandAnnouncment(Player sender, string message)
		{
            NAPI.Chat.SendChatMessageToAll($"~o~[A] {Core.CharacterName(sender)}~w~: {message}");
		}

        [Command("kickall", GreedyArg = true)] // huinya
        public void CommandKickAllPlayers(Player sender, string reason = "")
		{
            foreach (Player item in NAPI.Pools.GetAllPlayers())
			{
                item.Kick(reason);
                item.SendChatMessage($"{Messages.MESSAGE_INFO} {Core.CharacterName(sender)}~w~: {reason}");
			}
		}

        [Command("settime")]
        public void CommandSetTime(Player sender, int hours)
        {
            NAPI.World.SetTime(hours, 0, 0);
            sender.SendChatMessage($"~r~[A] ~w~Вы установили новое время. ~b~{hours}:00");
        }
    }
}
