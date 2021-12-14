var Levels = {

    currentLevel: undefined,
    allLevels: undefined,

    currentPosition: undefined,
    possibleNextPositions: undefined,

    generateLevels: function (game) {
        let http = new XMLHttpRequest();
        http.open('POST', "/Home/GetLevels", true);
        http.setRequestHeader("Content-Type", "application/json;charset=UTF-8");
        http.onload = function () {
            Levels.allLevels = JSON.parse(this.responseText);
            Levels.resetLevelVariables();
            game.levelsLoaded = true;
            //Levels.logAllLevels();
            game.load();
        }

        http.send();
    },

    resetLevelVariables: function () {
        this.currentPosition = this.newPos(2, -1); // TODO change the 2 so it's not hard coded
        this.possibleNextPositions = new Array();

        this.setPossiblePositions();
    },

    setPossiblePositions: function () {
        this.possibleNextPositions = new Array();

        if (this.currentPosition === null || this.currentPosition === undefined) {
            return;
        }

        // if the current y position is -1, add all in first row
        if (this.currentPosition.y === -1) {
            for (let x = 0; x < Global.squaresAcross; x++) {
                this.possibleNextPositions.push(this.newPos(x, 0));
            }
        }
    },

    newPos(x, y) {
        return {
            x: x,
            y: y
        };
    },

    isInPossiblePositions(x, y) {
        if (this.possibleNextPositions === null || this.possibleNextPositions === undefined) {
            return false;
        }

        for (let i = 0; i < this.possibleNextPositions.length; i++) {
            if (this.possibleNextPositions[i].x === x &&
                this.possibleNextPositions[i].y === y) {
                return true;
            }
        }
        return false;
    },

    logLevel: function (level) {
        console.log('Level ' + level.LevelNumber);
        console.log(level.StageDisplayString);
        console.log('Steps: ' + level.TotalSteps);
        console.log('Difficulty: ' + level.DifficultyIndex);
        console.log('-----------------------------------');
    },

    logCurrentLevel: function () {
        this.logLevel(this.currentLevel);
    },

    logAllLevels: function() {
        for (let i = 0; i < Levels.allLevels.length; i++) {
            Levels.logLevel(Levels.allLevels[i]);
        }
    }
}