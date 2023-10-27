mp.game.vehicle.defaultEngineBehaviour = true;

mp.keys.bind(0x4C, true, function() {

    if(mp.players.local.vehicle) {
        if(!mp.players.local.vehicle.getIsEngineRunning()) {
            mp.players.local.vehicle.setEngineOn(true, false, false);
            mp.players.local.setConfigFlag(429, true);
        } else {
            mp.players.local.vehicle.setEngineOn(false, false, false);
            mp.players.local.setConfigFlag(429, true);
        }
    }


});

mp.events.add('render', () => {

    if (mp.players.local.vehicle) {
        let vehicle = mp.players.local.vehicle
        let speed = vehicle.getSpeed();

        speed = Math.ceil(speed * (speed / 20) * 2);

        mp.game.graphics.drawText("Speed: " + speed, [0.95, 0.92], { 
            font: 7, 
            color: [255, 255, 255, 185], 
            scale: [0.65, 0.65], 
            outline: true
        })

        let statuscolor = undefined;

        let enginestatusstring = "";
        if (mp.players.local.vehicle.getIsEngineRunning()){
            enginestatusstring = "ON";
            statuscolor = [18, 203, 61, 185];
        } else {
            enginestatusstring = "OFF";
            statuscolor = [255, 0, 0, 185];
        }

        mp.game.graphics.drawText("Engine: " + enginestatusstring, [0.94, 0.95], { 
            font: 7, 
            color: statuscolor, 
            scale: [0.75, 0.75], 
            outline: true
        })
    }
});

mp.events.add("SetEnginePowerMultiplier", (multiplier) => {
    if (mp.players.local.vehicle) {
        mp.players.local.vehicle.setEnginePowerMultiplier(multiplier)
    }
});

mp.events.add("SetEngineTorqueMultiplier", multiplier => {
    if (mp.players.local.vehicle) {
        mp.players.local.vehicle.setEngineTorqueMultiplier(multiplier);
    }
});