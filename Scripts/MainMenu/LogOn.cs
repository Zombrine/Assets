using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SocketIO;

public class LogOn : MonoBehaviour {
   
   public GameObject Hero;

   private void OnMouseUpAsButton()
   {
      Hero.GetComponent<MainScript>().LogOn ();
   }
}
