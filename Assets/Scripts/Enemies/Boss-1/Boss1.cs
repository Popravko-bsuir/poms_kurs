using System;
using System.Collections;
using System.Collections.Generic;
using Character;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Boss1 : MonoBehaviour
{
    [SerializeField] private int hp;
    [SerializeField] private float speed;
    public GameObject bombPrefab;
    public GameObject bulletPrefab;
    public Transform leftPosition;
    public Transform rightPosition;
    public Rigidbody2D rb;
    private bool _isFacingLeft = true;
    public Transform firePoint;
    [SerializeField] private float shootingTime;
    [SerializeField] private float shootingPause;
    private float _time;
    private GameObject[] _bulletList;
    [SerializeField] private int numberOfBullets;
    private int _bulletNumber;
    private bool _cantShoot;
    public Slider slider;
    public Animator animator;
    public PolygonCollider2D polygonCollider2D;
    private float _timeJop;
    public MenuManager menuManager;
    void Start()
    {
        StartCoroutine(BossBehaviour(leftPosition));
        InvokeRepeating("StartShoot", 3f, 6f);
        PrepareBullets();
    }

    private void StartShoot()
    {
        if (_cantShoot)
        {
            StartCoroutine(Shoot());
        }
    }

    private IEnumerator Shoot()
    {
        for (float t = 0; t < shootingTime; t += shootingPause)
        {
            var firePointLocalPosition = firePoint.localPosition;
            firePoint.localRotation = Quaternion.Euler(firePointLocalPosition.x, firePointLocalPosition.y,
                firePointLocalPosition.z + Random.Range(-30f, 30f));
            LaunchBullet();
            yield return new WaitForSeconds(shootingPause);
        }
    }

    private IEnumerator BossBehaviour(Transform positionToMove)
    {
        if (hp <= 10)
        {
            yield break;
        }

        _cantShoot = true;
        
        var stop = false;
        
        var startHp = hp;
        yield return new WaitUntil(() => hp <= startHp - 10);

        polygonCollider2D.enabled = false;
        
        _cantShoot = false;
        
        var transform1 = transform;
        
        var direction = ((Vector2)positionToMove.position - (Vector2)transform1.position);
        var startMagnitude = direction.magnitude;
        
      //  Instantiate(bombPrefab, transform1.position, transform1.rotation);
        
        while (direction.magnitude > 2f)
        {
            direction = ((Vector2)positionToMove.position - (Vector2)transform.position);
            rb.velocity = direction.normalized * speed;
            
            if (_timeJop > 0.7f)
            {
                var transform2 = transform;
                Instantiate(bombPrefab, transform2.position, transform2.rotation);
                _timeJop = 0;
            }

            _timeJop += 0.02f;
            // if (((Vector2)positionToMove.position - (Vector2)transform.position).magnitude <= startMagnitude/2 && !stop)
            // {
            //     var transform2 = transform;
            //     Instantiate(bombPrefab, transform2.position, transform2.rotation);
            //     stop = true;
            // }

            yield return new WaitForSeconds(0.02f);
        }

        polygonCollider2D.enabled = true;
        //var transform3 = transform; 
       // Instantiate(bombPrefab, transform3.position, transform3.rotation);

        Flip();
        rb.velocity = new Vector2(0, 0);
        StartCoroutine(BossBehaviour(_isFacingLeft ? leftPosition : rightPosition));
    }

    private void Flip()
    {
        transform.rotation = Quaternion.Euler(0, _isFacingLeft ? 0 : 180, -32f);
        _isFacingLeft = !_isFacingLeft;
    }
    
    private void PrepareBullets()
    {
        _bulletList = new GameObject[numberOfBullets];
        for (int i = 0; i < numberOfBullets; i++)
        {
            _bulletList[i] = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            _bulletList[i].GetComponent<Bullet>().SetEnemyToDamage(10); 
            _bulletList[i].SetActive(false);
        }
    }
    
    void LaunchBullet()
    {
        var firePointTransform = firePoint.transform;
        _bulletList[_bulletNumber].transform.position =  firePointTransform.position;
        _bulletList[_bulletNumber].transform.rotation = firePointTransform.rotation;
        _bulletList[_bulletNumber].SetActive(true);
        _bulletNumber++;
        if (_bulletNumber == numberOfBullets)
        {
            _bulletNumber = 0;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Bullet"))
        {
            return;
        }

        hp--;
        slider.value = hp;

        if (hp == 0)
        {
            StartCoroutine(DestroyBoss());
        }
    }

    private IEnumerator DestroyBoss()
    {
        polygonCollider2D.enabled = false; 
        animator.Play("Explosion");
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        menuManager.LoadMenu();
    }
}
