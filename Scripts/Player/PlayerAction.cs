using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAction : MonoBehaviour {

   public GameObject Player;
	public string Type;

   private void OnMouseUpAsButton ()
   {
      string ToLevel;
      JSONObject data;

      switch (Type) {

         case "StepTo":
            foreach (GameObject Sector in GameObject.FindGameObjectsWithTag ("Sector")) {
               if (Sector.transform.localPosition.x == Player.transform.localPosition.x && Sector.transform.localPosition.y == Player.transform.localPosition.y) {
                  Sector.GetComponent<Sector> ().OnMouseUpAsButton ();
                  break;
               }
            }
            Destroy (gameObject.transform.parent.gameObject);
            break;

         case "NewDungeon":
            data = new JSONObject (JSONObject.Type.OBJECT);
            data = Global.PrepareData (data);

            ToLevel = "DarkDungeon";
            switch (Player.name) {
               case "Exit":
                  switch (Global.LocationType) {
                     case "DarkDungeon":
                        if (Global.Level > 2) ToLevel = "DarkDungeon"; // СДЕЛАЙТЕ УЖЕ НОВУЮ ЛОКАЦИЮ
                        else                  ToLevel = "DarkDungeon";
                        break;
                  }
                  break;
               case "DarkDungeon":
                  if (Global.Level > 2) ToLevel = "DarkDungeon"; // СДЕЛАЙТЕ УЖЕ НОВУЮ ЛОКАЦИЮ
                  else                  ToLevel = "DarkDungeon";
                  break;
            }

            (GameObject.Find ("Location/Hero/Main Camera/Canvas/Loading")).active = true;
            data.AddField ("tolevel", ToLevel);
            Global.Socket.Emit ("NewDungeon", data);
            Destroy (gameObject.transform.parent.gameObject);
            break;

         case "OldDungeon":
            data = new JSONObject (JSONObject.Type.OBJECT);
            data = Global.PrepareData (data);

            ToLevel = "DarkDungeon";
            switch (Global.LocationType) {
               case "DarkDungeon":
                  if (Global.Level > 2) ToLevel = "DarkDungeon"; // СДЕЛАЙТЕ УЖЕ НОВУЮ ЛОКАЦИЮ
                  else                  ToLevel = "DarkDungeon";
                  break;
            }
            data.AddField ("tolevel", ToLevel);
            Global.Socket.Emit ("OldDungeon", data);
            Destroy (gameObject.transform.parent.gameObject);
            break;

         case "Unite":
            data = new JSONObject (JSONObject.Type.OBJECT);
            data = Global.PrepareData (data);
            data.AddField ("Player", gameObject.transform.Find("Text").GetComponent<Text> ().text);

            (GameObject.Find ("Location/Hero/Main Camera/Canvas/Loading")).active = true;
            Global.Socket.Emit ("Unite", data);
            Destroy (gameObject.transform.parent.gameObject);
            break;

         case "Leave":
            data = new JSONObject (JSONObject.Type.OBJECT);
            data = Global.PrepareData (data);
            data.AddField ("Location", Global.LocationType);

            (GameObject.Find ("Location/Hero/Main Camera/Canvas/Loading")).active = true;
            Global.Socket.Emit ("Leave", data);
            Destroy (gameObject.transform.parent.gameObject);
            break;

         case "Grass": // Build - событие, которое генерирует новый тип клетки. (Для механизма генерации уровня вручную)
            data = new JSONObject (JSONObject.Type.OBJECT);
            data = Global.PrepareData (data);
            data.AddField ("x", (49 + Player.transform.localPosition.x) / 2);
            data.AddField ("y", (49 - Player.transform.localPosition.y) / 2);
            data.AddField ("Type", Type);

            Global.Socket.Emit ("Build", data);
            Destroy (gameObject.transform.parent.gameObject);
            break;

         case "TownWall":
            data = new JSONObject (JSONObject.Type.OBJECT);
            data = Global.PrepareData (data);
            data.AddField ("x", (49 + Player.transform.localPosition.x) / 2);
            data.AddField ("y", (49 - Player.transform.localPosition.y) / 2);
            data.AddField ("Type", Type);

            Global.Socket.Emit ("Build", data);
            Destroy (gameObject.transform.parent.gameObject);
            break;

         case "Window":
            data = new JSONObject (JSONObject.Type.OBJECT);
            data = Global.PrepareData (data);
            data.AddField ("x", (49 + Player.transform.localPosition.x) / 2);
            data.AddField ("y", (49 - Player.transform.localPosition.y) / 2);
            data.AddField ("Type", Type);

            Global.Socket.Emit ("Build", data);
            Destroy (gameObject.transform.parent.gameObject);
            break;

         case "TownFloor":
            data = new JSONObject (JSONObject.Type.OBJECT);
            data = Global.PrepareData (data);
            data.AddField ("x", (49 + Player.transform.localPosition.x) / 2);
            data.AddField ("y", (49 - Player.transform.localPosition.y) / 2);
            data.AddField ("Type", Type);

            Global.Socket.Emit ("Build", data);
            Destroy (gameObject.transform.parent.gameObject);
            break;

         case "Trail":
            data = new JSONObject (JSONObject.Type.OBJECT);
            data = Global.PrepareData (data);
            data.AddField ("x", (49 + Player.transform.localPosition.x) / 2);
            data.AddField ("y", (49 - Player.transform.localPosition.y) / 2);
            data.AddField ("Type", Type);

            Global.Socket.Emit ("Build", data);
            Destroy (gameObject.transform.parent.gameObject);
            break;

         case "Mountain":
            data = new JSONObject (JSONObject.Type.OBJECT);
            data = Global.PrepareData (data);
            data.AddField ("x", (49 + Player.transform.localPosition.x) / 2);
            data.AddField ("y", (49 - Player.transform.localPosition.y) / 2);
            data.AddField ("Type", Type);

            Global.Socket.Emit ("Build", data);
            Destroy (gameObject.transform.parent.gameObject);
            break;

         case "Tree":
            data = new JSONObject (JSONObject.Type.OBJECT);
            data = Global.PrepareData (data);
            data.AddField ("x", (49 + Player.transform.localPosition.x) / 2);
            data.AddField ("y", (49 - Player.transform.localPosition.y) / 2);
            data.AddField ("Type", Type);

            Global.Socket.Emit ("Build", data);
            Destroy (gameObject.transform.parent.gameObject);
            break;

         case "Cancel":
            Destroy (gameObject.transform.parent.gameObject);
            break;
      }
   }
}
