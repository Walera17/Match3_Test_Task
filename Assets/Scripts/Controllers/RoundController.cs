using System;
using System.Collections;
using BoardCode;
using Commons;
using UI;
using UnityEngine;
using UnityEngine.Events;

namespace Controllers
{
    public class RoundController : MonoBehaviour
    {
        public static event UnityAction OnLevelCompleted;
        public static event UnityAction<bool> OnActiveMenu;

        [SerializeField] private Board board;
        [SerializeField] private UIManager uiMan;
        [SerializeField] private GameObject menuBackground;
        [SerializeField] private float roundTime = 60f;
        [SerializeField] private float scoreSpeed = 5f;

        private bool endingRound;

        private float currentTime;
        private int currentScore;
        private float displayScore;

        private void OnEnable()
        {
            board.OnAddScore += Board_OnAddScore;
            board.OnAddTimeBonus += Board_OnAddTimeBonus;
        }

        private void OnDisable()
        {
            board.OnAddScore -= Board_OnAddScore;
            board.OnAddTimeBonus -= Board_OnAddTimeBonus;
        }

        private void Start()
        {
            menuBackground.SetActive(true);
            OnActiveMenu?.Invoke(true);
        }

        void Update()
        {
            if (currentTime > 0)
            {
                currentTime -= Time.deltaTime;

                if (currentTime <= 0)
                {
                    currentTime = 0;

                    endingRound = true;
                }

                uiMan.SetTime(currentTime);
            }

            if (Math.Abs(currentScore - displayScore) > float.Epsilon)
            {
                displayScore = Mathf.Lerp(displayScore, currentScore, scoreSpeed * Time.deltaTime);
                uiMan.SetScore(displayScore);
            }
        }

        public void StartGame()
        {
            endingRound = false;
            displayScore = currentScore = 0;
            uiMan.SetScore(displayScore);
            currentTime = roundTime;
            board.StartGame();
            menuBackground.SetActive(false);
            OnActiveMenu?.Invoke(false);
            StartCoroutine(CheckGameOver());
        }

        public void ExitGame()
        {
            menuBackground.SetActive(true);
            OnActiveMenu?.Invoke(true);
            board.ExitGame();
        }

        public void ShuffleBoard()
        {
            board.ShuffleBoard();
        }

        private IEnumerator CheckGameOver()
        {
            while (!endingRound || board.CurrentState == BoardState.Wait)
            {
                yield return null;
            }

            WinCheck();
            endingRound = false;
        }

        private void WinCheck()
        {
            uiMan.ShowOverGame(currentScore);
            board.ExitGame();

            if (currentScore > PlayerPrefs.GetInt("Score", 0))
            {
                PlayerPrefs.SetInt("Score", currentScore);
                uiMan.SetWinner("CONGRATULATIONS! YOU WIN!");
            }
            else
            {
                uiMan.SetWinner("ROUND OVER");
            }

            uiMan.SetRecord(PlayerPrefs.GetInt("Score", 0));

            OnLevelCompleted?.Invoke();
        }

        private void Board_OnAddScore(int score)
        {
            currentScore += score;
        }

        private void Board_OnAddTimeBonus()
        {
            endingRound = false;
            currentTime += roundTime / 4;
        }
    }
}