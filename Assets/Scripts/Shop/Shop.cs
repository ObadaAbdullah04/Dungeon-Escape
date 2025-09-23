using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    public GameObject shopPanel;
    public GameObject WinPanel;

    public int currentSelectedItem;
    public int currentItemCost;

    private Player _player;
    private void Start()
    {
        shopPanel.SetActive(false);
        WinPanel.SetActive(false);
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        _player = other.GetComponent<Player>();

        if (_player != null)
        {
            UIManager.Instance.OpenShop(_player.diamonds);
            UIManager.Instance.PlayButton1Sound();
        }
        shopPanel.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        UIManager.Instance.PlayButton2Sound();
        shopPanel.SetActive(false);
    }

    public void SelectItem(int item)
    {
        // 1 = flame sword
        // 2 = flight boots
        // 3 = Key
        Debug.Log("Item: "+ item);

        switch (item)
        {
            case 0:
                UIManager.Instance.UpdateShopSelection(140);
                currentSelectedItem = 0;
                currentItemCost = 1000;
                break;
            case 1:
                UIManager.Instance.UpdateShopSelection(25);
                currentSelectedItem = 1;
                currentItemCost = 1000;
                break;
            case 2:
                UIManager.Instance.UpdateShopSelection(-95);
                currentSelectedItem = 2;
                currentItemCost = 75;
                break;
        }
    }
    public void BuyItem()
    {
        if (_player == null)
        {
            Debug.LogWarning("Player not found. Cannot buy item.");
            return;
        }



        if (_player.diamonds >= currentItemCost)
        {
            Debug.Log("Purchased item: " + currentSelectedItem);
            _player.diamonds -= currentItemCost;
            UIManager.Instance.OpenShop(_player.diamonds);

            if (currentSelectedItem == 2)
            {
                GameManager.Instance.HasKeyToCastle = true;
                if (GameManager.Instance.HasKeyToCastle == true)
                {
                    UIManager.Instance.PlayPurchaseSound();
                    StartCoroutine(Delay());
                }
            }
        }
        else
        {
            UIManager.Instance.PlayButton2Sound();
            Debug.Log("Not enough diamonds!");
        }
    }
    IEnumerator Delay()
    {
        yield return new WaitForSeconds(UIManager.Instance.PurchaseClip.length);
        WinPanel.SetActive(true);
    }
}
