using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    // 싱글톤 패턴: 이 매니저를 어디서든 쉽게 접근할 수 있게 함
    public static MenuManager Instance;

    // 현재 씬에 있는 모든 메뉴 애니메이터들을 저장할 리스트
    private List<MenuAnimator> allMenus = new List<MenuAnimator>();

    void Awake()
    {
        // 매니저가 단 하나만 존재하도록 보장
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 메뉴가 시작될 때 자신을 매니저에게 등록하는 함수
    public void RegisterMenu(MenuAnimator menu)
    {
        if (!allMenus.Contains(menu))
        {
            allMenus.Add(menu);
        }
    }

    // 특정 메뉴가 열릴 때 호출되는 함수
    public void OnMenuOpened(MenuAnimator openedMenu)
    {
        // 등록된 모든 메뉴들을 확인
        foreach (MenuAnimator menu in allMenus)
        {
            // 방금 열린 메뉴가 아니라면
            if (menu != openedMenu)
            {
                // 닫으라고 명령!
                menu.CloseMenu();
            }
        }
    }
}