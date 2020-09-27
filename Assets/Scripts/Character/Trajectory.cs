using UnityEngine;

namespace Character
{
    public class Trajectory : MonoBehaviour
    {
        [Header("Components")]
        public GameObject dotsParent;
        public GameObject dotsPrefab;
        
        private Movement _movement;

        [SerializeField] private int dotsNumber = 20;
        [SerializeField] private float dotSpacing = 0.03f;
        [SerializeField] [Range(0.01f, 0.3f)] private float dotMinScale = 0.1f;
        [SerializeField] [Range(0.3f, 1f)] private float dotMaxScale = 0.3f;

        private Transform[] _dotList;

        private Vector2 _pos;

        private float _timeStamp;


        void Start()
        {
            _movement = FindObjectOfType<Movement>();
            Hide();
            PrepareDots();
        }
        
        public void PrepareDots()
        {
            _dotList = new Transform[dotsNumber];
            dotsPrefab.transform.localScale = Vector3.one * dotMaxScale;

            var scale = dotMaxScale;
            var scaleFactor = scale / dotsNumber;

            for (int i = 0; i < dotsNumber; i++)
            {
                _dotList[i] = Instantiate(dotsPrefab, null).transform;
                _dotList[i].parent = dotsParent.transform;

                _dotList[i].localScale = Vector3.one * scale;
                if (scale > dotMinScale)
                {
                    scale -= scaleFactor;
                }
            }
        }

        public void UpdateDots(Vector3 grenadePos, Vector2 forceApplied)
        {
            _timeStamp = dotSpacing;
            if (_movement.IsFacingRight)
            {
                for (int i = 0; i < dotsNumber; i++)
                {
                    _pos.x = (grenadePos.x + forceApplied.x * _timeStamp);
                    _pos.y = (grenadePos.y + forceApplied.y * _timeStamp) -
                            (Physics2D.gravity.magnitude * _timeStamp * _timeStamp) / 2f;

                    _dotList[i].position = _pos;
                    _timeStamp += dotSpacing;
                }
            }
            else
            {
                for (int i = 0; i < dotsNumber; i++)
                {
                    _pos.x = ((grenadePos.x) + forceApplied.x * _timeStamp * (-1));
                    _pos.y = ((grenadePos.y) + forceApplied.y * _timeStamp) -
                            (Physics2D.gravity.magnitude * _timeStamp * _timeStamp) / 2f;

                    _dotList[i].position = _pos;
                    _timeStamp += dotSpacing;
                }
            }
        }

        public void Show()
        {
            dotsParent.SetActive(true);
        }

        public void Hide()
        {
            dotsParent.SetActive(false);
        }
    }
}