using UnityEngine;

namespace Character
{
    public class Trajectory : MonoBehaviour
    {
        public int dotsNumber;
        public GameObject dotsParent;
        public GameObject dotsPrefab;
        public float dotSpacing;
        [Range(0.01f, 0.3f)] public float dotMinScale;
        [Range(0.3f, 1f)] public float dotMaxScale;

        public Transform[] dotList;

        public Vector2 pos;

        public float timeStamp;


        void Start()
        {
            Hide();
            PrepareDots();
        }

        void Update()
        {
        }

        public void PrepareDots()
        {
            dotList = new Transform[dotsNumber];
            dotsPrefab.transform.localScale = Vector3.one * dotMaxScale;

            var scale = dotMaxScale;
            var scaleFactor = scale / dotsNumber;

            for (int i = 0; i < dotsNumber; i++)
            {
                dotList[i] = Instantiate(dotsPrefab, null).transform;
                dotList[i].parent = dotsParent.transform;

                dotList[i].localScale = Vector3.one * scale;
                if (scale > dotMinScale)
                {
                    scale -= scaleFactor;
                }
            }
        }

        public void UpdateDots(Vector3 grenadePos, Vector2 forceApplied)
        {
            timeStamp = dotSpacing;
            if (Movement.isFacingRight)
            {
                for (int i = 0; i < dotsNumber; i++)
                {
                    pos.x = (grenadePos.x + forceApplied.x * timeStamp);
                    pos.y = (grenadePos.y + forceApplied.y * timeStamp) -
                            (Physics2D.gravity.magnitude * timeStamp * timeStamp) / 2f;

                    dotList[i].position = pos;
                    timeStamp += dotSpacing;
                }
            }
            else
            {
                for (int i = 0; i < dotsNumber; i++)
                {
                    pos.x = ((grenadePos.x) + forceApplied.x * timeStamp * (-1));
                    pos.y = ((grenadePos.y) + forceApplied.y * timeStamp) -
                            (Physics2D.gravity.magnitude * timeStamp * timeStamp) / 2f;

                    dotList[i].position = pos;
                    timeStamp += dotSpacing;
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