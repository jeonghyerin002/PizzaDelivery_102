using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    // �̱��� ����: �� �Ŵ����� ��𼭵� ���� ������ �� �ְ� ��
    public static MenuManager Instance;

    // ���� ���� �ִ� ��� �޴� �ִϸ����͵��� ������ ����Ʈ
    private List<MenuAnimator> allMenus = new List<MenuAnimator>();

    void Awake()
    {
        // �Ŵ����� �� �ϳ��� �����ϵ��� ����
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // �޴��� ���۵� �� �ڽ��� �Ŵ������� ����ϴ� �Լ�
    public void RegisterMenu(MenuAnimator menu)
    {
        if (!allMenus.Contains(menu))
        {
            allMenus.Add(menu);
        }
    }

    // Ư�� �޴��� ���� �� ȣ��Ǵ� �Լ�
    public void OnMenuOpened(MenuAnimator openedMenu)
    {
        // ��ϵ� ��� �޴����� Ȯ��
        foreach (MenuAnimator menu in allMenus)
        {
            // ��� ���� �޴��� �ƴ϶��
            if (menu != openedMenu)
            {
                // ������� ���!
                menu.CloseMenu();
            }
        }
    }
}