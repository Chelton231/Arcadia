let bot = mp.peds.new(
    mp.game.joaat('mp_m_freemode_01'),
    new mp.Vector3(413.8198, -971.0237, 29.456375),
    47.5,
    mp.players.local.dimension
);

mp.game.streaming.requestAnimDict("amb@code_human_wander_smoking@male@idle_a");

mp.events.add('entityStreamIn', (entity) => {
    if (entity && entity.handle !== 0) {    
        if (entity == bot) bot.taskPlayAnim("amb@code_human_wander_smoking@male@idle_a", "idle_a", 8.0, 1.0, -1, 1, 1.0, false, false, false);
    }
});