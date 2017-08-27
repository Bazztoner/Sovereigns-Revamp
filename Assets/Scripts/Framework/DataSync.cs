using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataSync : Photon.MonoBehaviour {

    [PunRPC]
    public void TakeDamage(float damage, string nickName)
    {
        var character = GetComponent<PlayerStats>();

        if (character.enabled && PhotonNetwork.player.NickName != nickName)
            character.TakeDamage(damage);
    }
}
