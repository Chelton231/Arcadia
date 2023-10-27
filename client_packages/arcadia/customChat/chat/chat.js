mp.events.add('Send_ToChat', (player, message) =>{
    mp.gui.chat.push(`${player.name.replace("_", " ")}[${player.id}]: ${message}`);
});
