using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using SocketIO;

public static class Global // важные данные, доступные во всей области проекта. Обычно, здесь хранятся данные пользователя.
{
   /*
     GameObject Temp = Instantiate (ItemPrefab, new Vector3 (pX, -148f, -1f), Quaternion.identity);
     Temp.transform.parent = Hero.transform;
     Temp.transform.localPosition = new Vector3(pX, -148f, -1f);
     Temp.transform.localScale = new Vector3(1, 1, 1);
   */
   public static SocketIOComponent Socket;
   public static string Login;
   public static string Token;
   public static string LocationType;
   public static int HP;
   public static int DMG;
   public static int Level;
   public static int XP;
   public static int XPForLevel;
   public static JSONObject[] Items;
   public static int QuantityItems;
   public static int Money;
   public static string Battle;
   public static string BattleLeader;
   public static string BattleStep;
   public static GameObject HeroCard;

   public static JSONObject PrepareData(JSONObject data)
   {
      data.AddField("login", Login);
      data.AddField("token", Token);
      if (Battle != "") data.AddField("battle", Battle);
      return data;
   }
}

public class MainScript : MonoBehaviour {

   public GameObject Login;
   public GameObject Password;
   public GameObject SectorPrefab;
   public GameObject PlayerPrefab;
   public GameObject CardPrefab;
   public GameObject MainMenu;
   public GameObject Loading;
   public GameObject SavePanelForBattle;
   public GameObject Battle;
   public GameObject Notify;
   public GameObject InfoPrefab;
   public GameObject AllInfo;
   public GameObject ImagePrefab;
   public GameObject TextPrefab;
   public GameObject Inventory;
   public GameObject ActionsPrefab;
   public GameObject ActionPrefab;
   public GameObject Hero;

   void Start()
   {
      GameObject go = GameObject.Find("SocketIO"); // Устанавливаем соединение с сервером и подготавливаем обработчики событий
      Global.Socket = go.GetComponent<SocketIOComponent>();

      Global.Items = new JSONObject[100];

      Global.Socket.On ("Entered", GetData);
      Global.Socket.On ("NotEntered", WrongData);
      Global.Socket.On ("GetLocation", StructLocation);
      Global.Socket.On ("StepTo", StepTo);
      Global.Socket.On ("GetInventory", GetInventory);
      Global.Socket.On ("Crash", Crash);
      Global.Socket.On ("SetSector", SetSector);
      Global.Socket.On ("StartBattle", StartBattle);
      Global.Socket.On ("MoveAlly", MoveAlly);
      Global.Socket.On ("MoveEnemy", MoveEnemy);
      Global.Socket.On ("Fight", Fight);
      Global.Socket.On ("FightEnemy", FightEnemy);
      Global.Socket.On ("Die", Die);
      Global.Socket.On ("NextStep", NextStep);
      Global.Socket.On ("EndFight", EndFight);
      Global.Socket.On ("Offline", Offline);
      Global.Socket.On ("Restore", Restore);
      Global.Socket.On ("GrandXP", GrandXP);
      Global.Socket.On ("GrandItem", GrandItem);
      Global.Socket.On ("OldDungeon", OldDungeon);
   }

   public void LogOn()
   {
      MainMenu.active = false;
      Loading.active = true;
      JSONObject data = new JSONObject(JSONObject.Type.OBJECT);
      data.AddField("login", Login.GetComponent<Text>().text);
      data.AddField("password", Password.GetComponent<Text>().text);
      data.AddField("version", 1);
      Global.Socket.Emit("user:login", data);
   }

   void GetData(SocketIOEvent ev) // Вызывается после того, как сервер сообщил об успешном логине
   {
      Global.Login = Login.GetComponent<Text>().text;
      Global.Token = ev.data["token"].str;
      Global.HP = (int)(ev.data["HP"].n);
      Global.DMG = (int)(ev.data["DMG"].n);
      Global.XP = (int)(ev.data["XP"].n);
      Global.XPForLevel = (int)(ev.data["XPForLevel"].n);
      Global.Level = (int)(ev.data["Level"].n);

      JSONObject data = new JSONObject(JSONObject.Type.OBJECT);
      data = Global.PrepareData(data);
      Global.Socket.Emit("GetLocation", data);
      Global.Socket.Emit("GetInventory", data);
   }

   void StructLocation(SocketIOEvent ev) // Построение локации. Вызывается сервером после получение запроса GetLocation
   {
      foreach (GameObject Sector in GameObject.FindGameObjectsWithTag ("Sector")) {
         Destroy (Sector);
      }
      foreach (GameObject Player in GameObject.FindGameObjectsWithTag ("Player")) {
         Destroy (Player);
      }

      Global.LocationType = ev.data["Location"].str;
      
      float Size = ev.data["Size"].n;
      Size = Mathf.Sqrt(Size);
      for (int j = 0; j < Size; j++) {
         for (int i = 0; i < Size; i++) {
            GameObject Temp = Instantiate(SectorPrefab, new Vector3(-49 + (i * 2), 49 - (j * 2), 0f), Quaternion.identity);
            string SectorName = ev.data["x" + i + "y" + j].str;
            Temp.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/" + SectorName);
            Temp.transform.parent = (GameObject.Find("Location/Canvas/Sectors")).transform;
            Temp.transform.localPosition = new Vector3(-49 + (i * 2), 49 - (j * 2), 0f);
            Temp.transform.localScale = new Vector3(1, 1, 1);
            Temp.GetComponent<Sector>().Hero = Hero;
            switch (SectorName) {
               case "StoneFloor":
               case "Grass":
               case "TownFloor":
               case "Trail":
                  Temp.GetComponent<Sector>().Movable = "Empty";
                  break;
               case "Stone":
                  Temp.GetComponent<Sector>().Movable = "Pickaxe";
                  break;
               case "Exit":
               case "DarkDungeon":
                  Temp.GetComponent<Sector> ().Movable = "Exit";
                  break;
               case "Enter":
                  Temp.GetComponent<Sector> ().Movable = "Enter";
                  break;
            }
         }
      }

      float EnemySize = ev.data["EnemySize"].n;
      string Enemy = "";
      string EnemyID = "";
      int x = 0;
      int y = 0;
      for (int i = 0; i < EnemySize; i++) {
         Enemy = ev.data["Enemy" + i].str; // x1y12TypeSkeletonID1
         Regex EnemyReg = new Regex (@"x(.*)y(.*)Type(.*)ID(.*)");
         MatchCollection Matches = EnemyReg.Matches (Enemy);
         if (Matches.Count > 0) {
            foreach (Match mat in Matches) {
               GroupCollection groups = mat.Groups;
               x = int.Parse (groups[1].Value);
               y = int.Parse(groups[2].Value);
               Enemy = groups[3].Value;
               EnemyID = groups[4].Value;
            }
            GameObject TempPlayer = Instantiate (PlayerPrefab, new Vector3(-49 + (x * 2), 49 - (y * 2), 0), Quaternion.identity);
            TempPlayer.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/" + Enemy);
            TempPlayer.GetComponent<PlayerClick> ().IsEnemy = true;
            TempPlayer.transform.parent = (GameObject.Find("Location/Canvas/Players")).transform;
            TempPlayer.transform.localPosition = new Vector3 (-49 + (x * 2), 49 - (y * 2), 0);
            TempPlayer.transform.localScale = new Vector3 (1, 1, 1);
            TempPlayer.name = EnemyID;
         }
      }
      Loading.active = false;
   }

   void StepTo(SocketIOEvent ev) // Переход персонажа на определенную клетку
   {
      if (ev.data["login"].str == Global.Login) {
         Hero.transform.localPosition = new Vector3 (-49 + (ev.data["x"].n * 2), 49 - (ev.data["y"].n * 2), -32);
      }
      else {
         GameObject TempPlayer = GameObject.Find("Location/Canvas/Players/" + ev.data["login"].str);
         if (TempPlayer == null) {
            TempPlayer = Instantiate(PlayerPrefab, new Vector3(-49 + (ev.data["x"].n * 2), 49 - (ev.data["y"].n * 2), 0), Quaternion.identity);
            TempPlayer.GetComponent<PlayerClick> ().IsEnemy = false;
            TempPlayer.GetComponent<PlayerClick> ().ActionsPrefab = ActionsPrefab;
            TempPlayer.GetComponent<PlayerClick> ().ActionPrefab = ActionPrefab;
            TempPlayer.transform.parent = (GameObject.Find("Location/Canvas/Players")).transform;
            TempPlayer.transform.localPosition = new Vector3(-49 + (ev.data["x"].n * 2), 49 - (ev.data["y"].n * 2), 0);
            TempPlayer.transform.localScale = new Vector3(1, 1, 1);
            TempPlayer.name = ev.data["login"].str;
         }
         TempPlayer.transform.localPosition = new Vector3(-49 + (ev.data["x"].n * 2), 49 - (ev.data["y"].n * 2), 0);
      }
   }

   void GetInventory (SocketIOEvent ev) // Получение инвентаря. Вызывается после того, как сервер получит запрос GetInventory
   {
      Global.Money = (int)(ev.data["Money"].n);
      Global.QuantityItems = (int)(ev.data["Size"].n);
      for (int i = 0; i < Global.QuantityItems; i++) {
         JSONObject TempData = new JSONObject(JSONObject.Type.OBJECT);
         Regex ItemsReg = new Regex(@"ID(.*)Quantity(.*)ContainerID(.*)");
         MatchCollection Matches = ItemsReg.Matches(ev.data[i].str);
         if (Matches.Count > 0) {
            foreach (Match mat in Matches) {
               GroupCollection groups = mat.Groups;
               TempData.AddField ("ID", int.Parse(groups[1].Value));
               TempData.AddField ("Quantity", int.Parse(groups[2].Value));
               TempData.AddField ("ContainerID", int.Parse(groups[3].Value));
            }
         }
         Global.Items[i] = TempData;
      }
   }

   void Crash (SocketIOEvent ev) // Разрушение объектов. Например, разрушение каменной породы киркой (а-ля майнкрафт)
   {
      int TempX = (int)(-49 + (ev.data["x"].n * 2));
      int TempY = (int)(49 - (ev.data["y"].n * 2));
      if ((Hero.transform.localPosition.x == TempX + 2 || Hero.transform.localPosition.x == TempX - 2 || Hero.transform.localPosition.x == TempX) &&
          (Hero.transform.localPosition.y == TempY + 2 || Hero.transform.localPosition.y == TempY - 2 || Hero.transform.localPosition.y == TempY) &&
          !(Hero.transform.localPosition.y != TempY && Hero.transform.localPosition.x != TempX)) {
         foreach (GameObject Sector in GameObject.FindGameObjectsWithTag("Sector")) {
            if (Sector.transform.localPosition.x == TempX && Sector.transform.localPosition.y == TempY) {
               /*Sector.GetComponent<Sector>().Movable = "Empty"; // Это можно изменить на анимацию (Без реструкта)
               string TempSectorName = "StoneFloor";
               Sector.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/" + TempSectorName);*/
               JSONObject data = new JSONObject(JSONObject.Type.OBJECT);
               data = Global.PrepareData(data);
               data.AddField ("x", ev.data["x"].n);
               data.AddField ("y", ev.data["y"].n);
               data.AddField ("Type", "StoneFloor");
               Global.Socket.Emit ("SetSector", data);
               break;
            }
         }
      }
   }

   void SetSector (SocketIOEvent ev) // Изменение типа клетки. Например, изменение каменной породы на почву
   {
      int TempX = (int)(-49 + (ev.data["x"].n * 2));
      int TempY = (int)(49 - (ev.data["y"].n * 2));
      
      foreach (GameObject Sector in GameObject.FindGameObjectsWithTag("Sector")) {
         if (Sector.transform.localPosition.x == TempX && Sector.transform.localPosition.y == TempY) {
            Sector.GetComponent<Sector>().Movable = "Empty";
            string TempSectorName = ev.data["type"].str;
            Sector.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/" + TempSectorName);
            break;
         }
      }
   }

   public void SendStartBattle (GameObject Enemy) // Подготовка к битве. Все игроки, которые находятся на клетке инициатора боя и на клетке защищающихся, будут перенесены в битву.
   {
      if (Enemy != Hero) {
         JSONObject data = new JSONObject(JSONObject.Type.OBJECT);
         data = Global.PrepareData(data);
         data.AddField("x", (49 + Hero.transform.localPosition.x) / 2);
         data.AddField("y", (49 - Hero.transform.localPosition.y) / 2);
         data.AddField("xenemy", (49 + Enemy.transform.localPosition.x) / 2);
         data.AddField("yenemy", (49 - Enemy.transform.localPosition.y) / 2);
         SavePanelForBattle.active = true;
         Global.Socket.Emit("StartBattle", data);
      }
   }

   void StartBattle (SocketIOEvent ev) // Раскидываем карты по полю боя (на данный момент можно только по 5 игроков сделать с одной и другой стороны) и устанавливаем характеристики
   {
      Battle.active = true;
      int Allies = (int)(ev.data["Allies"].n);
      for (int i = 0; i < Allies; i++) {
         string TempLogin = ev.data["Ally" + i].str;
         string TempData = ev.data["User" + TempLogin].str;
         int HP = 0;
         int DMG = 0;
         Regex StatReg = new Regex(@"HP(.*)DMG(.*)");
         MatchCollection Matches = StatReg.Matches (TempData);
         if (Matches.Count > 0) {
            foreach (Match mat in Matches) {
               GroupCollection groups = mat.Groups;
               HP = int.Parse (groups[1].Value);
               DMG = int.Parse (groups[2].Value);
            }
         }

         int TempX = 0;
         if (Allies%2 == 0) TempX = (-80 * ((Allies - 1))) + (160 * i);
         else TempX = (-160*((Allies-1)/2)) + (160*i);

         GameObject Card = Instantiate (CardPrefab, new Vector3(TempX, 0, 0), Quaternion.identity);
         Card.transform.parent = (GameObject.Find("Location/Hero/Main Camera/Canvas/Battle/Allies")).transform;
         Card.transform.localPosition = new Vector3(TempX, 0, 0);
         Card.transform.localScale = new Vector3(1, 1, 1);
         Card.name = TempLogin;
         if (TempLogin == Global.Login) Global.HeroCard = Card;
         Card.transform.Find("DMG/Text").GetComponent<Text>().text = DMG.ToString();
         Card.transform.Find("HP/Text").GetComponent<Text>().text = HP.ToString();
         Card.transform.Find("Name").GetComponent<Text>().text = TempLogin;
      }

      int Enemies = (int)(ev.data["Enemies"].n);
      for (int i = 0; i < Enemies; i++) {
         int TempEnemyID = (int)(ev.data["Enemy" + i].n);
         string TempEnemyData = ev.data["EnemyID" + TempEnemyID].str;
         int HP = 0;
         int DMG = 0;
         string Type = "Skeleton";
         Regex StatReg = new Regex(@"Type(.*)HP(.*)DMG(.*)");
         MatchCollection Matches = StatReg.Matches(TempEnemyData);
         if (Matches.Count > 0) {
            foreach (Match mat in Matches) {
               GroupCollection groups = mat.Groups;
               Type = groups[1].Value;
               HP = int.Parse(groups[2].Value);
               DMG = int.Parse(groups[3].Value);
            }
         }

         int TempX = 0;
         if (Enemies % 2 == 0) TempX = (-80 * ((Enemies - 1))) + (160 * i);
         else TempX = (-160 * ((Enemies - 1) / 2)) + (160 * i);
         
         GameObject Card = Instantiate (CardPrefab, new Vector3(TempX, 0, 0), Quaternion.identity);
         Card.transform.parent = (GameObject.Find("Location/Hero/Main Camera/Canvas/Battle/Enemies")).transform;
         Card.transform.localPosition = new Vector3(TempX, 0, 0);
         Card.transform.localScale = new Vector3(1, 1, 1);
         Card.name = TempEnemyID.ToString();
         Card.transform.Find("Fone/Character").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/" + Type);
         Card.transform.Find("DMG/Text").GetComponent<Text>().text = DMG.ToString();
         Card.transform.Find("HP/Text").GetComponent<Text>().text = HP.ToString();
         Card.transform.Find("Name").GetComponent<Text>().text = Type;
      }

      for (int i = 1; i < 6; i++) {
         if (ev.data["AField" + i].str != "") {
            GameObject TempPlayer = GameObject.Find("Location/Hero/Main Camera/Canvas/Battle/Allies/" + ev.data["AField" + i].str);
            if (TempPlayer != null) {
               TempPlayer.transform.parent = (GameObject.Find("Location/Hero/Main Camera/Canvas/Battle/AlliesField")).transform;
            }
         }
      }

      for (int i = 1; i < 6; i++) {
         if (ev.data["EField" + i].str != "") {
            GameObject TempPlayer = GameObject.Find("Location/Hero/Main Camera/Canvas/Battle/Enemies/" + ev.data["EField" + i].str);
            if (TempPlayer != null) {
               TempPlayer.transform.parent = (GameObject.Find("Location/Hero/Main Camera/Canvas/Battle/EnemiesField")).transform;
            }
         }
      }

      Global.Battle = ev.data["Battle"].str;
      Global.BattleLeader = ev.data["Leader"].str;
      Global.BattleStep = ev.data["Step"].str;
      
      Notify.GetComponent<Notify>().Show ("Фаза хода");
   }

   void MoveAlly(SocketIOEvent ev) // Фаза хода: перенос карты союзника на определенную ячейку поля боя
   {
      if (Global.Battle != "") {
         GameObject User = GameObject.Find("Location/Hero/Main Camera/Canvas/Battle/Allies/" + ev.data["User"].str);
         if (User != null) {
            User.transform.parent = (GameObject.Find("Location/Hero/Main Camera/Canvas/Battle/AlliesField/" + (int)(ev.data["Field"].n))).transform;
            User.transform.localPosition = new Vector3(0f, 0f, 0f);
         }
         Global.BattleStep = "WaitAlly";
      }
   }

   void MoveEnemy(SocketIOEvent ev) // Фаза хода: противник переносит свою карту на поле боя (на данный момент сделано только PVE)
   {
      if (Global.Battle != "") {
         if (ev.data["Enemy"] != null) {
            GameObject Enemy = GameObject.Find("Location/Hero/Main Camera/Canvas/Battle/Enemies/" + ev.data["Enemy"].str);
            if (Enemy != null) {
               Enemy.transform.parent = (GameObject.Find("Location/Hero/Main Camera/Canvas/Battle/EnemiesField/" + (int)(ev.data["Field"].n))).transform;
               Enemy.transform.localPosition = new Vector3(0f, 0f, 0f);
            }
         }
         Global.BattleStep = "Fight";

         Notify.GetComponent<Notify>().Show("Фаза битвы");
      }
   }

   void Fight(SocketIOEvent ev) // Фаза битвы: союзник атаковал противника
   {
      GameObject Enemy = null;
      for (int i = 1; i < 6; i++) {
         Enemy = GameObject.Find("Location/Hero/Main Camera/Canvas/Battle/EnemiesField/" + i + "/" + ev.data["Target"].str);
         if (Enemy != null) break;
      }
      if (Enemy != null) {
         Enemy.transform.Find("HP/Text").GetComponent<Text>().text = (int.Parse(Enemy.transform.Find("HP/Text").GetComponent<Text>().text) - ev.data["DMG"].n).ToString();
      }
   }

   void FightEnemy(SocketIOEvent ev) // Фаза битвы: противник атаковал союзника
   {
      GameObject Ally = null;
      for (int i = 1; i < 6; i++) {
         Ally = GameObject.Find("Location/Hero/Main Camera/Canvas/Battle/AlliesField/" + i + "/" + ev.data["Target"].str);
         if (Ally != null) break;
      }
      if (Ally != null) {
         Ally.transform.Find("HP/Text").GetComponent<Text>().text = (int.Parse(Ally.transform.Find("HP/Text").GetComponent<Text>().text) - ev.data["DMG"].n).ToString();
      }
   }

   void Die(SocketIOEvent ev) // Фаза подсчета итогов: уничтожаем карты, у которых меньше или равно 0 хп
   {
      GameObject TempCard = null;
      if (ev.data["Side"].str == "Ally") {
         for (var i = 1; i < 6; i++) {
            TempCard = GameObject.Find("Location/Hero/Main Camera/Canvas/Battle/AlliesField/" + i + "/" + ev.data["Login"].str);
            if (TempCard != null) break;
         }
      }
      else {
         for (var i = 1; i < 6; i++) {
            TempCard = GameObject.Find("Location/Hero/Main Camera/Canvas/Battle/EnemiesField/" + i + "/" + ev.data["Login"].str);
            if (TempCard != null) break;
         }
      }

      if (TempCard != null) {
         Destroy(TempCard);
      }
   }

   void NextStep(SocketIOEvent ev) // если еще есть кому драться, то переходим снова к фазе хода
   {
      Global.BattleStep = ev.data["Step"].str;
      Notify.GetComponent<Notify>().Show("Фаза хода");
   }

   void EndFight(SocketIOEvent ev) // если все убиты или есть сторона с выжившими, то объявляется конец битвы и раздача дропа (в случае ничьей дроп никому не дается)
   {
      Global.Battle = "";
      Global.BattleLeader = "";
      Global.BattleStep = "";
      Notify.GetComponent<Notify>().Show("Конец битвы", true);
   }

   public void CloseBattle () // Закрываем битву.
   {
      Battle.active = false;
      SavePanelForBattle.active = false;
      foreach (GameObject Card in GameObject.FindGameObjectsWithTag("Card")) {
         Destroy (Card);
      }
   }

   void Offline(SocketIOEvent ev) // Вызывается, когда игрок уходит в оффлайн (или пропадает с текущей локации)
   {
      if (ev.data["Login"].str == Global.Login) {
         foreach (GameObject Sector in GameObject.FindGameObjectsWithTag("Sector")) {
            Destroy (Sector);
         }

         foreach (GameObject Player in GameObject.FindGameObjectsWithTag("Player")) {
            Destroy (Player);
         }
      }
      else {
         GameObject TempPlayer = GameObject.Find("Location/Canvas/Players/" + ev.data["Login"].str);
         if (TempPlayer != null) {
            Destroy (TempPlayer);
         }
      }
   }

   void Restore(SocketIOEvent ev) // Восстановка данных. Вызывается, когда необходимо переместить персонажа, например, на другую локацию. Вызывается сервером.
   {
      JSONObject data = new JSONObject(JSONObject.Type.OBJECT);
      data = Global.PrepareData(data);
      Global.Socket.Emit("GetLocation", data);
   }

   void GrandXP(SocketIOEvent ev) // Награждение опытом
   {
      Global.XP += (int)(ev.data["XP"].n);
      if (Global.XP >= Global.XPForLevel) {
         Global.XP = 0;
         switch (Global.Level) {
            case 1:
               Global.XPForLevel = 100;
               break;
            default:
               break;
         }
         Global.Level++;
         Global.HP++;
      }
   }

   void GrandItem (SocketIOEvent ev) // Награждение дропом. Дроп дается только тому, кто инициировал битву.
   {
      GameObject Info = Instantiate (InfoPrefab, new Vector3 (0, 0, 0), Quaternion.identity);
      Info.transform.parent = AllInfo.transform;
      Info.transform.localPosition = new Vector3 (0, 0, 0);
      Info.transform.localScale = new Vector3 (1, 1, 1);

      if (ev.data["Recipient"].str != Global.Login) Info.transform.Find ("HeaderText").GetComponent<Text> ().text = "Добыча достается: " + ev.data["Recipient"].str;
      else Info.transform.Find ("HeaderText").GetComponent<Text> ().text = "Ваша добыча";

      Transform Body = Info.transform.Find ("Body");

      if ((int)(ev.data["Quantity"].n) != 0) {
         int Quantity = (int)(ev.data["Quantity"].n);
         for (int i = 0; i < Quantity; i++) {
            int Item = (int)(ev.data[i].n);

            if (ev.data["Recipient"].str == Global.Login) {
               bool IsFounded = false;
               for (int j = 0; j < Global.QuantityItems; j++) {
                  if ((int)(Global.Items[j].GetField ("ID").n) == Item) {
                     Global.Items[j].AddField ("Quantity", (int)(Global.Items[j].GetField ("Quantity").n) + 1);
                     IsFounded = true;
                     break;
                  }
               }

               if (!IsFounded) {
                  Global.QuantityItems++;
                  JSONObject TempItem = new JSONObject (JSONObject.Type.OBJECT);
                  TempItem.AddField ("ID", Item);
                  TempItem.AddField ("Quantity", 1);
                  TempItem.AddField ("ContainerID", 1);
               }
            }

            // 598 272|9 4| 576 256
            GameObject TempImage = Instantiate (ImagePrefab, new Vector3 (0, 0, 0), Quaternion.identity);
            TempImage.transform.parent = Body;
            int iX = i%9;
            int iY = i/9;
            TempImage.transform.localPosition = new Vector3 ((iX*64) - 288, 128 - (iY*64), 0);
            TempImage.transform.localScale = new Vector3 (1, 1, 1);
            TempImage.GetComponent<Image> ().sprite = Resources.Load<Sprite> ("Sprites/" + (Inventory.GetComponent<Inventory> ().GetIcon (Item)));
         }
      }

      if ((int)(ev.data["Money"].n) != 0) {
         if (ev.data["Recipient"].str == Global.Login) Global.Money += (int)(ev.data["Money"].n);
         GameObject TempImage = Instantiate (ImagePrefab, new Vector3 (0, 0, 0), Quaternion.identity);
         TempImage.transform.parent = Body;
         int iX = (int)(ev.data["Quantity"].n) % 9;
         int iY = (int)(ev.data["Quantity"].n) / 9;
         TempImage.transform.localPosition = new Vector3 ((iX * 64) - 288 + 32, 128 - 32 - (iY * 64), 0);
         TempImage.transform.localScale = new Vector3 (1, 1, 1);
         TempImage.GetComponent<Image> ().sprite = Resources.Load<Sprite> ("Sprites/Money");
         TempImage.transform.Find ("Text").GetComponent<Text> ().text = ((int)(ev.data["Money"].n)).ToString ();
      }
   }

   public void QueryToExit (GameObject Sector)
   {
      GameObject Actions = Instantiate (ActionsPrefab, new Vector3 (0, 0, 0), Quaternion.identity);
      Actions.transform.parent = AllInfo.transform;
      Actions.transform.localPosition = new Vector3 (0, 0, 0);
      Actions.transform.localScale = new Vector3 (1, 1, 1);
      int QuantityActions = 0;
      GameObject Action;

      Action = Instantiate (ActionPrefab, new Vector3 (0, 0, 0), Quaternion.identity);
      Action.transform.parent = Actions.transform;
      Action.transform.localPosition = new Vector3 (0, 128 - (QuantityActions * 51), 0);
      Action.transform.localScale = new Vector3 (1, 1, 1);
      Action.transform.Find ("Text").GetComponent<Text> ().text = "В новое подземелье";
      Action.GetComponent<PlayerAction> ().Type = "NewDungeon";
      Action.GetComponent<PlayerAction> ().Player = Sector;
      QuantityActions++;

      Action = Instantiate (ActionPrefab, new Vector3 (0, 0, 0), Quaternion.identity);
      Action.transform.parent = Actions.transform;
      Action.transform.localPosition = new Vector3 (0, 128 - (QuantityActions * 51), 0);
      Action.transform.localScale = new Vector3 (1, 1, 1);
      Action.transform.Find ("Text").GetComponent<Text> ().text = "Объединиться с ...";
      Action.GetComponent<PlayerAction> ().Type = "OldDungeon";
      Action.GetComponent<PlayerAction> ().Player = Sector;
      QuantityActions++;

      Action = Instantiate (ActionPrefab, new Vector3 (0, 0, 0), Quaternion.identity);
      Action.transform.parent = Actions.transform;
      Action.transform.localPosition = new Vector3 (0, 128 - (QuantityActions * 51), 0);
      Action.transform.localScale = new Vector3 (1, 1, 1);
      Action.transform.Find ("Text").GetComponent<Text> ().text = "Отмена";
      Action.GetComponent<PlayerAction> ().Type = "Cancel";
      Action.GetComponent<PlayerAction> ().Player = Hero;
   }

   public void QueryToLeave ()
   {
      GameObject Actions = Instantiate (ActionsPrefab, new Vector3 (0, 0, 0), Quaternion.identity);
      Actions.transform.parent = AllInfo.transform;
      Actions.transform.localPosition = new Vector3 (0, 0, 0);
      Actions.transform.localScale = new Vector3 (1, 1, 1);
      int QuantityActions = 0;
      GameObject Action;

      Action = Instantiate (ActionPrefab, new Vector3 (0, 0, 0), Quaternion.identity);
      Action.transform.parent = Actions.transform;
      Action.transform.localPosition = new Vector3 (0, 128 - (QuantityActions * 51), 0);
      Action.transform.localScale = new Vector3 (1, 1, 1);
      Action.transform.Find ("Text").GetComponent<Text> ().text = "Выйти";
      Action.GetComponent<PlayerAction> ().Type = "Leave";
      Action.GetComponent<PlayerAction> ().Player = Hero;
      QuantityActions++;

      Action = Instantiate (ActionPrefab, new Vector3 (0, 0, 0), Quaternion.identity);
      Action.transform.parent = Actions.transform;
      Action.transform.localPosition = new Vector3 (0, 128 - (QuantityActions * 51), 0);
      Action.transform.localScale = new Vector3 (1, 1, 1);
      Action.transform.Find ("Text").GetComponent<Text> ().text = "Отмена";
      Action.GetComponent<PlayerAction> ().Type = "Cancel";
      Action.GetComponent<PlayerAction> ().Player = Hero;
   }

   void OldDungeon (SocketIOEvent ev)
   {
      GameObject Actions = Instantiate (ActionsPrefab, new Vector3 (0, 0, 0), Quaternion.identity);
      Actions.transform.parent = AllInfo.transform;
      Actions.transform.localPosition = new Vector3 (0, 0, 0);
      Actions.transform.localScale = new Vector3 (1, 1, 1);
      int QuantityActions = 0;
      GameObject Action;

      for (int i = 0; i < ev.data.Count; i++) {
         Action = Instantiate (ActionPrefab, new Vector3 (0, 0, 0), Quaternion.identity);
         Action.transform.parent = Actions.transform;
         Action.transform.localPosition = new Vector3 (0, 128 - (QuantityActions * 51), 0);
         Action.transform.localScale = new Vector3 (1, 1, 1);
         Action.transform.Find ("Text").GetComponent<Text> ().text = ev.data[i].str;
         Action.GetComponent<PlayerAction> ().Type = "Unite";
         Action.GetComponent<PlayerAction> ().Player = Hero;
         QuantityActions++;
         if (QuantityActions >= 4) break;
      }

      Action = Instantiate (ActionPrefab, new Vector3 (0, 0, 0), Quaternion.identity);
      Action.transform.parent = Actions.transform;
      Action.transform.localPosition = new Vector3 (0, 128 - (QuantityActions * 51), 0);
      Action.transform.localScale = new Vector3 (1, 1, 1);
      Action.transform.Find ("Text").GetComponent<Text> ().text = "Отмена";
      Action.GetComponent<PlayerAction> ().Type = "Cancel";
      Action.GetComponent<PlayerAction> ().Player = Hero;
   }

   public void Redactor (GameObject Sector) // Механизм для генерации уровня со стороны клиента. Не нужная штука, так как используется только при необходимости создать уровень и сразу смотреть на него глазами. В будущем вынесем из кода.
   {
      GameObject Actions = Instantiate (ActionsPrefab, new Vector3 (0, 0, 0), Quaternion.identity);
      Actions.transform.parent = AllInfo.transform;
      Actions.transform.localPosition = new Vector3 (0, 0, 0);
      Actions.transform.localScale = new Vector3 (1, 1, 1);
      int QuantityActions = 0;
      GameObject Action;

      Action = Instantiate (ActionPrefab, new Vector3 (0, 0, 0), Quaternion.identity);
      Action.transform.parent = Actions.transform;
      Action.transform.localPosition = new Vector3 (0, 128 - (QuantityActions * 51), 0);
      Action.transform.localScale = new Vector3 (1, 1, 1);
      Action.transform.Find ("Text").GetComponent<Text> ().text = "Трава";
      Action.GetComponent<PlayerAction> ().Type = "Grass";
      Action.GetComponent<PlayerAction> ().Player = Sector;
      QuantityActions++;

      Action = Instantiate (ActionPrefab, new Vector3 (0, 0, 0), Quaternion.identity);
      Action.transform.parent = Actions.transform;
      Action.transform.localPosition = new Vector3 (0, 128 - (QuantityActions * 51), 0);
      Action.transform.localScale = new Vector3 (1, 1, 1);
      Action.transform.Find ("Text").GetComponent<Text> ().text = "Тропа";
      Action.GetComponent<PlayerAction> ().Type = "Trail";
      Action.GetComponent<PlayerAction> ().Player = Sector;
      QuantityActions++;

      Action = Instantiate (ActionPrefab, new Vector3 (0, 0, 0), Quaternion.identity);
      Action.transform.parent = Actions.transform;
      Action.transform.localPosition = new Vector3 (0, 128 - (QuantityActions * 51), 0);
      Action.transform.localScale = new Vector3 (1, 1, 1);
      Action.transform.Find ("Text").GetComponent<Text> ().text = "Дерево";
      Action.GetComponent<PlayerAction> ().Type = "Tree";
      Action.GetComponent<PlayerAction> ().Player = Sector;
      QuantityActions++;

      Action = Instantiate (ActionPrefab, new Vector3 (0, 0, 0), Quaternion.identity);
      Action.transform.parent = Actions.transform;
      Action.transform.localPosition = new Vector3 (0, 128 - (QuantityActions * 51), 0);
      Action.transform.localScale = new Vector3 (1, 1, 1);
      Action.transform.Find ("Text").GetComponent<Text> ().text = "Гора";
      Action.GetComponent<PlayerAction> ().Type = "Mountain";
      Action.GetComponent<PlayerAction> ().Player = Sector;
      QuantityActions++;

      Action = Instantiate (ActionPrefab, new Vector3 (0, 0, 0), Quaternion.identity);
      Action.transform.parent = Actions.transform;
      Action.transform.localPosition = new Vector3 (0, 128 - (QuantityActions * 51), 0);
      Action.transform.localScale = new Vector3 (1, 1, 1);
      Action.transform.Find ("Text").GetComponent<Text> ().text = "Отмена";
      Action.GetComponent<PlayerAction> ().Type = "Cancel";
      Action.GetComponent<PlayerAction> ().Player = Hero;
   }

   void WrongData(SocketIOEvent ev) // Вызывается в случае передачи неправильных данных. Пользователь будет отключен и перенесен в главное меню.
   {
      Debug.Log(ev.data["reason"].str);
      Loading.active = false;
      MainMenu.active = true;
   }
}
