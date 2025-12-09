using JetBrains.Annotations;
using TMPro;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    [Header("배달 설정")]
    public GameObject dronePrefab;

    // 플레이어 캐싱용 변수
    private Transform playerTransform;

    [Header("가격 설정")]
    public int drink10;
    public int drink30;
    public int drink60;

    [Header("UI 연결")]
    public TMP_Text textDrink10;
    public TMP_Text textDrink30;
    public TMP_Text textDrink60;
    public TMP_Text message;

    [Header("아이템 프리펩")]
    public GameObject drink10_;
    public GameObject drink30_;
    public GameObject drink60_;

    [Header("시스템 설정")]
    public Vector3 spawnOffset = new Vector3(0, 30f, -30f);
    public float purchaseCooldown = 2.0f; // 쿨타임
    private float lastPurchaseTime = -10f; // 마지막 구매 시간

    private void Start()
    {
        // UI 초기화
        UpdatePriceUI();

        // 플레이어
        GameObject playerObj = GameObject.FindGameObjectWithTag("OriginPlayer");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }
        else
        {
            Debug.LogError("ShopManager: 'OriginPlayer' 태그를 가진 오브젝트를 찾을 수 없습니다!");
        }
    }

    private void UpdatePriceUI()
    {
        if (textDrink10) textDrink10.text = $"비용: {drink10}";
        if (textDrink30) textDrink30.text = $"비용: {drink30}";
        if (textDrink60) textDrink60.text = $"비용: {drink60}";
    }

    public void PurchaseDrink10() => TryPurchase(drink10, drink10_);
    public void PurchaseDrink30() => TryPurchase(drink30, drink30_);
    public void PurchaseDrink60() => TryPurchase(drink60, drink60_);

    private void TryPurchase(int cost, GameObject itemPrefab)
    {
        // 쿨타임 체크
        if (Time.time < lastPurchaseTime + purchaseCooldown) return;

        // 이미 무제한 스태미나 상태인지 체크
        if (StaminaSystem.unlimitedStamina)
        {
            ShowMessage("이미 효과를 사용하고 있습니다", Color.red);
            return;
        }

        // 잔액 체크 및 구매
        if (myData.Coin >= cost)
        {
            lastPurchaseTime = Time.time; // 쿨타임 갱신
            myData.Coin -= cost;

            ShowMessage("구매 완료", Color.green);
            SpawnDeliveryDrone(itemPrefab);
        }
        else
        {
            ShowMessage("돈이 부족합니다!", Color.red);
        }
    }
    private void ShowMessage(string msg, Color color)
    {
        if (message != null)
        {
            message.text = msg;
            message.color = color;
        }
    }
    private void SpawnDeliveryDrone(GameObject itemToBuy)
    {
        if (playerTransform == null) return;

        // 위치 계산
        Vector3 startPos = playerTransform.position + (playerTransform.rotation * spawnOffset);
        Quaternion startRot = Quaternion.LookRotation(playerTransform.position - startPos);

        // 드론 생성
        GameObject droneObj = Instantiate(dronePrefab, startPos, startRot);

        // 배달 시작
        DeliveryDrone droneScript = droneObj.GetComponent<DeliveryDrone>();
        if (droneScript != null)
        {
            droneScript.StartDelivery(itemToBuy, playerTransform);
        }
    }
}