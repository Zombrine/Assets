using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Notify : MonoBehaviour {

   public GameObject NotifyText;
   private float Waiting;
   private int Vision;
   private int ForShowing; // 0 - остановились на убранном; 1 - показываем; 2 - скрываем
   private bool FlagCloseBattle;

   public void Show (string ShowText, bool IsNeedCloseBattle = false)
   {
      NotifyText.GetComponent<Text>().text = ShowText;
      Waiting = 0f;
      ForShowing = 1;
      if (IsNeedCloseBattle) FlagCloseBattle = true;
   }

   private void Update()
   {
      if (ForShowing == 1 || ForShowing == 2) {
         Waiting += Time.deltaTime;
         for (; Waiting > 0.008f; Waiting -= 0.008f) {
            if (ForShowing == 1) {
               Vision++;
               if (Vision >= 127) {
                  ForShowing = 2;
                  Waiting = -1f;
               }
            }
            else {
               Vision--;
               if (Vision <= 0) {
                  ForShowing = 0;
                  gameObject.GetComponent<Image>().color = new Color32(0x7E, 0x7E, 0x7E, 0x00);
                  NotifyText.GetComponent<Text>().color = new Color32(0x32, 0x32, 0x32, 0x00);
                  if (FlagCloseBattle) {
                     FlagCloseBattle = false;
                     GameObject.Find("Location/Hero/Main Camera/Canvas/Hero").GetComponent<MainScript>().CloseBattle ();
                  }
                  return;
               }
            }
            
            gameObject.GetComponent<Image>().color = new Color32 (0x7E, 0x7E, 0x7E, (byte)(Vision));
            NotifyText.GetComponent<Text>().color = new Color32 (0x32, 0x32, 0x32, (byte)(Vision*2));
         }
      }
   }
}
