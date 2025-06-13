using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Inventory
{
    public class DefenseItem : Item
    {
        public int ID;
        [PunRPC]
        protected override void RPC_RequestDestroy(int viewID)  
        {
            base.RPC_RequestDestroy(viewID);
        }
        [PunRPC]
        protected virtual void RPC_DefenseID(int newDefenseID)
        {
            ID = newDefenseID;
        }
        public void RemoveFromListOnDestory() 
        {
            DurabilityDefenseDatabase.instance.RemoveItemFromList(ID);
        }        
    }
}
