namespace Electric.UI.Models
{
    public class LevelStageModel
    {
        public int? LevelNumber { get; set; }

        public int[,] Stage { get; set; }

        public int ForwardSteps { get; set; }
        public int SideSteps { get; set; }
        public int BackwardSteps { get; set; }

        public int TotalSteps { get; set; }
        public decimal DifficultyIndex { get; set; }

        public string StageDisplayString { get; set; }

        public LevelStageModel(int squaresAcross)
        {
            LevelNumber = null;
            Stage = new int[squaresAcross, squaresAcross];
            DifficultyIndex = 0;
            ForwardSteps = 0;
            SideSteps = 0;
            BackwardSteps = 0;

            CreateEmptySquares(squaresAcross);
        }

        private void CreateEmptySquares(int squaresAcross)
        {
            for (int y = 0; y < squaresAcross; y++)
            {
                for (int x = 0; x < squaresAcross; x++)
                {
                    Stage[y, x] = 0;
                }
            }
        }
    }
}