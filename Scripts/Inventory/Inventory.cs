using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour {

   public GameObject InventoryPlace;
   public GameObject SavePanel;
   public GameObject ItemPrefab;
   public GameObject ItemInfoPrefab;

   private void OnMouseUpAsButton()
   {
      if (InventoryPlace.active) {
         InventoryPlace.active = false;
         SavePanel.active = false;
      }
      else {
         InventoryPlace.transform.Find ("Hero/LevelPlace/XPImage/Text").GetComponent<Text>().text = Global.XP + "/" + Global.XPForLevel;
         InventoryPlace.transform.Find ("Hero/LevelPlace/XPImage").GetComponent<Image>().fillAmount = (float)(Global.XP) / (float)(Global.XPForLevel);
         InventoryPlace.transform.Find ("Hero/HeroCanvas/HeroPlace/HP/Text").GetComponent<Text>().text = (Global.HP).ToString ();
         InventoryPlace.transform.Find ("Hero/HeroCanvas/HeroPlace/DMG/Text").GetComponent<Text>().text = (Global.DMG).ToString();
         InventoryPlace.transform.Find ("Hero/HeroCanvas/HeroPlace/Level/Text").GetComponent<Text>().text = (Global.Level).ToString();
         InventoryPlace.transform.Find ("Hero/MoneyPlace/Text").GetComponent<Text> ().text = (Global.Money).ToString ();

         for (int i = 0; i < Global.QuantityItems; i++) {
            GameObject Item;
            if ((int)(Global.Items[i].GetField ("ContainerID").n) != 1) {
               Item = Instantiate (ItemPrefab, new Vector3 (0, 0, 0), Quaternion.identity);
               switch ((int)(Global.Items[i].GetField ("ContainerID").n)) {
                  case 2:
                     Item.transform.parent = InventoryPlace.transform.Find ("Hero/HeroCanvas/Helmet");
                     break;
                  case 3:
                     Item.transform.parent = InventoryPlace.transform.Find ("Hero/HeroCanvas/Chest");
                     break;
                  case 4:
                     Item.transform.parent = InventoryPlace.transform.Find ("Hero/HeroCanvas/MainWeapon");
                     break;
                  case 5:
                     Item.transform.parent = InventoryPlace.transform.Find ("Hero/HeroCanvas/AddWeapon");
                     break;
                  case 6:
                     Item.transform.parent = InventoryPlace.transform.Find ("Hero/HeroCanvas/Legs");
                     break;
                  case 7:
                     Item.transform.parent = InventoryPlace.transform.Find ("Hero/HeroCanvas/Foots");
                     break;
                  case 8:
                     Item.transform.parent = InventoryPlace.transform.Find ("Hero/HeroCanvas/FirstAccessory");
                     break;
                  case 9:
                     Item.transform.parent = InventoryPlace.transform.Find ("Hero/HeroCanvas/SecondAccessory");
                     break;
                  default:
                     Destroy (Item);
                     continue;
                     break;
               }
               Item.transform.localPosition = new Vector3 (0, 0, 0);
               Item.transform.localScale = new Vector3 (1, 1, 1);
               Item.transform.Find ("Quantity").GetComponent<Text> ().text = "";
               Item.GetComponent<Image> ().sprite = Resources.Load<Sprite> ("Sprites/" + GetIcon ((int)(Global.Items[i].GetField ("ID").n)));
               Item.GetComponent<Item> ().ID = (int)(Global.Items[i].GetField ("ID").n);
               Item.GetComponent<Item> ().ContainerID = (int)(Global.Items[i].GetField ("ContainerID").n);
               Item.GetComponent<Item> ().InfoPrefab = ItemInfoPrefab;
            }
         }

         InventoryPlace.active = true;

         OpenPage (0);
                  
         SavePanel.active = true;
      }
   }

   public void OpenPage (int Number)
   {
      foreach (GameObject Item in GameObject.FindGameObjectsWithTag("Item")) {
         if (Item.transform.parent.name == "Items") Destroy (Item);
      }
      for (int i = (9 * Number); i < Global.QuantityItems && i < (9 * (Number+1)); i++) {

         int TempX = -113;
         int TempY = 135;
         switch (i) {
            case 0:
            case 3:
            case 6:
               TempX = -113;
               break;
            case 1:
            case 4:
            case 7:
               TempX = 0;
               break;
            case 2:
            case 5:
            case 8:
               TempX = 113;
               break;
         }
         switch (i) {
            case 0:
            case 1:
            case 2:
               TempY = 135;
               break;
            case 3:
            case 4:
            case 5:
               TempY = 23;
               break;
            case 6:
            case 7:
            case 8:
               TempY = -89;
               break;
         }
         
         GameObject Item;
         Item = Instantiate (ItemPrefab, new Vector3 (TempX, TempY, 0), Quaternion.identity);
         Item.transform.parent = (GameObject.Find("Location/Hero/Main Camera/Canvas/InventoryPlace/Items")).transform;
         Item.transform.localPosition = new Vector3 (TempX, TempY, 0);
         Item.transform.localScale = new Vector3 (1, 1, 1);
         if ((int)(Global.Items[i].GetField ("ContainerID").n) != 1) Item.transform.Find ("Quantity").GetComponent<Text> ().text = ((Global.Items[i].GetField ("Quantity").n) - 1).ToString ();
         else Item.transform.Find ("Quantity").GetComponent<Text> ().text = Global.Items[i].GetField ("Quantity").str;
         Item.GetComponent<Image> ().sprite = Resources.Load<Sprite> ("Sprites/" + GetIcon ((int)(Global.Items[i].GetField ("ID").n)));
         Item.GetComponent<Item> ().ID = (int)(Global.Items[i].GetField ("ID").n);
         Item.GetComponent<Item> ().ContainerID = (int)(Global.Items[i].GetField ("ContainerID").n);
         Item.GetComponent<Item> ().InfoPrefab = ItemInfoPrefab;
      }
   }

   public string GetIcon (int Item)
   {
      string Name = "Unknown";

      switch (Item) {
         case 1:
            Name = "Pickaxe";
            break;
         case 2:
            Name = "Club";
            break;
         case 3:
            Name = "Bones shield";
            break;
      }

      return Name;
   }
}
