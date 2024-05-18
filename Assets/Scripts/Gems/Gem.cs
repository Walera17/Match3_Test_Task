using BoardCode;
using Commons;
using Controllers;
using UnityEngine;
using UnityEngine.Events;

namespace Gems
{
    public class Gem : MonoBehaviour
    {
        public static event UnityAction<Gem> OnClick;                           // событие при клике на любом кристалле(передает параметром текущий кристалл)

        [SerializeField] protected SpriteRenderer gemSprite;
        [SerializeField] private ParticleController particleController;
        [SerializeField] protected GemType type;
        [SerializeField] protected int scoreValue = 10;

        [HideInInspector] public Vector2Int posIndex;

        public GemType Type => type;

        public int ScoreValue => scoreValue;

        private bool isMove;

        private void OnEnable()
        {
            gemSprite.enabled = true;
        }

        void Update()
        {
            if (isMove)
                Move();
        }

        private void OnMouseDown()
        {
            OnClick?.Invoke(this);
        }

        /// <summary>
        /// Настройка положения перемещения и запуск движения
        /// </summary>
        /// <param name="pos"></param>
        public void SetupMovePosition(Vector2Int pos)
        {
            posIndex = pos;
            isMove = true;
        }

        private void DisableView()
        {
            gemSprite.enabled = false;
        }

        private void Move()
        {
            if ((posIndex - (Vector2)transform.position).sqrMagnitude > .0001f)
            {
                transform.position = Vector2.Lerp(transform.position, posIndex, Board.SpeedGem * Time.deltaTime);
            }
            else
            {
                transform.position = new Vector3(posIndex.x, posIndex.y, 0f);
                isMove = false;
            }
        }
        
        public void RemoveGem()
        {
            DisableView();
            particleController.CreateBurst();
        }
    }
}