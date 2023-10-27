using GTANetworkAPI;
using System;
using Arcadia.database;
using System.Collections.Generic;

namespace Arcadia.model
{
	class AccountModel : Script
	{
		public Player Player { get; set; }
		public string AccountName { get; set; }
		public string SocialClub { get; set; }
		public string Email { get; set; }
		public string RegisterIp { get; set; }
		public string LastIp { get; set; }
		public string RegisterHardwareId { get; set; }
		public string LastHardwareId { get; set; }

		public AccountModel() { }

		public AccountModel(string _accountname, Player _player)
		{
			AccountName = _accountname;
			Player = _player;
			SocialClub = _player.SocialClubName;
			RegisterIp = _player.Address;
			LastIp = _player.Address;
			RegisterHardwareId = _player.Serial;
			LastHardwareId = _player.Serial;
		}

		public void RegisterAccount(string _accountname, string _password, string _email)
		{
			AccountName = _accountname;
			Email = _email;
			Database.RegisterAccount(this, _password);

			Player.SendChatMessage($"~b~Аккаунт ~w~{AccountName} ~b~успешно зарегистрирован!");
			Player.SendChatMessage($"~g~Добро пожаловать на Arcadia Role Play!");

			Login(true);
		}

		public void Login(bool firstLogin)
		{
			//Player.Name = AccountName; - !!!!!!!!!!!!!!!!!!!!!!!!!!!!

			if (!firstLogin) Database.LoadAccount(this);
			Player.Name = AccountName;

			Player.SetData("AccountName", AccountName);
			Player.SendChatMessage($"Welcome back ~b~{AccountName}~w~!");
			Player.SendChatMessage($"Choose your character:");

			Player.TriggerEvent("ChooseCharater.Create");

			List <CharacterModel> characters = Database.GetAccountCharacters(AccountName);
			foreach (CharacterModel item in characters)
			{
				//string name = $"{item.FirstName} {item.SecondName}";
				string firstname = item.FirstName;
				string secondname = item.SecondName;

				if (characters.IndexOf(item) == 0)
					Player.TriggerEvent("ChooseCharater.AddFirstCharacter", firstname, secondname);
				else if (characters.IndexOf(item) == 1)
					Player.TriggerEvent("ChooseCharater.AddSecondCharacter", firstname, secondname);
				else if (characters.IndexOf(item) == 2)
					Player.TriggerEvent("ChooseCharater.AddThirdCharacter", firstname, secondname);
			}
		}

		public static bool IsAccountLoggedIn(Player player)
		{
			return player.HasData("AccountName");
		}
	}
}
