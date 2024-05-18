using System.Collections.Generic;
using Controllers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] RoundController roundController;
        [SerializeField] private Button startButton;
        [SerializeField] private Button quitButton;
        [SerializeField] private Button shuffleButton;
        [SerializeField] private Button pauseButton;
        [SerializeField] private Button exitButton;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button menuButton;
        [SerializeField] private TMP_Text pause;
        [SerializeField] private TMP_Text timeText;
        [SerializeField] private TMP_Text scoreText;
        [SerializeField] private TMP_Text winScore;
        [SerializeField] private TMP_Text winText;
        [SerializeField] private TMP_Text recordText;
        [SerializeField] private GameObject menuScreen;
        [SerializeField] private GameObject gameScreen;
        [SerializeField] private GameObject roundOverScreen;


        private readonly List<GameObject> menus = new();

        private void Start()
        {
            SetupButton();
            SetupMenu();
        }

        public void SetTime(float roundTime)
        {
            timeText.text = roundTime.ToString("0.0") + "s";
        }

        public void SetScore(float displayScore)
        {
            scoreText.text = displayScore.ToString("0");
        }

        public void SetRecord(int getInt)
        {
            recordText.text = getInt.ToString();
        }

        public void SetWinner(string title)
        {
            winText.text = title;
        }

        public void ShowOverGame(int currentScore)
        {
            ShowMenu(roundOverScreen);
            winScore.text = currentScore.ToString("0");
        }

        private void SetupButton()
        {
            startButton.onClick.AddListener(StartGame);
            quitButton.onClick.AddListener(QuitGame);
            pauseButton.onClick.AddListener(PauseUnpause);
            shuffleButton.onClick.AddListener(ShuffleBoard);
            exitButton.onClick.AddListener(Menu);
            restartButton.onClick.AddListener(StartGame);
            menuButton.onClick.AddListener(Menu);
        }

        private void SetupMenu()
        {
            menus.Add(menuScreen);
            menus.Add(gameScreen);
            menus.Add(roundOverScreen);
            ShowMenu(menuScreen);
        }

        private void Menu()
        {
            roundController.ExitGame();
            SetupModeGame();
            ShowMenu(menuScreen);
        }

        private void ShowMenu(GameObject showMenu)
        {
            foreach (GameObject menu in menus)
            {
                menu.SetActive(menu == showMenu);
            }
        }

        private void StartGame()
        {
            roundController.StartGame();
            ShowMenu(gameScreen);
        }

        private void PauseUnpause()
        {
            if (Time.timeScale == 0f)
            {
                SetupModeGame();
            }
            else
            {
                pause.text = "Continue";
                Time.timeScale = 0f;
                shuffleButton.interactable = false;
            }
        }

        private void SetupModeGame()
        {
            pause.text = "Pause";
            Time.timeScale = 1f;
            shuffleButton.interactable = true;
        }

        private void ShuffleBoard()
        {
            roundController.ShuffleBoard();
        }

        private void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}