using System;
using System.Collections.Generic;
using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using FifaLotteryApp.Draw;
using Environment = System.Environment;
using String = System.String;

namespace FifaLotteryApp
{
    [Activity(Label = "FIFA Lottery", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private string _division;
        private int _numberOfPlayerDrawed;
        private const int MaxPlayer = 4;
        private const string SelectDivisionString = "Select Division";
        private string _currentTeamList;
        private bool _isLastRound = false;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            RegisterOnClickEvents();
            InitializeDivisionComboBox();
        }

        private void RegisterOnClickEvents()
        {
            Button fab = FindViewById<Button>(Resource.Id.fab);
            fab.Click += DrawDivision;

            Button player1 = FindViewById<Button>(Resource.Id.player1);
            player1.Click += DrawTeam;

            Button player2 = FindViewById<Button>(Resource.Id.player2);
            player2.Click += DrawTeam;

            Button player3 = FindViewById<Button>(Resource.Id.player3);
            player3.Click += DrawTeam;

            Button player4 = FindViewById<Button>(Resource.Id.player4);
            player4.Click += DrawTeam;

            Button resetButton = FindViewById<Button>(Resource.Id.ResetButton);
            resetButton.Click += OnNewRoundSelected;

            Button resetAll = FindViewById<Button>(Resource.Id.resetAll);
            resetAll.Click += ResetAll;

            ToggleButton player1Hide = FindViewById<ToggleButton>(Resource.Id.player1TeamHide);
            player1Hide.Click += ToggleHideButton;

            ToggleButton player2Hide = FindViewById<ToggleButton>(Resource.Id.player2TeamHide);
            player2Hide.Click += ToggleHideButton;

            ToggleButton player3Hide = FindViewById<ToggleButton>(Resource.Id.player3TeamHide);
            player3Hide.Click += ToggleHideButton;

            ToggleButton player4Hide = FindViewById<ToggleButton>(Resource.Id.player4TeamHide);
            player4Hide.Click += ToggleHideButton;

            Button gamesDrawButton = FindViewById<Button>(Resource.Id.GamesDraw);
            gamesDrawButton.Click += GamesDraw;
        }

        public void InitializeDivisionComboBox()
        {
            var comboBox = (Spinner)FindViewById(Resource.Id.divisionComboBox);
            comboBox.ItemSelected += OnDivisionItemSelected;

            var items = DrawManager.Instance.GetAllDivisions();
            items.Insert(0, SelectDivisionString);
            comboBox.Adapter = CreateAdapterForComboBox(items);
            comboBox.SetSelection(0);
        }

        private ArrayAdapter<String> CreateAdapterForComboBox(List<string> items)
        {
            ArrayAdapter<String> dataAdapter = new ArrayAdapter<String>(this,
                Android.Resource.Layout.SimpleSpinnerItem, items);  //simple_spinner_item
            dataAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);//simple_spinner_dropdown_item
            return dataAdapter;
        }

        private void ToggleHideButton(object sender, EventArgs e)
        {
            ToggleButton button = (ToggleButton)sender;
            
            var resourceId = GetPlayerTextIDFromHideButton(button);
            TextView playerTeam = FindViewById<TextView>(resourceId);

            resourceId = GetPlayerSafaIDFromHideButton(button);
            Button playerSafa = FindViewById<Button>(resourceId);
            
            if (button.Checked)
            {
                playerTeam.Visibility = ViewStates.Invisible;
                playerSafa.Visibility = ViewStates.Visible;
            }
            else
            {
                playerTeam.Visibility = ViewStates.Visible;
                playerSafa.Visibility = ViewStates.Invisible;
            }
        }

        private int GetPlayerSafaIDFromHideButton(ToggleButton button)
        {
            int resourceId = 0;
            switch (button.Id)
            {
                case Resource.Id.player1TeamHide:
                    resourceId = Resource.Id.player1TeamSafa;
                    break;
                case Resource.Id.player2TeamHide:
                    resourceId = Resource.Id.player2TeamSafa;
                    break;
                case Resource.Id.player3TeamHide:
                    resourceId = Resource.Id.player3TeamSafa;
                    break;
                case Resource.Id.player4TeamHide:
                    resourceId = Resource.Id.player4TeamSafa;
                    break;
            }

            return resourceId;
        }

        private int GetPlayerTextIDFromHideButton(ToggleButton button)
        {
            int resourceId = 0;
            switch (button.Id)
            {
                case Resource.Id.player1TeamHide:
                    resourceId = Resource.Id.player1Team;
                    break;
                case Resource.Id.player2TeamHide:
                    resourceId = Resource.Id.player2Team;
                    break;
                case Resource.Id.player3TeamHide:
                    resourceId = Resource.Id.player3Team;
                    break;
                case Resource.Id.player4TeamHide:
                    resourceId = Resource.Id.player4Team;
                    break;
            }

            return resourceId;
        }

        private void ResetAll(object sender, EventArgs e)
        {
            _isLastRound = false;
            Button resetButton = FindViewById<Button>(Resource.Id.ResetButton);
            NewRound(resetButton);
            TogglePlayerDrawButtonsVisibility(false);

            TextView teamList = FindViewById<TextView>(Resource.Id.teamList);
            teamList.Visibility = ViewStates.Invisible;
            teamList.Text = string.Empty;

            TextView divisionSelected = FindViewById<TextView>(Resource.Id.divisionView);
            divisionSelected.Visibility = ViewStates.Invisible;

            var comboBox = (Spinner)FindViewById(Resource.Id.divisionComboBox);
            comboBox.SetSelection(0);

            DrawManager.Instance.ResetAll();
        }

        private void OnDivisionItemSelected(object sender, EventArgs e)
        {
            Spinner comboBox = (Spinner)sender;
            _division = (string)comboBox.SelectedItem;
            if (_division != SelectDivisionString)
            {
                OnDivisionSelected();
            }
            else
            {
                ResetAll(sender, e);
            }
        }

        private void DrawDivision(object sender, EventArgs e)
        {
            int divisionNum = DrawManager.Instance.DrawDivision();
            var comboBox = (Spinner)FindViewById(Resource.Id.divisionComboBox);
            comboBox.SetSelection(divisionNum);
        }

        private void OnDivisionSelected()
        {
            TextView divisionSelected = FindViewById<TextView>(Resource.Id.divisionView);
            TextView teamList = FindViewById<TextView>(Resource.Id.teamList);
            divisionSelected.Visibility = ViewStates.Visible;

            if (_division != string.Empty)
            {
                divisionSelected.Text = $"Division selected: ";
                divisionSelected.SetTypeface(null, TypefaceStyle.Bold);

                // View Teams List for specific division
                var teams = DrawManager.Instance.GetTeamSelection(_division);
                teamList.Text = _currentTeamList = String.Join(Environment.NewLine, teams);
                teamList.Visibility = ViewStates.Visible;

                Button resetButton = FindViewById<Button>(Resource.Id.ResetButton);
                NewRound(resetButton);

                DrawManager.Instance.ResetPlayerDraws();
            }
            else
            {
                divisionSelected.Text = $"Could not select division";
                teamList.Visibility = ViewStates.Invisible;

                TogglePlayerDrawButtonsVisibility(false);
                HidePlayerTeamsText();
            }
        }

        private void TogglePlayerDrawButtonsVisibility(bool visible)
        {
            Button player1 = FindViewById<Button>(Resource.Id.player1);
            player1.Visibility = visible ? ViewStates.Visible : ViewStates.Invisible;

            Button player2 = FindViewById<Button>(Resource.Id.player2);
            player2.Visibility = visible ? ViewStates.Visible : ViewStates.Invisible;

            Button player3 = FindViewById<Button>(Resource.Id.player3);
            player3.Visibility = visible ? ViewStates.Visible : ViewStates.Invisible;

            Button player4 = FindViewById<Button>(Resource.Id.player4);
            player4.Visibility = visible ? ViewStates.Visible : ViewStates.Invisible;
        }

        private void DrawTeam(object sender, EventArgs e)
        {
            Button button = (Button)sender;

            _numberOfPlayerDrawed++;
            int playerTeamResourceId = 0;
            int playerTeamHideButtonResourceId = 0;
            int playerNum = 0;
            Color color = Color.Black;

            switch (button.Id)
            {
                case Resource.Id.player1:
                    playerTeamResourceId = Resource.Id.player1Team;
                    playerTeamHideButtonResourceId = Resource.Id.player1TeamHide;
                    color = Color.Blue;
                    playerNum = 1;
                    break;
                case Resource.Id.player2:
                    playerTeamResourceId = Resource.Id.player2Team;
                    playerTeamHideButtonResourceId = Resource.Id.player2TeamHide;
                    color = Color.DarkGreen;
                    playerNum = 2;
                    break;
                case Resource.Id.player3:
                    playerTeamResourceId = Resource.Id.player3Team;
                    playerTeamHideButtonResourceId = Resource.Id.player3TeamHide;
                    color = Color.Red;
                    playerNum = 3;
                    break;
                case Resource.Id.player4:
                    playerTeamResourceId = Resource.Id.player4Team;
                    playerTeamHideButtonResourceId = Resource.Id.player4TeamHide;
                    color = Color.DarkOrange;
                    playerNum = 4;
                    break;
                default:
                    break;
            }

            var selectedTeam = DrawManager.Instance.DrawTeam(_division, playerNum);
            if (selectedTeam != string.Empty)
            {
                ChangePlayerTeamProperties($"Player {playerNum} - {selectedTeam}", true, playerTeamResourceId, color);
                ChangeHideButtonProperties(true, playerTeamHideButtonResourceId);
            }
            else
            {
                ResetAll(null, null);
                return;
            }

            button.Visibility = ViewStates.Invisible;

            if (_numberOfPlayerDrawed == MaxPlayer)
            {
                Button resetButton = FindViewById<Button>(Resource.Id.ResetButton);
                if (!_isLastRound)
                {
                    if (DrawManager.Instance.IsLastRound(_division))
                    {
                        _isLastRound = true;
                        resetButton.Text = "Last Round";
                    }

                    resetButton.Visibility = ViewStates.Visible;
                }

                Button gamesDrawButton = FindViewById<Button>(Resource.Id.GamesDraw);
                gamesDrawButton.Visibility = ViewStates.Visible;
                if (DrawManager.Instance.HasProtocolGame)
                {
                    CheckBox protocolGameCheckBox = FindViewById<CheckBox>(Resource.Id.keepProtocolGameCheckbox);
                    protocolGameCheckBox.Visibility = ViewStates.Visible;
                }
            }
        }

        private void OnNewRoundSelected(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            NewRound(button);
        }

        private void NewRound(Button resetButton)
        {
            CheckBox protocolGameCheckBox = FindViewById<CheckBox>(Resource.Id.keepProtocolGameCheckbox);
            protocolGameCheckBox.Visibility = ViewStates.Invisible;
            protocolGameCheckBox.Checked = false;
            resetButton.Text = "New Round";
            resetButton.Visibility = ViewStates.Invisible;
            Button gamesDrawButton = FindViewById<Button>(Resource.Id.GamesDraw);
            gamesDrawButton.Visibility = ViewStates.Invisible;
            TextView teamList = FindViewById<TextView>(Resource.Id.teamList);
            teamList.Text = _currentTeamList;
            _numberOfPlayerDrawed = 0;
            TogglePlayerDrawButtonsVisibility(true);
            HidePlayerTeamsText();
            DrawManager.Instance.ResetTeamsDraw();
        }

        private void GamesDraw(object sender, EventArgs e)
        {
            Button gamesDrawButton = (Button)sender;
            gamesDrawButton.Visibility = ViewStates.Invisible;
            CheckBox protocolGameCheckBox = FindViewById<CheckBox>(Resource.Id.keepProtocolGameCheckbox);
            protocolGameCheckBox.Visibility = ViewStates.Invisible;

            string gamesDraw = DrawManager.Instance.DrawGames(protocolGameCheckBox.Checked);
            TextView teamList = FindViewById<TextView>(Resource.Id.teamList);
            teamList.Text = gamesDraw;

        }

        private void HidePlayerTeamsText()
        {
            ChangePlayerTeamProperties(string.Empty, false, Resource.Id.player1Team, Color.Black);
            ChangePlayerTeamProperties(string.Empty, false, Resource.Id.player2Team, Color.Black);
            ChangePlayerTeamProperties(string.Empty, false, Resource.Id.player3Team, Color.Black);
            ChangePlayerTeamProperties(string.Empty, false, Resource.Id.player4Team, Color.Black);

            ChangeHideButtonProperties(false, Resource.Id.player1TeamHide);
            ChangeHideButtonProperties(false, Resource.Id.player2TeamHide);
            ChangeHideButtonProperties(false, Resource.Id.player3TeamHide);
            ChangeHideButtonProperties(false, Resource.Id.player4TeamHide);

            ChangeHideButtonProperties(false, Resource.Id.player1TeamSafa);
            ChangeHideButtonProperties(false, Resource.Id.player2TeamSafa);
            ChangeHideButtonProperties(false, Resource.Id.player3TeamSafa);
            ChangeHideButtonProperties(false, Resource.Id.player4TeamSafa);
        }

        private void ChangePlayerTeamProperties(string text, bool visible, int resourceId, Color color)
        {
            TextView playerTeam = FindViewById<TextView>(resourceId);
            playerTeam.Text = text;
            playerTeam.Visibility = visible? ViewStates.Visible : ViewStates.Invisible;
            playerTeam.SetTextColor(color);
        }

        private void ChangeHideButtonProperties(bool visible, int resourceId)
        {
            ToggleButton hideButton = FindViewById<ToggleButton>(resourceId);
            hideButton.Visibility = visible ? ViewStates.Visible : ViewStates.Invisible;
            hideButton.Checked = false;
        }
    }
}
