using GTANetworkAPI;
using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Arcadia.database;
using Arcadia.model;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Arcadia.globals
{
	class Commands : Script
	{
		[Command("register")]
		public void CommandRegisterAccount(Player player, string accountname, string password, string email)
		{
			//Regex regex = new Regex(@"([a-zA-Z]+)_([a-zA-Z]+)");
			//if (!regex.IsMatch(name))
			//{
			//    player.SendChatMessage("Неправильный формат имени, используйте [Имя_Фамилия]!");
			//    return;
			//}

			if (Database.DoesAccountNameExists(accountname))
			{
				player.SendChatMessage("Такое имя аккаунта уже существует!");
				player.SendChatMessage("Используйте /login, если персонаж пренадлежит Вам!");
				return;
			}

			AccountModel account = new AccountModel(accountname, player);
			account.RegisterAccount(accountname, password, email);
			player.SendChatMessage($"~b~Аккаунт ~w~{accountname} ~b~успешно зарегистрирован!");
			player.SendChatMessage($"~g~Добро пожаловать на Arcadia Role Play!");
		}

		[Command("createcharacter")]
		public void CommandCreateCharacter(Player player, string firstname, string secondname, string gender)
		{
			if (!AccountModel.IsAccountLoggedIn(player))
			{
				player.SendChatMessage($"Для создания персонажа необходимо войти в аккаунт!");
				return;
			}

			Regex regex = new Regex(@"([a-zA-Z]+)");

			if (!regex.IsMatch(firstname) || !regex.IsMatch(secondname))
			{
				player.SendChatMessage("Неправильный формат имени!");
				return;
			}

			if (Database.CheckingTheNumberOfCharacters(player.Name) >= 2)
			{
				player.SendChatMessage($"Превышено допустимое число персонажей на аккаунте!");
				return;
			}

			if (Database.DoesCharacterNameExists(firstname, secondname))
			{
				player.SendChatMessage("Такой персонаж уже существует!");
				player.SendChatMessage("Используйте /choosecharacter если персонаж пренадлежит Вам или используйте другое имя и фамилию!");
				return;
			}

			CharacterModel character = new CharacterModel(firstname, secondname, player);
			character.RegisterCharacter(firstname, secondname, gender);
			player.SendChatMessage($"Персонаж {firstname} {secondname} успешно создан!");
		}

		[Command("login")]
		public void CommandLogin(Player player, string accountname, string password)
		{
			if (!Database.CheckPassword(accountname, password))
			{
				player.SendNotification($"Неверно введен пароль.");
				return;
			}

			if (AccountModel.IsAccountLoggedIn(player))
			{
				player.SendChatMessage("Аккаунт уже залогинен, если это не Вы, обратитесь к администрации сервера!");
				return;
			}

			if (!Database.DoesAccountNameExists(accountname))
			{
				player.SendChatMessage("Такого персонажа не существует!");
				player.SendChatMessage("Используйте /register для регистрации нового персонажа!");
				return;
			}

			AccountModel account = new AccountModel(accountname, player);
			account.Login(false);
		}

		[Command("choosecharacter")]
		public void CommandChooseCharacter(Player player, string firstname, string secondname)
		{
			if (CharacterModel.IsCharacterLoggedIn(player))
			{
				player.SendChatMessage($"Вы уже выбрали персонажа. Что-бы сменить персонажа, необходимо перезайти на сервер.");
				return;
			}

			if (!Database.DoesCharacterNameExists(firstname, secondname))
			{
				player.SendChatMessage("Такого персонажа не существует!");
				player.SendChatMessage("Используйте /register для регистрации нового персонажа!");
				return;
			}

			string account = player.GetData<string>("AccountName");
			if (!Database.DoesAccountOwnedCharacter(account, firstname, secondname))
			{
				player.SendChatMessage("У Вас нету такого персонажа!");
				return;
			}

			CharacterModel character = new CharacterModel(firstname, secondname, player);
			character.LoginCharacter(false);
		}
	}
}
