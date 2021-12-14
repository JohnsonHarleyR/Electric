var Game = {
    levelsLoaded: false,

    timestamp: Date.now(),
    totalSecondsPassed: 0,
    secondsTowardInterval: 0,
    animationInterval: 0.02,

    mousePosition: null,
    highlightedPosition: null,

    load: function () {
        Levels.currentLevel = Levels.allLevels[0];
        Global.load();
        GameDisplay.load(this);

        Game.run();
    },

    run: function () {
        GameDisplay.context.canvas.addEventListener("mousemove", function (evt) { Game.getMousePosition(evt); });

        // start animation loop
        window.requestAnimationFrame(this.gameLoop);
    },

    gameLoop: function () {
        let newTimestamp = Date.now();
        let timePassed = (newTimestamp - Game.timestamp) / 1000;
        Game.timestamp = newTimestamp;
        Game.secondsTowardInterval += timePassed;
        Game.totalSecondsPassed += timePassed;

        if (Game.secondsTowardInterval > Game.animationInterval) {
            GameDisplay.drawCanvas();
        }

        window.requestAnimationFrame(Game.gameLoop);
    },

    isCursorOnGrid: function () {
        if (this.mousePosition.x > 0 && this.mousePosition.x < Global.gridWidth &&
            this.mousePosition.y > GameDisplay.gridStartY && this.mousePosition.y < Global.gridWidth) {
            return true;
        }
        return false;
    },

    setHighlightedPosition: function () {
        this.highlightedPosition = this.getRelativeGridSquarePosition(this.mousePosition);

        // test
        if (this.highlightedPosition != null) {
            console.log('[' + this.highlightedPosition.x + ', ' + this.highlightedPosition.y + ']');
        }

    },

    getRelativeGridSquarePosition: function (position) {
        if (!this.isCursorOnGrid || position === null) {
            return null;
        }

        for (let y = 0; y < Global.squaresAcross; y++) {
            for (let x = 0; x < Global.squaresAcross; x++) {
                let squareStartX = x * Global.squareWidth;
                let squareEndX = (x + 1) * Global.squareWidth;
                let squareStartY = y * Global.squareWidth + GameDisplay.gridStartY;
                let squareEndY = (y + 1) * Global.squareWidth + GameDisplay.gridStartY;

                if (position.x >= squareStartX && position.x < squareEndX &&
                    position.y >= squareStartY && position.y < squareEndY) {
                    return {
                        x: x,
                        y: y
                    };
                }
            }
        }

        return null;
    },

    getMousePosition: function (evt) {
        let rect = evt.target.getBoundingClientRect();
        this.mousePosition = {
            x: evt.clientX - rect.left,
            y: evt.clientY - rect.top
        };

        this.setHighlightedPosition();
    }
}