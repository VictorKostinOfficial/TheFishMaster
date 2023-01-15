using UnityEngine;
using DG.Tweening;
using System;

public class Fish : MonoBehaviour
{
    private Fish.FishType type;
    private CircleCollider2D coll;
    private SpriteRenderer rend;
    private float screenLeft;
    private Tweener tweener;

    public Fish.FishType Type
    {
        get
        {
            return type;
        }
        
        set
        {
            type = value;
            coll.radius = type.colliderRadius;
            rend.sprite = type.sprite;
        }
    }

    void Awake()
    {
        coll = GetComponent<CircleCollider2D>();
        rend = GetComponentInChildren<SpriteRenderer>();
        screenLeft = Camera.main.ScreenToWorldPoint(Vector3.zero).x;
    }

    public void ResetFish()
    {
        if(tweener != null)
            tweener.Kill(false);

        float deepOfFish = UnityEngine.Random.Range(type.minLength, type.maxLength);
        coll.enabled = true;

        Vector3 position = transform.position;
        position.y = deepOfFish;
        position.x = screenLeft;
        transform.position = position;

        float borderBound = 1;
        float y = UnityEngine.Random.Range(deepOfFish - borderBound, deepOfFish + borderBound);
        Vector2 v = new Vector2(-position.x, y);

        float duration = 3;
        float delay = UnityEngine.Random.Range(0, 2 * duration);
        tweener = transform.DOMove(v, duration, false).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear).SetDelay(delay).OnStepComplete(delegate
        {
            Vector3 localScale = transform.localScale;
            localScale.x = -localScale.x;
            transform.localScale = localScale;
        });
    }

    public void Hooked()
    {
        coll.enabled = false;
        tweener.Kill(false);
    }

    [Serializable]
    public class FishType
    {
        public int price;
        public float fishCount;
        public float minLength;
        public float maxLength;
        public float colliderRadius;
        public Sprite sprite;
    }
}
