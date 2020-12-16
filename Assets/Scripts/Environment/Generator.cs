using System;
using System.Collections;
using Cinemachine.Utility;
using UnityEngine;

namespace Environment
{
    public class Generator : MonoBehaviour
    {
        [SerializeField] private float speed;
        public SpriteRenderer spriteRenderer;
        public Sprite brokenGenerator;
        public ParticleSystem electricityEffect;
        public ParticleSystem smokeEffect;
        public BoxCollider2D trigger;
        public Transform door;
        private readonly Vector3 _doorOffset = new Vector3(0, -4f, 0);

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Bullet"))
            {
                electricityEffect.Play(false);
                smokeEffect.Stop(false);
                spriteRenderer.sprite = brokenGenerator;
                trigger.enabled = false;
                StartCoroutine(MoveDoor());
                // var doorPosition = door.position;
                // door.position = Vector3.Lerp(doorPosition, doorPosition + _doorOffset, 3f); // 3.94 0.06
            }
        }

        private IEnumerator MoveDoor()
        {
            var doorPosition = door.position + _doorOffset;
            while (door.position.y >= doorPosition.y)
            {
                door.Translate(-transform.up * speed);
                yield return new WaitForSeconds(0.02f);
            }
        }
    }
}
