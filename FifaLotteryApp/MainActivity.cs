using System;
using System.Collections.Generic;
using Android.App;
using Android.Bluetooth;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
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
        private const string HideText = "Hide";
        private const string ShowText = "Show";
        private const string SelectDivisionString = "Select Division";
        private string _currentTeamList;
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
            resetButton.Click += ResetTeamsDraw;

            Button resetAll = FindViewById<Button>(Resource.Id.resetAll);
            resetAll.Click += ResetAll;

            Button player1Hide = FindViewById<Button>(Resource.Id.player1TeamHide);
            player1Hide.Click += ToggleHideButton;

            Button player2Hide = FindViewById<Button>(Resource.Id.player2TeamHide);
            player2Hide.Click += ToggleHideButton;

            Button player3Hide = FindViewById<Button>(Resource.Id.player3TeamHide);
            player3Hide.Click += ToggleHideButton;

            Button player4Hide = FindViewById<Button>(Resource.Id.player4TeamHide);
            player4Hide.Click += ToggleHideButton;
            
            Button gamesDrawButton = FindViewById<Button>(Resource.Id.GamesDraw);
            gamesDrawButton.Click += GamesDraw;
        }

        public void InitializeDivisionComboBox()
        {
            var comboBox = (Spinner)FindViewById(Resource.Id.divisionComboBox);
            comboBox.ItemSelected += OnDivisionItemSelected;

            var items = DivisionTeamSelector.Instance.GetAllDivisions();
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
            Button button = (Button)sender;
            var resourceId = GetPlayerTextIDFromHideButton(button);
            TextView playerTeam = FindViewById<TextView>(resourceId);
            if (button.Text == HideText)
            {
                button.Text = ShowText;
                playerTeam.Visibility = ViewStates.Invisible;
            }
            else
            {
                button.Text = HideText;
                playerTeam.Visibility = ViewStates.Visible;
            }
        }

        private int GetPlayerTextIDFromHideButton(Button button)
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
            Button resetButton = FindViewById<Button>(Resource.Id.ResetButton);
            NewRound(resetButton);
            TogglePlayerDrawButtonsVisibility(false);
            DivisionTeamSelector.Instance.ResetPlayerDraws();

            TextView teamList = FindViewById<TextView>(Resource.Id.teamList);
            teamList.Visibility = ViewStates.Invisible;
            teamList.Text = string.Empty;

            TextView divisionSelected = FindViewById<TextView>(Resource.Id.divisionView);
            divisionSelected.Visibility = ViewStates.Invisible;

            var comboBox = (Spinner)FindViewById(Resource.Id.divisionComboBox);
            comboBox.SetSelection(0);

            DivisionTeamSelector.Instance.ResetAll();
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
            int divisionNum = DivisionTeamSelector.Instance.DrawDivision();
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
                var teams = DivisionTeamSelector.Instance.GetTeamSelection(_division);
                teamList.Text = _currentTeamList = String.Join(Environment.NewLine, teams);
                teamList.Visibility = ViewStates.Visible;

                Button resetButton = FindViewById<Button>(Resource.Id.ResetButton);
                NewRound(resetButton);

                DivisionTeamSelector.Instance.ResetPlayerDraws();
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

            var selectedTeam = DivisionTeamSelector.Instance.DrawTeam(_division, playerNum);
            if (selectedTeam != string.Empty)
            {
                ChangePlayerTeamProperties($"Player {playerNum} - {selectedTeam}", true, playerTeamResourceId, color);
                ChangeHideButtonProperties(HideText, true, playerTeamHideButtonResourceId);
            }
            else
                ChangePlayerTeamProperties($"Could not find team", true, playerTeamResourceId, Color.Black);

            button.Visibility = ViewStates.Invisible;

            if (_numberOfPlayerDrawed == MaxPlayer)
            {
                Button resetButton = FindViewById<Button>(Resource.Id.ResetButton);
                resetButton.Visibility = ViewStates.Visible;
                Button gamesDrawButton = FindViewById<Button>(Resource.Id.GamesDraw);
                gamesDrawButton.Visibility = ViewStates.Visible;
            }
        }

        private void ResetTeamsDraw(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            NewRound(button);
        }

        private void NewRound(Button resetButton)
        {
            resetButton.Visibility = ViewStates.Invisible;
            Button gamesDrawButton = FindViewById<Button>(Resource.Id.GamesDraw);
            gamesDrawButton.Visibility = ViewStates.Invisible;
            TextView teamList = FindViewById<TextView>(Resource.Id.teamList);
            teamList.Text = _currentTeamList;
            _numberOfPlayerDrawed = 0;
            TogglePlayerDrawButtonsVisibility(true);
            HidePlayerTeamsText();
            DivisionTeamSelector.Instance.ResetTeamsDraw();
        }

        private void GamesDraw(object sender, EventArgs e)
        {
            string gamesDraw = DivisionTeamSelector.Instance.DrawGames();
            TextView teamList = FindViewById<TextView>(Resource.Id.teamList);
            teamList.Text = gamesDraw;
        }

        private void HidePlayerTeamsText()
        {
            ChangePlayerTeamProperties(string.Empty, false, Resource.Id.player1Team, Color.Black);
            ChangePlayerTeamProperties(string.Empty, false, Resource.Id.player2Team, Color.Black);
            ChangePlayerTeamProperties(string.Empty, false, Resource.Id.player3Team, Color.Black);
            ChangePlayerTeamProperties(string.Empty, false, Resource.Id.player4Team, Color.Black);
            ChangeHideButtonProperties(HideText, false, Resource.Id.player1TeamHide);
            ChangeHideButtonProperties(HideText, false, Resource.Id.player2TeamHide);
            ChangeHideButtonProperties(HideText, false, Resource.Id.player3TeamHide);
            ChangeHideButtonProperties(HideText, false, Resource.Id.player4TeamHide);
        }

        private void ChangePlayerTeamProperties(string text, bool visible, int resourceId, Color color)
        {
            TextView playerTeam = FindViewById<TextView>(resourceId);
            playerTeam.Text = text;
            playerTeam.Visibility = visible? ViewStates.Visible : ViewStates.Invisible;
            playerTeam.SetTextColor(color);
        }

        private void ChangeHideButtonProperties(string text, bool visible, int resourceId)
        {
            Button hideButton = FindViewById<Button>(resourceId);
            hideButton.Text = text;
            hideButton.Visibility = visible ? ViewStates.Visible : ViewStates.Invisible;
        }
    }
}
