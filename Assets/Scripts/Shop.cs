using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Shop : MonoBehaviour
{
    public GameObject loadedEnemies;

    public static bool visible;

    private static Dictionary<int, Func<Consumable>> consumableFactories;

    private void Awake()
    {
        this.transform.GetChild(0).gameObject.SetActive(false);
        registerConsumables();
    }

    public void displayShop()
    {
        if(visible == false)
        {
            if (loadedEnemies.transform.childCount == 0)
            {
                this.transform.GetChild(0).gameObject.SetActive(true);
                visible = true;
                Player.S.CanMove = false;
            }
            else
                FlavorText.flavorText.showText("\"It's not safe to shop here...\"");
        }
    }

    public void hideShop()
    {
        this.transform.GetChild(0).gameObject.SetActive(false);
        visible = false;
        Player.S.CanMove = true;
    }
    private static void registerConsumables() //Register new consumables here
    {
        consumableFactories = new Dictionary<int, Func<Consumable>>
        {
            { 0, () => new HPRestore() } ,
            { 1, () => new HPRegen() }
        };

    }

    public void purchaseConsumable(int consumID) //Attempts purchase of consumable registered during registerConsumables()
    {
        if(consumableFactories.TryGetValue(consumID, out var factory))
        {
            Consumable consumable = factory();
            if(Player.threads >= consumable.price)
            {
                Player.threads -= consumable.price;
                Player.playerConsumables.Add(consumable);
            }
        }
    }
}

public abstract class Consumable //Define custom consumables here
{
    public abstract void consume();
    public abstract int price
    {
        get;
    }
}

public class HPRestore : Consumable
{
    public override int price
    {
        get { return 20; }
    }
    public override void consume()
    {
        Player.S.HP += 30;
        if(Player.S.HP > Player.S.MaxHP)
        {
            Player.S.HP = Player.S.MaxHP;
        }
        FlavorText.flavorText.showText("\"I feel rejuvinated.\"");
        Player.playerConsumables.RemoveAt(0);
    }
}

public class HPRegen : Consumable
{
    public override int price
    {
        get { return 40; }
    }
    public override void consume()
    {
        Player.setRegen(0.5f, 100);
        FlavorText.flavorText.showText("\"My wounds are mending.\"");
        Player.playerConsumables.RemoveAt(0);
    }
}