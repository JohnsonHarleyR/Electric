var Global = {

    // sizes
    squaresAcross: 5,
    squareWidth: 75,
    gridWidth: 0,
    canvasWidth: 0,

    canvasOutlineThickness: '4',
    gridAreaOutlineThickness: '2',
    squareOutlineThickness: '2',
    squareHightlightedOutlineThickness: '4',

    // Colors
    // TODO decide colors
    canvasBackgroundColor: '#E0E5E9',
    canvasOutlineColor: '#000000',

    gridAreaBackgroundColor: '#8798A8',
    gridAreaOutlineColor: '000000',

    squareBackgroundColor: '#ffffff',
    squareOutlineColor: '#000000',
    squareHighlightedOutlineColor: '#EF1ACC',
    squarePossibleOutlineColor: '#30D5C8',

    normalCircleColor: '#000000',
    correctCircleColor: '#00FF00',
    wrongCircleColor: '#FF0000',

    load: function () {
        this.gridWidth = this.squaresAcross * this.squareWidth;
        this.canvasWidth = this.gridWidth * 2;
    }
}