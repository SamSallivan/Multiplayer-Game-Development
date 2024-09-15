using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float _lifeTime = 0f;
    public float maxLifeTime = 5f;
    public float damage = 5f;
    
    void Update()
    {
        _lifeTime += Time.deltaTime;

        if (_lifeTime >= maxLifeTime)
        {
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<PlayerController>(out var player))
        {
            player.Damage(damage);
        }
        
        Destroy(gameObject);
    }
}
