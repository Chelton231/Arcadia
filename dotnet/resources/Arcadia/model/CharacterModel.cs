using GTANetworkAPI;
using System;
using Arcadia.database;

namespace Arcadia.model
{
	class CharacterModel : Script
	{
		public Player Player { get; set; }
		public int SqlId { get; set; }
		public string Account { get; set; }
		public string FirstName { get; set; }
		public string SecondName { get; set; }
		public int Level { get; set; }
		public int Expirience { get; set; } 
		public string Gender { get; set; }
		public string DOB { get; set; }
		public uint Skin { get; set; }
		public int AdminLevel { get; set; }
		public int Cash { get; set; }
		public int Bank { get; set; }
		public int Faction { get; set; }
		public int Rank { get; set; }
		public int Health { get; set; }
		public int Armor { get; set; }
		public int TotalTimePlayed { get; set; }
		public float PosX { get; set; }
		public float PosY { get; set; }
		public float PosZ { get; set; }
		public string RegisterIp { get; }
		public string LastIp { get; }
        public int TimePlayed { get; set; }

        public CharacterModel() { }

		public CharacterModel(string _firstname, string _secondname, Player _player)
		{
			Player = _player;
			Account = Player.Name;
			FirstName = _firstname;
			SecondName = _secondname;
			Level = 1;
			Expirience = 0;
			Cash = 750;
			Bank = 5000;
			AdminLevel = 0;
			RegisterIp = _player.Address;
			LastIp = _player.Address;
			Skin = Skin;
		}

		public void RegisterCharacter(string _firstname, string _secondname, string _gender)
		{
			FirstName = _firstname;
			SecondName = _secondname;
			Gender = _gender;
			Skin = Gender == "male" ? 1885233650 : Convert.ToUInt32(-1667301416);
			Database.CreateCharacter(this);

			LoginCharacter(true);
		}

		public void LoginCharacter(bool firstlogin)
		{
			if (!firstlogin) Database.LoadCharacter(this);

			Player.Name = FirstName + "_" + SecondName;

			Player.SetSkin(Skin);
			Player.SetData("sqlid", SqlId);
			Player.SetData("account", Account);
			Player.SetData("firstname", FirstName);
			Player.SetData("secondname", SecondName);
			Player.SetSharedData("level", Level);
			Player.SetSharedData("expirience", Expirience);
			Player.SetData("gender", Gender);
			Player.SetData("skin", Skin);
			Player.SetData("adminlevel", AdminLevel);
			Player.SetSharedData("cash", Cash);
			Player.SetSharedData("bank", Bank);
			Player.SetData("faction", Faction);
			Player.SetData("rank", Rank);
			Player.SetData("PlayerPlayed", true);
			Player.SetData("PlayerPlaying", 0);
			Player.SetSharedData("totaltimeplayed", TotalTimePlayed);
			Player.SendChatMessage($"Character ~b~{FirstName} {SecondName}~w~ successful load!");
			Player.Dimension = 0;
			Player.Health = Health;
			Player.Armor = Armor;

			if (PosX != 0 && PosY != 0 && PosZ != 0)
				Player.Position = new Vector3(PosX, PosY, PosZ);
		}

		public void Disconect()
		{
			Save();
		}

		public void Save()
		{
			NAPI.Util.ConsoleOutput($"[INFO] Characters {FirstName} {SecondName} was succssesful saved.");
			Database.UpdateCharacter(this);
		}

		public static bool IsCharacterLoggedIn(Player player)
		{
			if (player.HasData("PlayerPlaying"))
				return true;
			return false;
		}
	}
}
