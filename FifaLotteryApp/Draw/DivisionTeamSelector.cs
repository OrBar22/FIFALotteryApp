using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using String = System.String;

namespace FifaLotteryApp.Draw
{
    public class DivisionTeamSelector
    {
        private const string DivisionA = "A";
        private const string DivisionB = "B";
        private const string DivisionC = "C";
        private const string DivisionAll = "All";
        private const int NumOfDevisions = 4;
        private const int NumOfPlayers = 4;
        private const int MaxNumOfRetriesPerDraw = 20;

        private Dictionary<string, List<string>> _teamByDivision;
        private Dictionary<string, List<string>> _selectedTeamsByDivision;
        private Dictionary<int, List<string>> _selectedTeamsByPlayer;
        private static DivisionTeamSelector _instance = new DivisionTeamSelector();
        private ProtocolGame _protocolGame;

        public static DivisionTeamSelector Instance
        {
            get { return _instance; }
        }

        private DivisionTeamSelector()
        {
            InitializeAllTeamsPerDivision();

            _selectedTeamsByDivision = new Dictionary<string, List<string>>();
            InitializeSelectedTeamsPerDivision();

            _selectedTeamsByPlayer = new Dictionary<int, List<string>>();
            InitializeSelectedTeamsPerPlayer();
        }

        #region Initialization
        private void InitializeSelectedTeamsPerPlayer()
        {
            _selectedTeamsByPlayer[1] = new List<string>();
            _selectedTeamsByPlayer[2] = new List<string>();
            _selectedTeamsByPlayer[3] = new List<string>();
            _selectedTeamsByPlayer[4] = new List<string>();
        }

        private void InitializeSelectedTeamsPerDivision()
        {
            _selectedTeamsByDivision[DivisionA] = new List<string>();
            _selectedTeamsByDivision[DivisionB] = new List<string>();
            _selectedTeamsByDivision[DivisionC] = new List<string>();
            _selectedTeamsByDivision[DivisionAll] = new List<string>();
        }

        private void InitializeAllTeamsPerDivision()
        {
            _teamByDivision = new Dictionary<string, List<string>>();
            _teamByDivision.Add(DivisionA, new List<string>());
            _teamByDivision.Add(DivisionB, new List<string>());
            _teamByDivision.Add(DivisionC, new List<string>());
            _teamByDivision.Add(DivisionAll, new List<string>());

            _teamByDivision[DivisionA].AddRange(GetDivisionATeams());
            _teamByDivision[DivisionB].AddRange(GetDivisionBTeams());
            _teamByDivision[DivisionC].AddRange(GetDivisionCTeams());
            _teamByDivision[DivisionAll].AddRange(GetDivisionAllTeamsTeams());

            //_teamByDivision[DivisionA].ForEach(s =>
            //{
            //    if (s != "?")
            //    {
            //        _teamByDivision[DivisionAll].Add(s);
            //    }
            //}
            //);
            //_teamByDivision[DivisionB].ForEach(s =>
            //{
            //    if (s != "?")
            //    {
            //        _teamByDivision[DivisionAll].Add(s);
            //    }
            //}
            //);
            //_teamByDivision[DivisionC].ForEach(s => _teamByDivision[DivisionAll].Add(s));
        }

        #region Populate Teams
        private List<string> GetDivisionATeams()
        {
            List<string> res = new List<string>();
            res.Add("Man United");
            res.Add("Man City");
            res.Add("Chelsea");
            res.Add("Liverpool");
            res.Add("Bayern Munich");
            res.Add("PSG");
            res.Add("Real Madrid");
            res.Add("?");
            return res;
        }

        private List<string> GetDivisionBTeams()
        {
            List<string> res = new List<string>();
            res.Add("Tottenham");
            res.Add("Atletico madrid");
            res.Add("Juventus");
            res.Add("Barca");
            res.Add("Dortmund");
            res.Add("Napoli");
            res.Add("Inter");
            res.Add("?");
            return res;
        }

        private List<string> GetDivisionCTeams()
        {
            List<string> res = new List<string>();
            res.Add("Atalanta");
            res.Add("Leicester");
            res.Add("Leipzig");
            res.Add("Arsenal");
            res.Add("Lazio");
            res.Add("Ajax");
            res.Add("Sevilla");
            res.Add("?");
            return res;
        }

        private List<string> GetDivisionAllTeamsTeams()
        {
            List<string> res = new List<string>();
            res.AddRange(GetDivisionATeams());
            res.AddRange(GetDivisionBTeams());
            res.AddRange(GetDivisionCTeams());
            return res.Distinct().ToList();
        }
        #endregion
        #endregion

        public List<string> GetTeamSelection(string division)
        {
            return _teamByDivision[division];
        }

        public List<string> GetAllDivisions()
        {
            return _teamByDivision.Keys.ToList();
        }
        public bool HasProtocolGame
        {
            get { return _protocolGame != null; }
        }

        #region Draw

        public string DrawTeam(string division, int playerNum)
        {
            string selectedTeam = string.Empty;
            bool selected = false;
            int numOfRetries = 0;
            while (!selected && numOfRetries < MaxNumOfRetriesPerDraw)
            {
                System.Random r = new System.Random();
                var teamNumber = r.Next(0, _teamByDivision[division].Count);
                selectedTeam = _teamByDivision[division][teamNumber];

                if (!_selectedTeamsByDivision[division].Contains(selectedTeam) && !_selectedTeamsByPlayer[playerNum].Contains(selectedTeam))
                {
                    _selectedTeamsByDivision[division].Add(selectedTeam);
                    _selectedTeamsByPlayer[playerNum].Add(selectedTeam);
                    selected = true;
                }
                else
                {
                    numOfRetries++;
                    selectedTeam = String.Empty;
                }
            }

            return selectedTeam;
        }

        public int DrawDivision()
        {
            System.Random r = new System.Random();
            int divisionNum = r.Next(1, NumOfDevisions + 1);

            return divisionNum;
        }

        #endregion

        #region Reset methods

        public void ResetTeamsDraw()
        {
            InitializeSelectedTeamsPerDivision();
        }

        public void ResetPlayerDraws()
        {
            InitializeSelectedTeamsPerPlayer();
        }
        private void ResetProtocol()
        {
            _protocolGame = null;
        }

        public void ResetAll()
        {
            ResetPlayerDraws();
            ResetTeamsDraw();
            ResetProtocol();
        }

        #endregion

        //private string ConvertDivisionNumToDivisionString(int divisionNum)
        //{
        //    string divisionStr;
        //    switch (divisionNum)
        //    {
        //        case 1:
        //            divisionStr = DivisionA;
        //            break;
        //        case 2:
        //            divisionStr = DivisionB;
        //            break;
        //        case 3:
        //            divisionStr = DivisionC;
        //            break;
        //        case 4:
        //            divisionStr = DivisionAll;
        //            break;
        //        default:
        //            divisionStr = string.Empty;
        //            break;
        //    }

        //    return divisionStr;
        //}

        #region Draw Games
        public string DrawGames(bool keepProtocolGame)
        {
            bool[,] gamesPlayed = new bool[NumOfPlayers, NumOfPlayers];
            InitializeGamesPlayed(gamesPlayed);

            StringBuilder sb = new StringBuilder();

            AppendRound(sb, gamesPlayed, keepProtocolGame && HasProtocolGame);
            AppendRound(sb, gamesPlayed);
            AppendRound(sb, gamesPlayed);

            return sb.ToString();
        }

        private void InitializeGamesPlayed(bool[,] gamesPlayed)
        {
            gamesPlayed[0, 0] = true;
            gamesPlayed[1, 1] = true;
            gamesPlayed[2, 2] = true;
            gamesPlayed[3, 3] = true;
        }

        private void AppendRound(StringBuilder sb, bool[,] gamesPlayed, bool keepProtocolGame = false)
        {
            int firstRandomNumber, secondRandomNumber;

            if (!keepProtocolGame)
            {
                int numberOfRetries = 0;
                Random r = new Random();
                firstRandomNumber = r.Next(1, 5);
                secondRandomNumber = r.Next(1, 5);

                while (gamesPlayed[firstRandomNumber - 1, secondRandomNumber - 1] &&
                       numberOfRetries < MaxNumOfRetriesPerDraw)
                {
                    secondRandomNumber = r.Next(1, 5);
                    numberOfRetries++;
                }

                if (numberOfRetries == MaxNumOfRetriesPerDraw)
                    sb.Append("Errorrrrrrr");
            }
            else
            {
                firstRandomNumber = _protocolGame.Player1;
                secondRandomNumber = _protocolGame.Player2;
            }

            gamesPlayed[firstRandomNumber - 1, secondRandomNumber - 1] =
                gamesPlayed[secondRandomNumber - 1, firstRandomNumber - 1] = true;

            sb.Append($"{GetLastPlayerTeam(firstRandomNumber)} - {GetLastPlayerTeam(secondRandomNumber)} {Environment.NewLine}");
            sb.Append(GetSecondGame(firstRandomNumber, secondRandomNumber, gamesPlayed));
        }

        private string GetSecondGame(int randomNumberA, int randomNumberB, bool[,] gamesPlayed)
        {
            string firstTeamPlayer = null;
            string secondTeamPlayer = null;
            int firstPlayer = 0, secondPlayer = 0;
            foreach (KeyValuePair<int, List<string>> keyValuePair in _selectedTeamsByPlayer)
            {
                if (keyValuePair.Key != randomNumberA && keyValuePair.Key != randomNumberB )
                {
                    if (firstTeamPlayer == null)
                    {
                        firstPlayer = keyValuePair.Key;
                        firstTeamPlayer = GetLastPlayerTeam(keyValuePair.Key);
                    }
                    else if (secondTeamPlayer == null)
                    {
                        secondPlayer = keyValuePair.Key;
                        secondTeamPlayer = GetLastPlayerTeam(keyValuePair.Key);
                    }
                }
            }

            gamesPlayed[firstPlayer - 1, secondPlayer - 1] =
                gamesPlayed[secondPlayer - 1, firstPlayer - 1] = true;

            _protocolGame = new ProtocolGame()
            {
                Player1 = firstPlayer,
                Player2 = secondPlayer
            };

            return $"{firstTeamPlayer} - {secondTeamPlayer} {Environment.NewLine}";
        }

        private string GetLastPlayerTeam(int randomNumber)
        {
            switch (randomNumber)
            {
                case 1:
                    return $"{_selectedTeamsByPlayer[1].Last()} (Player 1)";
                    break;
                case 2:
                    return $"{_selectedTeamsByPlayer[2].Last()} (Player 2)";
                    break;
                case 3:
                    return $"{_selectedTeamsByPlayer[3].Last()} (Player 3)";
                    break;
                case 4:
                    return $"{_selectedTeamsByPlayer[4].Last()} (Player 4)";
                    break;
                default:
                    return null;
                    break;
            }
        }
     
        #endregion
    }
}