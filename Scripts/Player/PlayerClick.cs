using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerClick : MonoBehaviour {

   public bool IsEnemy;
   public GameObject ActionsPrefab;
   public GameObject ActionPrefab;

   void OnMouseUpAsButton ()
   {
      if (IsEnemy) (GameObject.Find ("Location/Hero/Main Camera/Canvas/Hero")).GetComponent<MainScript> ().SendStartBattle (gameObject);
      else {
         /*foreach (GameObject Sector in GameObject.FindGameObjectsWithTag ("Sector")) {
            if (Sector.transform.localPosition.x == gameObject.transform.localPosition.x && Sector.transform.localPosition.y == gameObject.transform.localPosition.y) {
               Sector.GetComponent<Sector> ().OnMouseUpAsButton ();
               break;
            }
         }*/
         GameObject AllInfo = GameObject.Find ("Location/Hero/Main Camera/Canvas/AllInfo");

         GameObject Actions = Instantiate (ActionsPrefab, new Vector3 (0, 0, 0), Quaternion.identity);
         Actions.transform.parent = AllInfo.transform;
         Actions.transform.localPosition = new Vector3 (0, 0, 0);
         Actions.transform.localScale = new Vector3 (1, 1, 1);
         int QuantityActions = 0;
         GameObject Action;

         GameObject TempHero = GameObject.Find ("Location/Hero");
         int TempX = (int)(TempHero.transform.localPosition.x);
         int TempY = (int)(TempHero.transform.localPosition.y);

         if ((gameObject.transform.localPosition.x == TempX + 2 || gameObject.transform.localPosition.x == TempX - 2 || gameObject.transform.localPosition.x == TempX) &&
            (gameObject.transform.localPosition.y == TempY + 2 || gameObject.transform.localPosition.y == TempY - 2 || gameObject.transform.localPosition.y == TempY) &&
            !(gameObject.transform.localPosition.y != TempY && gameObject.transform.localPosition.x != TempX)) {

            Action = Instantiate (ActionPrefab, new Vector3 (0, 0, 0), Quaternion.identity);
            Action.transform.parent = Actions.transform;
            Action.transform.localPosition = new Vector3 (0, 128 - (QuantityActions * 51), 0);
            Action.transform.localScale = new Vector3 (1, 1, 1);
            Action.transform.Find ("Text").GetComponent<Text> ().text = "Перейти";
            Action.GetComponent<PlayerAction> ().Type = "StepTo";
            Action.GetComponent<PlayerAction> ().Player = gameObject;

            QuantityActions++;
         }

         Action = Instantiate (ActionPrefab, new Vector3 (0, 0, 0), Quaternion.identity);
         Action.transform.parent = Actions.transform;
         Action.transform.localPosition = new Vector3 (0, 128 - (QuantityActions * 51), 0);
         Action.transform.localScale = new Vector3 (1, 1, 1);
         Action.transform.Find ("Text").GetComponent<Text> ().text = "Отмена";
         Action.GetComponent<PlayerAction> ().Type = "Cancel";
         Action.GetComponent<PlayerAction> ().Player = gameObject;
      }
   }
}
