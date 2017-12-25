using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fight : MonoBehaviour {

	public GameObject Target;
   private int HardLevel;   
   private float StaticWaiting;
   private float QuantityHits;
   private float CurrentHit;
   public float SuccessHits;
   private float PHeight;
   private float PWidth;
   private float Waiting;
   private bool IsFight;
   public GameObject HitPrefab;
   private GameObject Hit;

   public void StartFight ()
   {
      switch (Target.transform.Find("Fone/Character").GetComponent<Image>().sprite.name) {
         case "Skeleton":
            HardLevel = 1;
            break;
         case "Warrior":
            HardLevel = 2;
            break;
         default:
            HardLevel = 1;
            break;
      }

      QuantityHits = HardLevel*10;
      CurrentHit = 1;
      SuccessHits = 0;
      PHeight = gameObject.GetComponent<RectTransform> ().rect.height;
      PWidth = gameObject.GetComponent<RectTransform> ().rect.width;
      StaticWaiting = 2f;
      Waiting = 0f;
      IsFight = true;
      gameObject.active = true;

      float TempX = Random.Range(30f, PWidth - 30f);
      if (TempX > ((PWidth - 60f) / 2)) TempX = (TempX - ((PWidth - 60f) / 2)) * -1;
      float TempY = Random.Range(30f, PHeight - 30f);
      if (TempY > ((PHeight - 60f) / 2)) TempY = (TempY - ((PHeight - 60f) / 2)) * -1;
      Hit = Instantiate(HitPrefab, new Vector3(TempX, TempY, -10), Quaternion.identity);
      Hit.transform.parent = (GameObject.Find("Location/Hero/Main Camera/Canvas/Battle/Fight")).transform;
      Hit.transform.localPosition = new Vector3(TempX, TempY, -10);
      Hit.transform.localScale = new Vector3(1, 1, 1);
   }

   private void Update()
   {
      if (IsFight) {
         Waiting += Time.deltaTime;
         if (Waiting > StaticWaiting) {
            NextHit ();
         }
      }
   }

   public void NextHit ()
   {
      Destroy (Hit);
      if (CurrentHit < QuantityHits) {
         float TempX = Random.Range(30f, PWidth - 30f);
         if (TempX > ((PWidth - 60f) / 2)) TempX = (TempX - ((PWidth - 60f) / 2)) * -1;
         float TempY = Random.Range(30f, PHeight - 30f);
         if (TempY > ((PHeight - 60f) / 2)) TempY = (TempY - ((PHeight - 60f) / 2)) * -1;
         Hit = Instantiate(HitPrefab, new Vector3(TempX, TempY, -10), Quaternion.identity);
         Hit.transform.parent = (GameObject.Find("Location/Hero/Main Camera/Canvas/Battle/Fight")).transform;
         Hit.transform.localPosition = new Vector3(TempX, TempY, -10);
         Hit.transform.localScale = new Vector3(1, 1, 1);
         Waiting = 0f;
         CurrentHit++;
      }
      else {
         gameObject.active = false;
         Global.BattleStep = "Counting";
         int DMG = int.Parse (Global.HeroCard.transform.Find("DMG/Text").GetComponent<Text>().text);
         int SuccessDMG = 0;
         if (SuccessHits / QuantityHits == 0) SuccessDMG = 0;
         else SuccessDMG = Mathf.RoundToInt(DMG * (SuccessHits / QuantityHits));
         Target.transform.Find("HP/Text").GetComponent<Text>().text = (int.Parse (Target.transform.Find("HP/Text").GetComponent<Text>().text) - SuccessDMG).ToString();

         JSONObject data = new JSONObject(JSONObject.Type.OBJECT);
         data = Global.PrepareData(data);
         data.AddField("Target", Target.name);
         data.AddField("DMG", SuccessDMG);

         Global.Socket.Emit("Fight", data);
      }
   }
}
