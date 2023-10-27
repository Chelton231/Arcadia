mp.events.add({
    "SetPlayerInvincible" : () => {
        mp.players.local.setInvincible(true);
    },

    "DafaultPlayerInvincible" : () => {
        mp.players.local.setInvincible(false);
    }
})

mp.events.add ({
    "freeze" : () => {
        mp.players.local.freezePosition(true);
        if (mp.players.local.vehicle) {
            let vehicle = mp.players.local.vehicle;
            vehicle.freezePosition(true);
        } else return;
    },

    "unfreeze" : () => {
        mp.players.local.freezePosition(false);
        if (mp.players.local.vehicle) {
            let vehicle = mp.players.local.vehicle;
            vehicle.freezePosition(false);
        } else return;
    }
})