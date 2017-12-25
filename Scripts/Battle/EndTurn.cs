using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndTurn : MonoBehaviour {

   private void OnMouseUpAsButton()
   {
      if (Global.BattleLeader == Global.Login && (Global.BattleStep == "WaitAlly" || Global.BattleStep == "MoveAlly")) {
         JSONObject data = new JSONObject(JSONObject.Type.OBJECT);
         data = Global.PrepareData(data);

         Global.Socket.Emit("WaitAlly", data);
         Global.BattleStep = "MoveEnemy";
      }
   }
}
