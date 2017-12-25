using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hit : MonoBehaviour {

   private void OnMouseUpAsButton()
   {
      GameObject Fight = GameObject.Find("Location/Hero/Main Camera/Canvas/Battle/Fight");
      Fight.GetComponent<Fight>().SuccessHits++;
      Fight.GetComponent<Fight>().NextHit ();
   }
}
