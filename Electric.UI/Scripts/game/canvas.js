var GameDisplay = {
    canvasId: 'gameCanvas',
    game: undefined,

    canvas: undefined,
    context: undefined,

    gridStartY: undefined,

    load: function (game) {
        GameDisplay.game = game;

        GameDisplay.canvas = $('#' + GameDisplay.canvasId);
        GameDisplay.context = GameDisplay.canvas[0].getContext("2d");

        GameDisplay.context.canvas.width = Global.canvasWidth;
        GameDisplay.context.canvas.height = (Global.squaresAcross * Global.squareWidth) + (2 * Global.squareWidth * 2);

        GameDisplay.context.canvas.style.border = Global.canvasOutlineColor;

        GameDisplay.gridStartY = 2 * Global.squareWidth;

        // test draw the grid
        GameDisplay.drawCanvas();
    },

    drawCanvas: function () {

        // draw canvas background
        this.context.beginPath();

        this.context.clearRect(0, 0,
            this.context.canvas.width, this.context.canvas.height);

        // draw background
        this.context.fillStyle = Global.canvasBackgroundColor;
        this.context.fillRect(0, 0,
            this.context.canvas.width, this.context.canvas.height);

        this.context.closePath();


        // draw background and line around around movement area;
        this.context.beginPath();

        // draw background
        this.context.fillStyle = Global.gridAreaBackgroundColor;
        this.context.fillRect(0, 0,
            Global.gridWidth, this.context.canvas.height);

        this.context.closePath();

        // draw outline
        //this.context.beginPath();

        //this.context.lineWidth = Global.gridAreaOutlineThickness;
        //this.context.strokeStyle = Global.gridAreaOutlineColor;
        //this.context.rect(0, 0,
        //    Global.gridWidth, this.context.canvas.height);
        //this.context.stroke();

        //this.context.closePath();



        // now draw grid
        // save room for highlighted, possibles, etc.
        let highlightedX = null;
        let highlightedY = null;
        for (let y = 0; y < Global.squaresAcross; y++) {
            for (let x = 0; x < Global.squaresAcross; x++) {
                if (!Levels.isInPossiblePositions(x, y)) {
                    this.drawSquare(x, y);
                } else {
                    // if it is in the possible positions, check if it's the highlighted one
                    if (this.game.highlightedPosition != null &&
                        this.game.highlightedPosition != undefined &&
                        this.game.highlightedPosition.x === x &&
                        this.game.highlightedPosition.y === y) {
                        highlightedX = x;
                        highlightedY = y;
                    }
                }
            }
        }

        // draw canvas outline
        this.context.beginPath();

        this.context.lineWidth = Global.canvasOutlineThickness;
        this.context.strokeStyle = Global.canvasOutlineColor;
        this.context.rect(0, 0,
            this.context.canvas.width, this.context.canvas.height);
        this.context.stroke();

        this.context.closePath();

        // draw possible squares
        let possibleDrawCount = 0;
        if (Levels.possibleNextPositions != null && Levels.possibleNextPositions != undefined) {
            for (let i = 0; i < Levels.possibleNextPositions.length; i++) {
                this.drawSquare(Levels.possibleNextPositions[i].x, Levels.possibleNextPositions[i].y);
                possibleDrawCount++;
            }
        }

        // draw highlighted square
        if (highlightedX != null && highlightedY != null) {
            this.drawSquare(highlightedX, highlightedY);
        }


    },

    drawSquare: function (x, y) {
        this.context.beginPath();

        let startX = x * Global.squareWidth;
        let startY = y * Global.squareWidth + this.gridStartY;

        // TODO create square class that houses data to determine what to draw here
        this.context.fillStyle = Global.squareBackgroundColor;
        this.context.fillRect(startX, startY,
            Global.squareWidth, Global.squareWidth);

        // create square outline according to circumstance
        let strokeColor = Global.squareOutlineColor;
        let lineWidth = Global.squareOutlineThickness;

        if (Levels.possibleNextPositions != null && Levels.possibleNextPositions != undefined && Levels.isInPossiblePositions(x, y)) {
            if (this.game.highlightedPosition != null &&
                this.game.highlightedPosition != undefined &&
                this.game.highlightedPosition.x === x &&
                this.game.highlightedPosition.y === y) {
                strokeColor = Global.squareHighlightedOutlineColor;
                lineWidth = Global.squareHightlightedOutlineThickness;

            } else {
                strokeColor = Global.squarePossibleOutlineColor;
                lineWidth = Global.squareHightlightedOutlineThickness;
            }
            
        }

        this.context.lineWidth = lineWidth;
        this.context.strokeStyle = strokeColor;
        this.context.rect(startX, startY,
            Global.squareWidth, Global.squareWidth);
        this.context.stroke();

        this.context.closePath();
    }
}