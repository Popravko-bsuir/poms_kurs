using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Environment
{
    public class SecretsController : MonoBehaviour
    {
        public Tilemap secretLayer;
        [SerializeField] private float time;
    

        private IEnumerator ChangeSecretAlpha(float startValue, float newValue)
        {
            var tempColor = secretLayer.color;
            var t = 0f;
            //var i = 0;
            while (t <= 1f)
            {
                //i++;    
                t += Time.deltaTime / time;
                tempColor.a = Mathf.Lerp(startValue, newValue, t);
                secretLayer.color = tempColor;
                yield return null;
                yield return new WaitForSeconds(0.01f);
            }
            //Debug.Log(i);
        }

        // private IEnumerator ChangeAlpha(float from, float to, int steps, float duration) {
        //     float diff = to - from;
        //     for (int i = 0; i < steps; i++) {
        //         from += diff / steps;
        //         secretLayer.color = new Color(secretLayer.color.r, secretLayer.color.g, secretLayer.color.b, from);
        //         yield return new WaitForSeconds(duration/steps);
        //     }
        //
        //     secretLayer.color = new Color(secretLayer.color.r, secretLayer.color.g, secretLayer.color.b, to);
        // }
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.gameObject.CompareTag("Player"))
            {
                return;
            }

            StartCoroutine(ChangeSecretAlpha(1f, 0f));
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.gameObject.CompareTag("Player"))
            {
                return;
            }

            StartCoroutine(ChangeSecretAlpha(0f, 1f));
        }
    }
}
