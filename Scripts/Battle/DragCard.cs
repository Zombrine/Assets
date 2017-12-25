using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragCard : MonoBehaviour {

   private float StartX;
   private float StartY;
   private float MousePosX;
   private float MousePosY;

   private void Start()
   {
      StartX = gameObject.transform.localPosition.x;
      StartY = gameObject.transform.localPosition.y;
   }

   private void OnMouseDown()
   {
      if (gameObject.transform.parent.name == "Allies" && Global.BattleLeader == Global.Login && Global.BattleStep == "MoveAlly") {
         MousePosX = Input.mousePosition.x;
         MousePosY = Input.mousePosition.y;
         StartX = gameObject.transform.localPosition.x;
         StartY = gameObject.transform.localPosition.y;
         gameObject.transform.localPosition = new Vector3(StartX, StartY, 100f);
      }
   }

   private void OnMouseUp()
   {
      if (gameObject.transform.parent.name == "Allies" && Global.BattleLeader == Global.Login && Global.BattleStep == "MoveAlly") {
         if (StartY - (MousePosY - Input.mousePosition.y) < 250f && StartY - (MousePosY - Input.mousePosition.y) > 70f) {
            float TempX = StartX - (MousePosX - Input.mousePosition.x);
            if (TempX < -225f) ToBattle(1);
            else if (TempX >= -225f && TempX < -75f) ToBattle(2);
            else if (TempX >= -75f && TempX < 75f) ToBattle(3);
            else if (TempX >= 75f && TempX < 225f) ToBattle(4);
            else if (TempX >= 225f) ToBattle(5);
            else gameObject.transform.localPosition = new Vector3(StartX, StartY, 0f);
         }
         else gameObject.transform.localPosition = new Vector3(StartX, StartY, 0f);
      }
   }

   private void ToBattle (int Number)
   {
      GameObject TempField = GameObject.Find("Location/Hero/Main Camera/Canvas/Battle/AlliesField/" + Number);
      if (TempField.transform.childCount > 0) gameObject.transform.localPosition = new Vector3(StartX, StartY, 0f);
      else {
         gameObject.transform.parent = TempField.transform;
         gameObject.transform.localPosition = new Vector3(0f, 0f, 0f);
         Global.BattleStep = "MoveEnemy";

         JSONObject data = new JSONObject(JSONObject.Type.OBJECT);
         data = Global.PrepareData(data);
         data.AddField("User", gameObject.name);
         data.AddField("Field", Number);

         Global.Socket.Emit ("MoveAlly", data);
         Global.BattleStep = "WaitAlly";
      }
   }

   private void OnMouseDrag()
   {
      if (gameObject.transform.parent.name == "Allies" && Global.BattleLeader == Global.Login && Global.BattleStep == "MoveAlly") {
         gameObject.transform.localPosition = new Vector3(StartX - (MousePosX - Input.mousePosition.x), StartY - (MousePosY - Input.mousePosition.y), 100f);
      }
   }

   private void OnMouseUpAsButton()
   {
      GameObject Fight = GameObject.Find("Location/Hero/Main Camera/Canvas/Battle/Fight");

      if (Global.BattleStep == "Fight" && Global.HeroCard != null && !Fight.active) {
         if (GameObject.Find("Location/Hero/Main Camera/Canvas/Battle/Allies/" + Global.Login) == null) {
            Fight.GetComponent<Fight>().Target = gameObject;
            Fight.GetComponent<Fight>().StartFight();
         }
      }
   }
}
