using UnityEngine; using System.Collections.Generic;
public class Shop : MonoBehaviour {
  public void BuySkin(string skinId, int price){
    var data = SaveSystem.Load();
    if(data.coins >= price){
      data.coins -= price;
      var list = new List<string>(data.ownedSkins);
      if(!list.Contains(skinId)) list.Add(skinId);
      data.ownedSkins = list.ToArray();
      data.equippedSkin = skinId;
      SaveSystem.Save(data);
    }
  }
}
