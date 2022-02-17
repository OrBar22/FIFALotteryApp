using System;
using System.Collections.Generic;
using System.Linq;

namespace FifaLotteryApp.Draw
{
    public class TeamSelector
    {
        private const int MaxNumOfRetriesPerDraw = 40;

        private Dictionary<Division, List<string>> _teamByDivision;
        private Dictionary<Division, List<string>> _selectedTeamsByDivision;
        private Dictionary<int, List<string>> _selectedTeamsByPlayer;
        private Dictionary<int, List<Division>> _divisionsPlayedByPlayer; // relevant for Division.All mode

        #region Initialization

        public void Initialize()
        {
            InitializeAllTeamsPerDivision();

            _selectedTeamsByDivision = new Dictionary<Division, List<string>>();
            InitializeSelectedTeamsPerDivision();

            _selectedTeamsByPlayer = new Dictionary<int, List<string>>();
            InitializeSelectedTeamsPerPlayer();

            _divisionsPlayedByPlayer = new Dictionary<int, List<Division>>();
            InitializeDivisionsPlayedByPlayer();
        }

        private void InitializeDivisionsPlayedByPlayer()
        {
            _divisionsPlayedByPlayer[1] = new List<Division>();
            _divisionsPlayedByPlayer[2] = new List<Division>();
            _divisionsPlayedByPlayer[3] = new List<Division>();
            _divisionsPlayedByPlayer[4] = new List<Division>();
        }

        private void InitializeSelectedTeamsPerPlayer()
        {
            _selectedTeamsByPlayer[1] = new List<string>();
            _selectedTeamsByPlayer[2] = new List<string>();
            _selectedTeamsByPlayer[3] = new List<string>();
            _selectedTeamsByPlayer[4] = new List<string>();
        }

        private void InitializeSelectedTeamsPerDivision()
        {
            _selectedTeamsByDivision[Division.A] = new List<string>();
            _selectedTeamsByDivision[Division.B] = new List<string>();
            _selectedTeamsByDivision[Division.C] = new List<string>();
            _selectedTeamsByDivision[Division.All] = new List<string>();
        }

        private void InitializeAllTeamsPerDivision()
        {
            _teamByDivision = new Dictionary<Division, List<string>>();
            _teamByDivision.Add(Division.A, new List<string>());
            _teamByDivision.Add(Division.B, new List<string>());
            _teamByDivision.Add(Division.C, new List<string>());
            _teamByDivision.Add(Division.All, new List<string>());

            _teamByDivision[Division.A].AddRange(GetDivisionATeams());
            _teamByDivision[Division.B].AddRange(GetDivisionBTeams());
            _teamByDivision[Division.C].AddRange(GetDivisionCTeams());
            _teamByDivision[Division.All].AddRange(GetDivisionAllTeamsTeams());
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

        public List<string> GetTeamSelection(Division division)
        {
            return _teamByDivision[division];
        }

        public string GetLastPlayerTeam(int playerNum)
        {
            return _selectedTeamsByPlayer[playerNum].Last();
        }

        #region Draw

        public string Draw(Division division, int playerNum)
        {
            string selectedTeam = string.Empty;
            bool selected = false;
            int numOfRetries = 0;
            while (!selected && numOfRetries < MaxNumOfRetriesPerDraw)
            {
                Random r = new Random();
                var teamNumber = r.Next(0, _teamByDivision[division].Count);
                selectedTeam = _teamByDivision[division][teamNumber];

                selected = ValidateSelectedTeam(division, playerNum, selectedTeam);

                if (!selected)
                {
                    numOfRetries++;
                    selectedTeam = string.Empty;
                }
            }

            return selectedTeam;
        }

        private bool ValidateSelectedTeam(Division division, int playerNum, string selectedTeam)
        {
            bool selected = false;

            if (!_selectedTeamsByDivision[division].Contains(selectedTeam) &&
                !_selectedTeamsByPlayer[playerNum].Contains(selectedTeam))
            {
                if (division == Division.All)
                {
                    Division teamDivison = GetDivisionByTeam(selectedTeam);
                    if (!_divisionsPlayedByPlayer[playerNum].Contains(teamDivison))
                    {
                        _divisionsPlayedByPlayer[playerNum].Add(teamDivison);
                    }
                    else
                    {
                        return false;
                    }
                }

                _selectedTeamsByDivision[division].Add(selectedTeam);
                _selectedTeamsByPlayer[playerNum].Add(selectedTeam);
                selected = true;
            }

            return selected;
        }

        private Division GetDivisionByTeam(string selectedTeam)
        {
            if (selectedTeam == "?") // corner case that need to handle in the future
            {
                return Division.A;
            }
            else if (GetDivisionATeams().Contains(selectedTeam))
            {
                return Division.A;
            }
            else if (GetDivisionBTeams().Contains(selectedTeam))
            {
                return Division.B;
            }
            else
            {
                return Division.C;
            }
        }

        #endregion 

        public void ResetSelectedTeams()
        {
            foreach (var list in _selectedTeamsByDivision.Values)
            {
                list.Clear();
            }
        }

        public void ResetPlayerTeams()
        {
            foreach (var list in _selectedTeamsByPlayer.Values)
            {
                list.Clear();
            }
        }
    }
}