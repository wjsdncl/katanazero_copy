using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{

    private void Start()
    {
        StartSetting();
    }

    private void Update()
    {
        FollowingMouse();
    }

    [SerializeField] bool isPointerOn = false;
    [SerializeField] Vector2 mousePos;
    [SerializeField] GameObject mouseImg = null;
    
    private void StartSetting()
    {
        mouseImg.SetActive(isPointerOn);
    }

    private void FollowingMouse()
    {
        
        if (isPointerOn)
        {
            // 마우스 위치 저장
            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseImg.transform.position = mousePos;
        }
    }
}
