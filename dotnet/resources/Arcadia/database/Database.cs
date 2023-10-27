using GTANetworkAPI;
using Arcadia.model;
using Arcadia.imported;
using System;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using Arcadia.globals;

namespace Arcadia.database
{
	class Database : Script
	{
		private static readonly string connect = "SERVER=localhost; DATABASE=Arcadia; UID=Arcadia; PASSWORD=123";

		[ServerEvent(Event.ResourceStart)]
		public void OnResourceStart()
        {
			Core.teleportsList = LoadAllTeleports();
			Console.WriteLine($"Load {Core.teleportsList.Count} jump points");
		}

		public static List<TeleportModel> LoadAllTeleports()
        {
			List<TeleportModel> teleportsList = new List<TeleportModel>();

			using (MySqlConnection connection = new MySqlConnection(connect))
            {
				connection.Open();
				MySqlCommand command = connection.CreateCommand();
				command.CommandText = "SELECT * FROM JumpPoints";

				using (MySqlDataReader reader = command.ExecuteReader())
				{
					while (reader.Read())
					{
						TeleportModel jumpPoints = new TeleportModel();
						jumpPoints.Name = reader.GetString("name");
						jumpPoints.X = reader.GetFloat("x");
						jumpPoints.Y = reader.GetFloat("y");
						jumpPoints.Z = reader.GetFloat("z");
						jumpPoints.Creator = reader.GetString("creator");
						jumpPoints.DateTime = reader.GetString("datetime");

						teleportsList.Add(jumpPoints);
					}
				}
				return teleportsList;
			}
        }

		public static void DeleteJumpPoint(string jumppoint)
        {
			using (MySqlConnection connection = new MySqlConnection(connect))
			{
				try
				{
					connection.Open();
					MySqlCommand command = connection.CreateCommand();

					command.CommandText = "DELETE FROM jumppoints WHERE Name = @Name";
					command.Parameters.AddWithValue("@Name", jumppoint);
					command.ExecuteNonQuery();
				}
				catch (Exception ex)
				{
					NAPI.Util.ConsoleOutput("[EXCEPTION UpdateAllJumpPoints] " + ex.Message);
					NAPI.Util.ConsoleOutput("[EXCEPTION UpdateAllJumpPoints] " + ex.StackTrace);
				}
			}
		}

		public static void UpdateAllJumpPoints(List<TeleportModel> teleportsList)
		{
			using (MySqlConnection connection = new MySqlConnection(connect))
			{
				try
				{
					connection.Open();
					MySqlCommand command = connection.CreateCommand();

					command.CommandText = "INSERT INTO JumpPoints (Name, X, Y, Z, Creator, DateTime) VALUES (@Name, @X, @Y, @Z, @Creator, @DateTime) ON DUPLICATE KEY UPDATE Name = @Name";

					foreach (TeleportModel item in Core.teleportsList)
					{
						command.Parameters.Clear();

						command.Parameters.AddWithValue("@Name", item.Name);
						command.Parameters.AddWithValue("@X", item.X);
						command.Parameters.AddWithValue("@Y", item.Y);
						command.Parameters.AddWithValue("@Z", item.Z);
						command.Parameters.AddWithValue("@Creator", item.Creator);
						command.Parameters.AddWithValue("@DateTime", item.DateTime);

						command.ExecuteNonQuery();
					}
				}
				catch (Exception ex)
				{
					NAPI.Util.ConsoleOutput("[EXCEPTION UpdateAllJumpPoints] " + ex.Message);
					NAPI.Util.ConsoleOutput("[EXCEPTION UpdateAllJumpPoints] " + ex.StackTrace);
				}
			}
		}

		public static void RegisterAccount(AccountModel account, string password)
		{
			string saltedProtector = PasswordDerivation.Derive(password);

			using (MySqlConnection connection = new MySqlConnection(connect))
			{
				try
				{
					connection.Open();
					MySqlCommand command = connection.CreateCommand();

					command.CommandText = "INSERT INTO accounts (accountname, password, socialсlub, email, registerip, lastip, registerhardwareid, lasthardwareid) VALUES " +
										  "(@accountname, @password, @socialсlub, @email, @registerip, @lastip, @registerhardwareid, @lasthardwareid)";

					command.Parameters.AddWithValue("@accountname", account.AccountName);
					command.Parameters.AddWithValue("@password", saltedProtector);
					command.Parameters.AddWithValue("@socialсlub", account.SocialClub);
					command.Parameters.AddWithValue("@email", account.Email);
					command.Parameters.AddWithValue("@registerip", account.RegisterIp);
					command.Parameters.AddWithValue("@lastip", account.LastIp);
					command.Parameters.AddWithValue("@registerhardwareid", account.RegisterHardwareId);
					command.Parameters.AddWithValue("@lasthardwareid", account.LastHardwareId);

					command.ExecuteNonQuery();
					connection.Close();
				}
				catch (Exception e)
				{
					NAPI.Util.ConsoleOutput($"[Exception] RegisterAccount: {e.Message}");
					NAPI.Util.ConsoleOutput($"[Exception] RegisterAccount: {e.StackTrace}");
				}
			}
		}

		public static void LoadAccount(AccountModel account)
		{
			using (MySqlConnection connection = new MySqlConnection(connect))
			{
				connection.Open();
				MySqlCommand command = connection.CreateCommand();
				command.CommandText = "SELECT * FROM accounts WHERE accountname=@accountname LIMIT 1";
				command.Parameters.AddWithValue("@accountname", account.AccountName);

				using (MySqlDataReader reader = command.ExecuteReader())
				{
					if (reader.HasRows)
					{
						reader.Read();
						account.SocialClub = reader.GetString("socialсlub");
						account.AccountName = reader.GetString("accountname");
					}
				}
				connection.Close();
			}
		}

		public static void CreateCharacter(CharacterModel character)
		{
			using (MySqlConnection connection = new MySqlConnection(connect))
			{
				try
				{
					connection.Open();
					MySqlCommand command = connection.CreateCommand();

					command.CommandText = "INSERT INTO characters (account, firstname, secondname, level, expirience, gender, skin, adminlevel, cash, bank, faction, rank, registerip, lastip)" +
						" VALUES (@account, @firstname, @secondname, @level, @expirience, @gender, @skin, @adminlevel, @cash, @bank, @faction, @rank, @registerip, @lastip)";

					command.Parameters.AddWithValue("@account", character.Account);
					command.Parameters.AddWithValue("@firstname", character.FirstName);
					command.Parameters.AddWithValue("@secondname", character.SecondName);
					command.Parameters.AddWithValue("@level", character.Level);
					command.Parameters.AddWithValue("@expirience", character.Expirience);
					command.Parameters.AddWithValue("@gender", character.Gender);
					command.Parameters.AddWithValue("@skin", character.Skin);
					command.Parameters.AddWithValue("@adminlevel", character.AdminLevel);
					command.Parameters.AddWithValue("@cash", character.Cash);
					command.Parameters.AddWithValue("@bank", character.Bank);
					command.Parameters.AddWithValue("@faction", character.Faction);
					command.Parameters.AddWithValue("@rank", character.Rank);
					command.Parameters.AddWithValue("@registerip", character.RegisterIp);
					command.Parameters.AddWithValue("@lastip", character.LastIp);

					command.ExecuteNonQuery();
					connection.Close();
				}
				catch (Exception e)
				{
					NAPI.Util.ConsoleOutput($"[Exception] RegisterAccount: {e.Message}");
					NAPI.Util.ConsoleOutput($"[Exception] RegisterAccount: {e.StackTrace}");
				}
			}
		}

		public static CharacterModel LoadCharacter(CharacterModel character)
		{
			using (MySqlConnection connection = new MySqlConnection(connect))
			{
				connection.Open();
				MySqlCommand command = connection.CreateCommand();
				command.CommandText = "SELECT * FROM characters WHERE firstname=@firstname AND secondname=@secondname LIMIT 1";
				command.Parameters.AddWithValue("@firstname", character.FirstName);
				command.Parameters.AddWithValue("@secondname", character.SecondName);

				using (MySqlDataReader reader = command.ExecuteReader())
				{
					if (reader.HasRows)
					{
						reader.Read();
						character.SqlId = reader.GetInt16("id");
						character.Account = reader.GetString("account");
						character.FirstName = reader.GetString("firstname");
						character.SecondName = reader.GetString("secondname");
						character.Level = reader.GetInt16("level");
						character.Expirience = reader.GetInt16("expirience");
						character.Gender = reader.GetString("gender");
						character.Skin = reader.GetUInt32("skin");
						character.AdminLevel = reader.GetInt16("adminlevel");
						character.Cash = reader.GetInt32("cash");
						character.Bank = reader.GetInt32("bank");
						character.Faction = reader.GetInt32("faction");
						character.Rank = reader.GetInt32("rank");
						character.Health = reader.GetInt32("health");
						character.Armor = reader.GetInt32("armor");
						character.TotalTimePlayed = reader.GetInt16("totaltimeplayed");
						character.PosX = reader.GetFloat("posx");
						character.PosY = reader.GetFloat("posy");
						character.PosZ = reader.GetFloat("posz");
					}
				}
				connection.Close();
			}
			return character;
		}

		public static void UpdateCharacter (CharacterModel character)
		{
			using (MySqlConnection connection = new MySqlConnection(connect))
			{
				connection.Open();
				MySqlCommand command = connection.CreateCommand();
				command.CommandText = "UPDATE characters SET firstname=@firstname, secondname=@secondname, level=@level, expirience=@expirience, gender=@gender," +
															"skin=@skin, adminlevel=@adminlevel, cash=@cash, bank=@bank, faction=@faction, rank=@rank," +
                                                            "health=@health, armor=@armor, totaltimeplayed=@totaltimeplayed, posx=@posx, posy=@posy, posz=@posz WHERE id=@id";
				command.Parameters.AddWithValue("id", character.SqlId);
				command.Parameters.AddWithValue("firstname", character.FirstName);
				command.Parameters.AddWithValue("secondname", character.SecondName);
				command.Parameters.AddWithValue("level", character.Level);
				command.Parameters.AddWithValue("expirience", character.Expirience);
				command.Parameters.AddWithValue("gender", character.Gender);
				command.Parameters.AddWithValue("skin", character.Skin);
				command.Parameters.AddWithValue("adminlevel", character.AdminLevel);
				command.Parameters.AddWithValue("cash", character.Cash);
				command.Parameters.AddWithValue("bank", character.Bank);
				command.Parameters.AddWithValue("faction", character.Faction);
				command.Parameters.AddWithValue("rank", character.Rank);
				command.Parameters.AddWithValue("health", character.Health);
				command.Parameters.AddWithValue("armor", character.Armor);
				command.Parameters.AddWithValue("totaltimeplayed", character.TotalTimePlayed);
				command.Parameters.AddWithValue("posx", character.PosX);
				command.Parameters.AddWithValue("posy", character.PosY);
				command.Parameters.AddWithValue("posz", character.PosZ);

				command.ExecuteNonQuery();
				connection.Close();
			}
		}

		public static bool CheckPassword(string accountname, string input)
		{
			string password = string.Empty;

			using (MySqlConnection connection = new MySqlConnection(connect))
			{
				connection.Open();
				MySqlCommand command = connection.CreateCommand();
				command.CommandText = "SELECT password FROM accounts WHERE accountname=@accountname LIMIT 1";
				command.Parameters.AddWithValue("@accountname", accountname);

				using (MySqlDataReader reader = command.ExecuteReader())
				{
					if (reader.HasRows)
					{
						reader.Read();
						password = reader.GetString("password");
					}
				}
				connection.Close();
			}
			if (PasswordDerivation.Verify(password, input))
			{
				return true;
			}
			return false;
		}

		public static bool DoesAccountNameExists(string accountname)
		{
			using (MySqlConnection connection = new MySqlConnection(connect))
			{
				connection.Open();
				MySqlCommand command = connection.CreateCommand();
				command.CommandText = "SELECT * FROM accounts WHERE accountname=@accountname LIMIT 1";
				command.Parameters.AddWithValue("@accountname", accountname);

				using (MySqlDataReader reader = command.ExecuteReader())
				{
					if (reader.HasRows)
					{
						connection.Close();
						return true;
					}
				}
			}
			return false;
		}

		public static int CheckingTheNumberOfCharacters(string account)
		{
			using (MySqlConnection connection = new MySqlConnection(connect))
			{
				connection.Open();
				MySqlCommand command = connection.CreateCommand();
				command.CommandText = "SELECT COUNT(*) FROM characters WHERE account=@account";
				command.Parameters.AddWithValue("@account", account);

				int count = Convert.ToInt32(command.ExecuteScalar());
				connection.Close();
				return count;
			}
		}

		public static bool DoesAccountOwnedCharacter(string account, string firstname, string secondname)
		{
			using (MySqlConnection connection = new MySqlConnection(connect))
			{
				connection.Open();
				MySqlCommand command = connection.CreateCommand();
				command.CommandText = "SELECT * FROM characters WHERE account=@account AND firstname=@firstname AND secondname=@secondname LIMIT 1";
				command.Parameters.AddWithValue("@account", account);
				command.Parameters.AddWithValue("@firstname", firstname);
				command.Parameters.AddWithValue("@secondname", secondname);

				using (MySqlDataReader reader = command.ExecuteReader())
				{
					if (reader.HasRows)
					{
						connection.Close();
						return true;
					}
					return false;
				}
			}
		}

		public static List<CharacterModel> GetAccountCharacters(string account)
		{
			List<CharacterModel> characters = new List<CharacterModel>();

			using (MySqlConnection connection = new MySqlConnection(connect))
			{
				connection.Open();
				MySqlCommand command = connection.CreateCommand();
				command.CommandText = "SELECT firstname, secondname, level, gender, dateofbrithday, cash, bank FROM characters WHERE account = @account";
				command.Parameters.AddWithValue("@account", account);

				using (MySqlDataReader reader = command.ExecuteReader())
				{
					while (reader.Read())
					{
						CharacterModel character = new CharacterModel();
						character.FirstName = reader.GetString("firstname");
						character.SecondName = reader.GetString("secondname");
						character.Level = reader.GetInt16("level");
						character.Gender = reader.GetString("gender");
						character.DOB = reader.GetString("dateofbrithday");
						character.Cash = reader.GetInt32("cash");
						character.Bank = reader.GetInt32("bank");
						characters.Add(character);
					}
				}
				connection.Close();
			}
			return characters;
		}

		public static bool DoesCharacterNameExists(string firstname, string secondname)
		{
			using (MySqlConnection connection = new MySqlConnection(connect))
			{
				connection.Open();
				MySqlCommand command = connection.CreateCommand();
				command.CommandText = "SELECT * FROM characters WHERE firstname=@firstname AND secondname=@secondname LIMIT 1";
				command.Parameters.AddWithValue("@firstname", firstname);
				command.Parameters.AddWithValue("@secondname", secondname);

				using (MySqlDataReader reader = command.ExecuteReader())
				{
					if (reader.HasRows)
					{
						connection.Close();
						return true;
					}
				}
			}
			return false;
		}
    }
}
