using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FifaLotteryApp.Draw
{
    public class DrawManager
    {
        private TeamSelector _teamSelector;
        private GamesSelector _gamesSelector;
        private DivisionSelector _divisionSelector;

        private static DrawManager _instance = new DrawManager();

        public static DrawManager Instance
        {
            get { return _instance; }
        }

        private DrawManager()
        {
            _teamSelector = new TeamSelector();
            _teamSelector.Initialize();
            _gamesSelector = new GamesSelector();
            _divisionSelector = new DivisionSelector();
        }
        
        public bool HasProtocolGame
        {
            get { return _gamesSelector.HasProtocolGame; }
        }

        public List<string> GetAllDivisions()
        {
            return Enum.GetNames(typeof(Division)).ToList();
        }

        public List<string> GetTeamSelection(string divisionStr)
        {
            if (Enum.TryParse(divisionStr, out Division divisionEnum))
                return _teamSelector.GetTeamSelection(divisionEnum);
            else
                return new List<string>() { "Invalid division" };
        }

        #region Draws

        public string DrawTeam(string division, int playerNum)
        {
            if (Division.TryParse(division, out Division divisionEnum))
                return _teamSelector.Draw(divisionEnum, playerNum);
            else
                return "Unexpected Division";
        }

        public int DrawDivision()
        {
            return _divisionSelector.Draw();
        }

        public string DrawGames(bool keepProtocolGame)
        {
            var allGamesPerTurn = _gamesSelector.Draw(keepProtocolGame);

            return ConvertGamesToString(allGamesPerTurn);
        }

        private string ConvertGamesToString(List<List<Game>> allGamesPerTurn)
        {
            StringBuilder sb = new StringBuilder();

            foreach (List<Game> gamesInTurn in allGamesPerTurn)
            {
                foreach (Game game in gamesInTurn)
                {
                    sb.Append(GetStringPerGame(game));
                    sb.Append(Environment.NewLine);
                }
            }

            return sb.ToString();
        }

        private string GetStringPerGame(Game game)
        {
            return $"{_teamSelector.GetLastPlayerTeam(game.Player1)}  ({GetPlayerNameByNumber(game.Player1)}) - {_teamSelector.GetLastPlayerTeam(game.Player2)}  ({GetPlayerNameByNumber(game.Player2)})";
        }

        #endregion

        #region Reset methods

        public void ResetTeamsDraw()
        {
            _teamSelector.ResetSelectedTeams();
        }

        public void ResetPlayerDraws()
        {
            _teamSelector.ResetPlayerTeams();
        }

        private void ResetGames()
        {
            _gamesSelector.Reset();
        }

        public void ResetAll()
        {
            ResetPlayerDraws();
            ResetTeamsDraw();
            ResetGames();
        }

        #endregion

        public bool IsLastRound(string division)
        {
            if (Division.TryParse(division, out Division divisionEnum))
                return _teamSelector.IsLastRound(divisionEnum);
            else
                throw new Exception("Unexpected division");
        }

        public static string GetPlayerNameByNumber(int playerNum)
        {
            string playerName = null;
            switch (playerNum)
            {
                case 1: playerName = "Or";
                    break;
                case 2:
                    playerName = "Roman";
                    break;
                case 3:
                    playerName = "Yevgeni";
                    break;
                case 4:
                    playerName = "FULG";
                    break;
            }

            return playerName;
        }
    }
}