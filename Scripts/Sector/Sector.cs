using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sector : MonoBehaviour {

   public string Movable;
   public GameObject Hero;
   private bool IsRightClicked = false;

  /* private void OnMouseOver ()
   {
      if (Input.GetMouseButton (1)) {
         IsRightClicked = true;
      }
      else {
         if (IsRightClicked) {
            IsRightClicked = false;
            Hero.GetComponent<MainScript> ().Redactor (gameObject);
         }
      }
   }*/

   public void OnMouseUpAsButton()
   {
      if ((Hero.transform.localPosition.x == gameObject.transform.localPosition.x - 2 || Hero.transform.localPosition.x == gameObject.transform.localPosition.x + 2 || Hero.transform.localPosition.x == gameObject.transform.localPosition.x) &&
          (Hero.transform.localPosition.y == gameObject.transform.localPosition.y - 2 || Hero.transform.localPosition.y == gameObject.transform.localPosition.y + 2 || Hero.transform.localPosition.y == gameObject.transform.localPosition.y) &&
          !(Hero.transform.localPosition.x != gameObject.transform.localPosition.x && Hero.transform.localPosition.y != gameObject.transform.localPosition.y)) {
         JSONObject data = new JSONObject(JSONObject.Type.OBJECT);
         switch (Movable) {
            case "Empty":
               data.AddField ("x", (49 + gameObject.transform.localPosition.x) / 2);
               data.AddField ("y", (49 - gameObject.transform.localPosition.y) / 2);
               data = Global.PrepareData (data);
               Global.Socket.Emit ("StepTo", data);
               break;
            case "Pickaxe":
               for (int i = 0; i < Global.QuantityItems; i++) {
                  switch ((int)Global.Items[i]["ID"].n) {
                     case 1:
                        data.AddField ("x", (49 + gameObject.transform.localPosition.x) / 2);
                        data.AddField ("y", (49 - gameObject.transform.localPosition.y) / 2);
                        data = Global.PrepareData(data);
                        Global.Socket.Emit("StepTo", data);
                        return;
                        break;
                     default:
                        break;
                  }
               }
               break;
            case "Exit":
               GameObject.Find ("Location/Hero/Main Camera/Canvas/Hero").GetComponent<MainScript> ().QueryToExit (gameObject);
               break;
            case "Enter":
               GameObject.Find ("Location/Hero/Main Camera/Canvas/Hero").GetComponent<MainScript> ().QueryToLeave ();
               break;
            default:
               break;
         }
      }
   }
}
