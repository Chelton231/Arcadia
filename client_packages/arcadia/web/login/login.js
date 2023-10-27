const Browser = require('./arcadia/classes/browser');
const Camera = require('./arcadia/classes/camera');

let loginBrowser = new Browser('loginBrowser', 'package://arcadia/web/login/login.html');
let charactersBrowser = null;
let createСharactersBrowser = null;
let loginCam = new Camera('loginCam', new mp.Vector3(24.48141, -255.5137, 141.269), new mp.Vector3(-51.10479, -680.2582, 140.7251), 40);
loginCam.startMoving(250.0, 0.15);

setTimeout(() => {
    mp.gui.cursor.show(true, true);
    mp.gui.chat.show(false);
    mp.game.ui.displayRadar(false);
}, 250);

mp.events.add({
    
    //отправка на сервер данных логина
    "Login.Submit": (accountname, password) => {
        mp.events.callRemote('Login.OnLogin', accountname, password);
    },

    //Ошибка пароля
    "Login.WrongPassword" : ()=> {
        loginBrowser.callFunction(`WrongPassword()`);
    },

    //Ошибка логина
    "Login.AccountNameNotExists" : () => {
        loginBrowser.callFunction(`AccountNameNotExists()`);
    },

    //Успехный вход, убираем камеру, бразуер 
    "Login.Success": () => {
        // mp.gui.cursor.show(false, false);

        // mp.gui.chat.show(true);
        // mp.game.ui.displayRadar(true);

        //А ЭТО ЗАКОММЕНТИТЬ!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        if(loginCam !== null) {
            loginCam.delete();
            loginCam = new Camera('loginCam', new mp.Vector3(31.5966, 488.0076, 184.76924), new mp.Vector3(23.29097, 521.7514, 170.22752), 20);
            loginCam.startMoving(10.0, 0.005);
        }

        // if(loginBrowser !== null) {
        //     loginBrowser.close();
        //     loginBrowser = null;
        // }

        if(loginBrowser !== null) {
            loginBrowser.close();
            loginBrowser = null;
        }
    },
    
    //Создаем новый браузер с созданием персонажа
    "ChooseCharater.Create" : () => {
        charactersBrowser = new Browser('charactersBrowser', 'package://arcadia/web/characters/index.html');
    },
    
    //Добавляем первую карточку игрока
    "ChooseCharater.AddFirstCharacter" : (firstname, secondname) => {
        charactersBrowser.callFunction(`AddFirstCard('${firstname}', '${secondname}')`);
    },
    
    //Добавляем вторую карточку игрока
    "ChooseCharater.AddSecondCharacter" : (firstname, secondname) => {
        charactersBrowser.callFunction(`AddSecondCard('${firstname}', '${secondname}')`);
    },
    
    //Добавляем третью карточку игрока
    "ChooseCharater.AddThirdCharacter" : (firstname, secondname) => {
        charactersBrowser.callFunction(`AddThirdCard('${firstname}', '${secondname}')`);
    },

    //Регистрация, отправляем данные на сервер
    "Register.Submit": (accountname, password, email) => {
        mp.events.callRemote('Register.OnRegister', accountname, password, email);
    },

    //Выбор персонажа, отправляем на сервер выбранного персонажа для загрузки
    "ChooseCharacter.Submit": (firstname, secondname) => {
        //Rufe Event beim Server auf
        mp.events.callRemote('Character.ChooseCharacter', firstname, secondname);
    },

    //Успешный выбор персонажа, закрывает бразей, возвращаем худ
    "ChooseCharacter.Success": () => {
        mp.gui.cursor.show(false, false);

        //Zeigt Chat an
        mp.gui.chat.show(true);
        mp.game.ui.displayRadar(true);

        //ЭТО НАДО БУДЕТ РАСКОММЕНТИТЬ!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        if(loginCam !== null) {
            loginCam.delete();
            loginCam = null;
        }

        //Wenn Browser existiert lösche ihn
        if(charactersBrowser !== null) {
            charactersBrowser.close();
            charactersBrowser = null;
        }
    },


    "CreateCharacter.Open": () => {
        if(charactersBrowser !== null) {
            charactersBrowser.close();
            charactersBrowser = null;
        }

        createСharactersBrowser = new Browser('charactersBrowser', 'package://arcadia/web/characters/createcharacter.html');
    },

    "CreateCharacter.Submit": (firstname, secondname, gender) => {
        //Rufe Event beim Server auf
        mp.events.callRemote('Character.CreateCharacter', firstname, secondname, gender);
    },

    "CreateCharacter.Success": () => {
        mp.gui.cursor.show(false, false);

        //Zeigt Chat an
        mp.gui.chat.show(true);
        mp.game.ui.displayRadar(true);

        //wenn Camera existiert lösche sie
        if(loginCam !== null) {
            loginCam.delete();
            loginCam = null;
        }

        //Wenn Browser existiert lösche ihn
        if(createСharactersBrowser !== null) {
            createСharactersBrowser.close();
            createСharactersBrowser = null;
        }
    },

    "Register.Success": () => {
        //Versteckt Cursor
        mp.gui.cursor.show(false, false);

        //Zeigt Chat an
        mp.gui.chat.show(true);
        mp.game.ui.displayRadar(true);

        //wenn Camera existiert lösche sie
        if(loginCam !== null) {
            loginCam.delete();
            loginCam = null;
        }

        //Wenn Browser existiert lösche ihn
        if(loginBrowser !== null) {
            loginBrowser.close();
            loginBrowser = null;
        }
    }
})