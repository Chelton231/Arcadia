using GTANetworkAPI;
using Arcadia.database;
using Arcadia.model;
using System.Text.RegularExpressions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Arcadia.globals
{
	class Connection : Script
	{
        [RemoteEvent("Login.OnLogin")]
        public void OnLogin(Player player, string accountname, string password)
        {
            if (AccountModel.IsAccountLoggedIn(player))
            {
                player.SendNotification("You already loggen in, to change account relogin to the server.");
                return;
            }

            if (!Database.DoesAccountNameExists(accountname))
            {
                player.TriggerEvent("Login.AccountNameNotExists");
                return;
            }

            if (!Database.CheckPassword(accountname, password))
            {
                player.SetData("PasswordErrors", player.GetData<int>("PasswordErrors") +1 );
                if (player.GetData<int>("PasswordErrors") == 3)
                    player.Kick();
                player.TriggerEvent("Login.WrongPassword");
                return;
            }

            AccountModel account = new AccountModel(accountname, player);
            player.TriggerEvent("Login.Success");
            account.Login(false);
            player.ResetData("PasswordErrors");
		}

        [RemoteEvent("Register.OnRegister")]
        public void OnRegister(Player player, string accountname, string password, string email)
		{
			//Regex regex = new Regex(@"[A - Z][a - z] +");
   //         if (!regex.IsMatch(accountname))
			//{
			//	player.SendChatMessage("Неправильный формат имени, используйте [Имя_Фамилия]!");
			//	return;
			//}

			if (Database.DoesAccountNameExists(accountname))
            {
                player.SendChatMessage("This account name already exists.");
                player.SendChatMessage("Use /login, if this is your account.");
                return;
            }

            AccountModel account = new AccountModel(accountname, player);
            account.RegisterAccount(accountname, password, email);

            player.TriggerEvent("Register.Success");
        }

        [RemoteEvent("Character.CreateCharacter")]
        public void CreateCharater(Player player, string firstname, string secondname, string gender)
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
            player.TriggerEvent("CreateCharacter.Success");
            character.RegisterCharacter(firstname, secondname, gender);
            player.SendChatMessage($"Персонаж {firstname} {secondname} успешно создан!");
        }

        [RemoteEvent("Character.ChooseCharacter")]
        public void ChooseCharacter(Player player, string firstname, string secondname)
		{
            if (CharacterModel.IsCharacterLoggedIn(player))
            {
                player.SendChatMessage($"Вы уже выбрали персонажа. Что-бы сменить персонажа, необходимо перезайти на сервер.");
                return;
            }

            Console.WriteLine($"{firstname} {secondname}");

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
            player.TriggerEvent("ChooseCharacter.Success");
            character.LoginCharacter(false);
        }
    }
}
