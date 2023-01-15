using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Hook : MonoBehaviour
{
    public Transform hookedTransform;
    private Camera mainCamera;
    private Tweener cameraTween;
    private Collider2D hookColl;

    private int hookLength;
    private int hookStrength;
    private int fishCount;
    //private float timeDown;
    
    private bool hookIsMoveable = false;
    //private bool fishingIsStarted = false;
    //private bool fishingIsAction = false;

    private List<Fish> hookedFishes;

    private int screenHookLength = 20;
    private float endGameBoard = -25.0f;
    private float hookStartedPosition = -6.0f;
    private float hookTransformBoard = -11.0f;

    private void Awake() 
    {
        mainCamera = Camera.main;
        hookColl = GetComponent<Collider2D>();
        hookedFishes = new List<Fish>();
    }

    void Update()
    {
        if(hookIsMoveable && Input.GetMouseButton(0))
        {
            Vector3 vector = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            Vector3 position = transform.position;
            position.x = vector.x;
            transform.position = position;
        }

        //if(fishingIsStarted)
        //{
        //    if(mainCamera.transform.position.y >= hookLength)
        //    {
        //        Vector3 position = mainCamera.transform.position;
        //        position.y -= timeDown;
        //        mainCamera.transform.position = position;
        //        if(mainCamera.transform.position.y <= -11)
        //        {
        //            transform.SetParent(mainCamera.transform);
        //        }
        //        print(position.y);
        //    }
        //    else
        //    {
        //        fishingIsStarted = false;
        //        fishingIsAction = true;
        //        hookIsMoveable = true;
        //        hookColl.enabled = true;
        //    }       
        //}
        //
        //if(fishingIsAction)
        //{
        //    if(mainCamera.transform.position.y <= 0)
        //    {
        //        Vector3 position = mainCamera.transform.position;
        //        position.y += timeDown * 0.1f;
        //        mainCamera.transform.position = position;
        //        if(mainCamera.transform.position.y >= -25)
        //        {
        //            hookIsMoveable = false;
        //            hookColl.enabled = false;
        //            StopFishing();
        //        }
        //        print(position.y);
        //    }
        //    else
        //    {
        //        hookIsMoveable = false;
        //    }    
        //}
    }

    public void StartFishing()
    {
        hookLength = IdleManager.instance.length - screenHookLength;
        hookStrength = IdleManager.instance.strength;
        fishCount = 0;
        float time = (-hookLength) * 0.1f;
        //fishingIsStarted = true;
    
        cameraTween = mainCamera.transform.DOMoveY(hookLength, 1 + time * 0.25f, false).OnUpdate(delegate
        {
            if(mainCamera.transform.position.y <= hookTransformBoard)
                transform.SetParent(mainCamera.transform);
        }).OnComplete(delegate
        {
            hookColl.enabled = true;
            cameraTween = mainCamera.transform.DOMoveY(0, time * 5, false).OnUpdate(delegate
            {
                if(mainCamera.transform.position.y >= endGameBoard)
                    StopFishing();
            });
        });

        ScreensManager.instance.ChangeScreen(ScreensManager.Screens.GAME);
        hookColl.enabled = false;
        hookIsMoveable = true;
        hookedFishes.Clear();
    }

    void StopFishing()
    {
        hookIsMoveable = false;
        cameraTween.Kill(false);
        cameraTween = mainCamera.transform.DOMoveY(0,2,false).OnUpdate(delegate
        {
            if(mainCamera.transform.position.y >= hookTransformBoard)
            {
                transform.SetParent(null);
                transform.position = new Vector2(transform.position.x, hookStartedPosition);
            }
        }).OnComplete(delegate
        {
            transform.position = Vector2.up * hookStartedPosition;
            hookColl.enabled = true;
            int fishCost = 0;

            for(int i = 0; i < hookedFishes.Count; i++)
            {
                hookedFishes[i].transform.SetParent(null);
                hookedFishes[i].ResetFish();
                fishCost += hookedFishes[i].Type.price;
            }

            IdleManager.instance.totalGain = fishCost;
            ScreensManager.instance.ChangeScreen(ScreensManager.Screens.END);
        });
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.CompareTag("Fish") && fishCount != hookStrength)
        {
            fishCount++;
            Fish component = other.GetComponent<Fish>();
            component.Hooked();
            hookedFishes.Add(component);
            other.transform.SetParent(transform);
            other.transform.position = hookedTransform.position;
            other.transform.rotation = hookedTransform.rotation;
            other.transform.localScale = Vector3.one;

            other.transform.DOShakeRotation(5, Vector3.forward * 45, 10, 90, false).SetLoops(1, LoopType.Yoyo).OnComplete(delegate
            {
                other.transform.rotation = Quaternion.identity;
            });

            if(fishCount == hookStrength)
                StopFishing();
        }
    }
}
