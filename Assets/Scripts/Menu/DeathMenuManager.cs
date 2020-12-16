using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Menu
{
    public class DeathMenuManager : MonoBehaviour
    {
        public GameObject deathMenu;
        public Image youDiedImage;

        public void ShowDeathScreen()
        {
            //Time.timeScale = 0;
            youDiedImage.gameObject.SetActive(true);
            StartCoroutine(ChangeAlpha(0, 1f, 60, 1f));
        }

        private IEnumerator ChangeAlpha(float from, float to, int steps, float duration) 
        {
            float diff = to - from;
            for (int i = 0; i < steps; i++) 
            {
                from += diff / steps;
                var color = youDiedImage.color;
                color = new Color(color.r, color.g, color.b, from);
                youDiedImage.color = color;
                yield return new WaitForSeconds(duration/steps);
            }

            var color1 = youDiedImage.color;
            color1 = new Color(color1.r, color1.g, color1.b, to);
            youDiedImage.color = color1;
            
            deathMenu.SetActive(true);
        }
    }
}
