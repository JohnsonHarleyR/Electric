using Electric.UI.Enums;
using Electric.UI.Global;
using Electric.UI.Models;
using System;
using System.Collections.Generic;

namespace Electric.UI.Helpers
{
    public class LevelGenerator
    {
        private int[,] generatedStage = null;
        private int generatedStepCount = 0;
        private int generatedForwardCount = 0;
        private int generatedDiagonalCount = 0;
        private int generatedSidewaysCount = 0;
        private int generatedBackwardCount = 0;

        private int highestStepCount = 0;

        private Random random = new Random();

        // NOTE: Due to the matrix setup, it will be referenced as [y,x] rather than the reverse.

        public List<LevelStageModel> GenerateLevels(int levelCount, int? maxSteps = null)
        {
            List<LevelStageModel> allLevels = new List<LevelStageModel>();

            // first generate all levels and store the highest step count
            for (int i = 0; i < levelCount; i++)
            {
                LevelStageModel newLevel;
                bool isExisting = true;

                // don't generate a level with the same difficulty
                do
                {
                    newLevel = GenerateLevelByStepPreference(maxSteps);
                    isExisting = IsRepeatDifficulty(newLevel, allLevels);

                } while (isExisting);

                if (newLevel.TotalSteps > highestStepCount)
                {
                    highestStepCount = newLevel.TotalSteps;
                }

                allLevels.Add(newLevel);
            }

            // now sort all those levels into lists by those counts
            List<LevelStageModel>[] levelsByStepCountArray = SortLevelsIntoListsByStepCount(allLevels);

            // now sort each of those lists by difficulty index
            levelsByStepCountArray = SortListsByDifficultyIndex(levelsByStepCountArray);

            // now put it all back together
            allLevels = AssembleSingleListWithLevelNumbers(levelsByStepCountArray);

            // set highest step count back to 0
            highestStepCount = 0;

            // return result
            return allLevels;

        }

        public LevelStageModel GenerateLevelByStepPreference(int? maxSteps)
        {
            LevelStageModel newLevel;

            // if there are a max number of steps, keep generating a level until it meets the criteria
            if (maxSteps == null)
            {
                newLevel = CreateRandomLevel();
            }
            else
            {
                int numberOfSteps = (int)maxSteps;
                do
                {
                    newLevel = CreateRandomLevel();
                    numberOfSteps = newLevel.TotalSteps;
                } while (numberOfSteps >= (int)maxSteps);
            }

            return newLevel;
        }

        public LevelStageModel CreateRandomLevel()
        {
            // Initiate
            LevelStageModel level = new LevelStageModel(GlobalVariables.SQUARES_ACROSS);

            int[,] initialStage = CreateEmptyStage();

            // Choose first step location - make it the previous step
            int[] firstStep = ChooseFirstStep();
            initialStage[firstStep[0], firstStep[1]] = 1;

            // Now generate next moves until it gets to end
            bool success = false;
            do
            {
                ResetGeneratedInformation();
                generatedForwardCount++;
                success = GenerateStage(firstStep, false, Direction.Forward, CreateStageCopy(initialStage), initialStage, 2);
            } while (success == false);

            // once successful, set stage to generated stage as well as the counts
            if (success)
            {
                level.Stage = generatedStage;
                level.TotalSteps = generatedStepCount;
                level.ForwardSteps = generatedForwardCount;
                level.DiagonalSteps = generatedDiagonalCount;
                level.SideSteps = generatedSidewaysCount;
                level.BackwardSteps = generatedBackwardCount;
            }

            // Create display string
            level.StageDisplayString = GenerateStageDisplayString(level.Stage);

            // Calculate the difficulty index
            level.DifficultyIndex = CalculateDifficultyIndex(level);

            // set generated stuff back to default
            ResetGeneratedInformation();

            // Return level
            return level;
        }

        private void ResetGeneratedInformation()
        {
            generatedStage = null;
            generatedStepCount = 0;
            generatedForwardCount = 0;
            generatedDiagonalCount = 0;
            generatedSidewaysCount = 0;
            generatedBackwardCount = 0;
        }

        private bool IsRepeatDifficulty(LevelStageModel newLevel, List<LevelStageModel> generatedLevels)
        {
            foreach (var existingLevel in generatedLevels)
            {
                if (newLevel.TotalSteps == existingLevel.TotalSteps && newLevel.DifficultyIndex == existingLevel.DifficultyIndex)
                {
                    return true;
                }
            }
            return false;
        }

        private List<LevelStageModel> AssembleSingleListWithLevelNumbers(List<LevelStageModel>[] levelListArray)
        {
            int levelCount = 1;
            List<LevelStageModel> assembledList = new List<LevelStageModel>();

            foreach (var levelList in levelListArray)
            {
                foreach (var level in levelList)
                {
                    level.LevelNumber = levelCount;
                    assembledList.Add(level);
                    levelCount++;
                }
            }

            return assembledList;
        }

        private List<LevelStageModel>[] SortListsByDifficultyIndex(List<LevelStageModel>[] levelListArray)
        {
            // sort each list individually
            foreach (var levelList in levelListArray)
            {
                levelList.Sort((a, b) => a.DifficultyIndex.CompareTo(b.DifficultyIndex));
            }

            // return result
            return levelListArray;
        }

        public List<LevelStageModel>[] SortLevelsIntoListsByStepCount(List<LevelStageModel> allLevels)
        {

            // create empty array
            List<LevelStageModel>[] listArray = new List<LevelStageModel>[highestStepCount];

            // create empty lists
            for (int i = 0; i < highestStepCount; i++)
            {
                listArray[i] = new List<LevelStageModel>();
            }

            // now sort all the levels into that list
            foreach (var level in allLevels)
            {
                listArray[level.TotalSteps - 1].Add(level);
            }

            // return the list array
            return listArray;
        }

        private decimal CalculateDifficultyIndex(LevelStageModel level)
        {
            return (decimal)(level.DiagonalSteps) + (decimal)(level.SideSteps * 2) + (decimal)(level.BackwardSteps * 5);
            //return (decimal)level.ForwardSteps / (decimal)((level.SideSteps * 2) + (level.BackwardSteps * 5) + 1);
        }

        private string GenerateStageDisplayString(int[,] stage)
        {
            string newDisplayString = "";

            for (int y = 0; y < 5; y++)
            {
                string newRowString = "";
                for (int x = 0; x < 5; x++)
                {
                    string initialString = "";
                    if (stage[y, x] < 10)
                    {
                        initialString = "0";
                    }
                    newRowString += $"{initialString}{stage[y, x]} ";
                }
                newDisplayString += $"{newRowString}\n";
            }

            return newDisplayString;
        }

        // returns a bool and modifies the stage if that bool is true
        private bool GenerateStage(int[] previousStep, bool wasDiagonal, Direction previousDirection, int[,] stage, int[,] previousStage, int count)
        {
            // if the previous step is the last row on the stage, return true
            if (previousStep[0] == GlobalVariables.SQUARES_ACROSS - 1)
            {
                // this was the final step so set the generated stage to it
                generatedStepCount = count - 1;
                generatedStage = stage;
                return true;
            }

            // create copy of stage
            int[,] newStage = CreateStageCopy(stage);

            // decide whether to move forward, sideways, or backwards
            Direction nextDirection = GetNextDirection(previousStep, stage);

            // if the direction is none,, backtrack and return false - otherwise use recursion on self
            if (nextDirection == Direction.None)
            {
                AdjustGeneratedCount(previousDirection, -1);
                AdjustDiagnonalCount(wasDiagonal, 1);
                stage = previousStage;
                return false;
            }
            else
            {
                // generate the next move on the stage
                int[] nextStep = GenerateNextMove(nextDirection, previousStep, stage);

                newStage[(int)nextStep[0], (int)nextStep[1]] = count;

                // adjust generated counts
                AdjustGeneratedCount(nextDirection, 1);

                // adjust diagnonal count
                bool isDiagonal = IsDiagonalMove(nextStep, previousStep);
                AdjustDiagnonalCount(isDiagonal, 1);

                return GenerateStage(nextStep, isDiagonal, nextDirection, newStage, stage, count + 1);
            }
        }

        private void AdjustDiagnonalCount(bool isDiagonal, int amount)
        {

            if (isDiagonal)
            {
                generatedDiagonalCount += amount;
            }

        }

        private bool IsDiagonalMove(int[] nextStep, int[] previousStep)
        {
            bool isDiagonal = false;

            int[] yPossibles = new int[] { 1, -1 };
            int[] xPossibles = new int[] { 1, -1 };
            for (int y = 0; y < yPossibles.Length; y++)
            {
                for (int x = 0; x < xPossibles.Length; x++)
                {
                    if (nextStep[0] == previousStep[0] + yPossibles[y] &&
                        nextStep[1] == previousStep[1] + xPossibles[x])
                    {
                        isDiagonal = true;
                        break;
                    }
                }
                if (isDiagonal)
                {
                    break;
                }
            }

            return isDiagonal;
        }

        private void AdjustGeneratedCount(Direction direction, int amount)
        {
            switch (direction)
            {
                case (Direction.Forward):
                    generatedForwardCount += amount;
                    break;
                case (Direction.Sidways):
                    generatedSidewaysCount += amount;
                    break;
                case (Direction.Backward):
                    generatedBackwardCount += amount;
                    break;
                default:
                    return;
            }
        }

        private int[] GenerateNextMove(Direction nextDirection, int[] previousStep, int[,] stage)
        {

            // otherwise, grab the possibilities
            List<int[]> possibilities;
            switch (nextDirection)
            {
                case (Direction.Forward):
                    possibilities = GetForwardPossibilities(previousStep, stage);
                    break;
                case (Direction.Sidways):
                    possibilities = GetSidewaysPossibilities(previousStep, stage);
                    break;
                case (Direction.Backward):
                    possibilities = GetBackwardPossibilities(previousStep, stage);
                    break;
                default:
                    possibilities = new List<int[]>();
                    break;
            }

            // now randomly choose a possibility
            int choice = random.Next(possibilities.Count);
            return possibilities[choice];
        }

        private Direction GetNextDirection(int[] previousStep, int[,] stage)
        {
            Direction nextDirection = Direction.Undecided;
            Direction[] directionChoices = new Direction[] { Direction.Forward, Direction.Sidways, Direction.Backward };

            // loop until a valid direction is chosen - use for loop - break if a valid direction is chosen
            for (int i = 0; i < directionChoices.Length; i++)
            {
                Direction possibleDirection = (Direction)random.Next(directionChoices.Length);
                if (CanMoveDirection(possibleDirection, previousStep, stage))
                {
                    nextDirection = possibleDirection;
                    break;
                }
            }
            // if it's still undecided, return none.
            if (nextDirection == Direction.Undecided)
            {
                nextDirection = Direction.None;
            }

            // return result
            return nextDirection;
        }

        private bool CanMoveDirection(Direction direction, int[] previousStep, int[,] stage)
        {
            List<int[]> possibles = GetPossibilities(direction, previousStep, stage);
            if (possibles.Count == 0)
            {
                return false;
            }
            return true;
        }

        private List<int[]> GetPossibilities(Direction direction, int[] previousStep, int[,] stage)
        {
            List<int[]> possibles;
            switch (direction)
            {
                case (Direction.Forward):
                    possibles = GetForwardPossibilities(previousStep, stage);
                    break;
                case (Direction.Sidways):
                    possibles = GetSidewaysPossibilities(previousStep, stage);
                    break;
                case (Direction.Backward):
                    possibles = GetBackwardPossibilities(previousStep, stage);
                    break;
                default:
                    possibles = new List<int[]>();
                    break;
            }
            return possibles;
        }

        public List<int[]> GetForwardPossibilities(int[] previousStep, int[,] stage)
        {
            List<int[]> possibilitites = new List<int[]>();
            if (previousStep[0] == GlobalVariables.SQUARES_ACROSS - 1)
            {
                return possibilitites;
            }

            int[] relatives = new int[] { -1, 0, 1 };
            int newY = previousStep[0] + 1;
            for (int i = 0; i < relatives.Length; i++)
            {
                int newX = previousStep[1] + relatives[i];
                // make sure it exists on the stage
                if (newX >= 0 && newX < GlobalVariables.SQUARES_ACROSS)
                {
                    // make sure there hasn't already been a move there
                    if (stage[newY, newX] == 0)
                    {
                        possibilitites.Add(new int[] { newY, newX });
                    }
                }
            }

            return possibilitites;
        }

        public List<int[]> GetSidewaysPossibilities(int[] previousStep, int[,] stage)
        {
            List<int[]> possibilitites = new List<int[]>();

            bool canMoveLeft = true;
            bool canMoveRight = true;

            if (previousStep[1] == 0)
            {
                canMoveLeft = false;
            }
            else if (previousStep[1] == GlobalVariables.SQUARES_ACROSS - 1)
            {
                canMoveRight = false;
            }

            if (canMoveLeft && stage[previousStep[0], previousStep[1] - 1] == 0)
            {
                possibilitites.Add(new int[] { previousStep[0], previousStep[1] - 1 });
            }

            if (canMoveRight && stage[previousStep[0], previousStep[1] + 1] == 0)
            {
                possibilitites.Add(new int[] { previousStep[0], previousStep[1] + 1 });
            }

            return possibilitites;
        }

        public List<int[]> GetBackwardPossibilities(int[] previousStep, int[,] stage)
        {
            List<int[]> possibilitites = new List<int[]>();
            if (previousStep[0] == 0)
            {
                return possibilitites;
            }

            int[] relatives = new int[] { -1, 0, 1 };
            int newY = previousStep[0] - 1;
            for (int i = 0; i < relatives.Length; i++)
            {
                int newX = previousStep[1] + relatives[i];
                // make sure it exists on the stage
                if (newX >= 0 && newX < GlobalVariables.SQUARES_ACROSS)
                {
                    // make sure there hasn't already been a move there
                    if (stage[newY, newX] == 0)
                    {
                        possibilitites.Add(new int[] { newY, newX });
                    }
                }
            }

            return possibilitites;
        }

        private int[,] CreateEmptyStage()
        {
            int[,] newStage = new int[GlobalVariables.SQUARES_ACROSS, GlobalVariables.SQUARES_ACROSS];

            for (int y = 0; y < GlobalVariables.SQUARES_ACROSS; y++)
            {
                for (int x = 0; x < GlobalVariables.SQUARES_ACROSS; x++)
                {
                    newStage[y, x] = 0;
                }
            }

            return newStage;
        }

        private int[,] CreateStageCopy(int[,] stage)
        {
            int[,] newStage = new int[GlobalVariables.SQUARES_ACROSS, GlobalVariables.SQUARES_ACROSS];

            for (int y = 0; y < GlobalVariables.SQUARES_ACROSS; y++)
            {
                for (int x = 0; x < GlobalVariables.SQUARES_ACROSS; x++)
                {
                    newStage[y, x] = stage[y, x];
                }
            }

            return newStage;
        }

        private int[] ChooseFirstStep()
        {
            int[] firstStep = new int[] { 0, -1 }; // y,x

            firstStep[1] = random.Next(GlobalVariables.SQUARES_ACROSS);

            return firstStep;
        }
    }
}