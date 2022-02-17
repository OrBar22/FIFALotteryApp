using System;
using System.Collections.Generic;
using System.Linq;

namespace FifaLotteryApp.Draw
{
    public class GamesSelector
    {
        private const int NumOfPlayers = 4;
        private const int MaxNumOfRetriesPerDraw = 20;
        
        private Game _protocolGame;

        public bool HasProtocolGame
        {
            get { return _protocolGame != null; }
        }

        public List<List<Game>> Draw(bool keepProtocolGame)
        {
            List<List<Game>> allGamesPerTurn = new List<List<Game>>();

            bool[,] gamesPlayed = new bool[NumOfPlayers, NumOfPlayers];
            InitializeGamesPlayed(gamesPlayed);

            var firstTurn = AppendRound(gamesPlayed, keepProtocolGame && HasProtocolGame);
            var secondTurn = AppendRound(gamesPlayed);
            var thirdTurn = AppendRound(gamesPlayed);

            if (firstTurn == null || secondTurn == null || thirdTurn == null)
                return null;

            allGamesPerTurn.Add(firstTurn);
            allGamesPerTurn.Add(secondTurn);
            allGamesPerTurn.Add(thirdTurn);
            
            _protocolGame = thirdTurn.Last();

            return allGamesPerTurn;
        }

        private void InitializeGamesPlayed(bool[,] gamesPlayed)
        {
            gamesPlayed[0, 0] = true;
            gamesPlayed[1, 1] = true;
            gamesPlayed[2, 2] = true;
            gamesPlayed[3, 3] = true;
        }

        private List<Game> AppendRound(bool[,] gamesPlayed, bool keepProtocolGame = false)
        {
            Game firstGame = DrawFirstGame(gamesPlayed, keepProtocolGame);

            if (firstGame == null)
                return null;

            Game secondGame = DrawSecondGame(firstGame, gamesPlayed);

            if (secondGame == null)
                return null;

            List<Game> games = new List<Game>() { firstGame, secondGame };

            return games;
        }

        private Game DrawFirstGame(bool[,] gamesPlayed, bool keepProtocolGame)
        {
            int firstRandomNumber, secondRandomNumber;

            if (!keepProtocolGame)
            {
                int numberOfRetries = 0;
                Random r = new Random();
                firstRandomNumber = r.Next(1, NumOfPlayers + 1);
                secondRandomNumber = r.Next(1, NumOfPlayers + 1);

                while (gamesPlayed[firstRandomNumber - 1, secondRandomNumber - 1] &&
                       numberOfRetries < MaxNumOfRetriesPerDraw)
                {
                    secondRandomNumber = r.Next(1, NumOfPlayers + 1);
                    numberOfRetries++;
                }

                if (numberOfRetries == MaxNumOfRetriesPerDraw)
                    return null;
            }
            else
            {
                firstRandomNumber = _protocolGame.Player1;
                secondRandomNumber = _protocolGame.Player2;
            }

            gamesPlayed[firstRandomNumber - 1, secondRandomNumber - 1] =
                gamesPlayed[secondRandomNumber - 1, firstRandomNumber - 1] = true;

            Game firstGame = new Game()
            {
                Player1 = firstRandomNumber,
                Player2 = secondRandomNumber
            };
            
            return firstGame;
        }

        private Game DrawSecondGame(Game firstGame, bool[,] gamesPlayed)
        {
            int player1FirstGame = firstGame.Player1;
            int player2FirstGame = firstGame.Player2;
            int player1SecondGame = 0, player2SecondGame = 0;

            for (int i = 1; i <= NumOfPlayers; i++)
            {
                if (i != player1FirstGame && i != player2FirstGame)
                {
                    if (player1SecondGame == 0)
                        player1SecondGame = i;
                    else if (player2SecondGame == 0)
                        player2SecondGame = i;
                }
            }

            gamesPlayed[player1SecondGame - 1, player2SecondGame - 1] =
                gamesPlayed[player2SecondGame - 1, player1SecondGame - 1] = true;

            Game secondGame = new Game()
            {
                Player1 = player1SecondGame,
                Player2 = player2SecondGame
            };

            return secondGame;
        }
        
        public void Reset()
        {
            _protocolGame = null;
        }
    }
}