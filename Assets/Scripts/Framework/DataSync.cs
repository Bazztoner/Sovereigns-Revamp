using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataSync : Photon.MonoBehaviour
{
    [PunRPC] [System.Obsolete("No se usa más, usar la que toma un string como segundo parámetro")]
    public void TakeDamage(float damage, string nickName)
    {
        var character = GetComponent<PlayerStats>();

        //TODO: Agregar el string de AttackType
        if (character.enabled && PhotonNetwork.player.NickName != nickName)
            character.TakeDamage(damage);
    }

    [PunRPC]
    public void TakeDamage(float damage, string nickName, string attackType)
    {
        var character = GetComponent<PlayerStats>();

        //TODO: Agregar el string de AttackType
        if (character.enabled && PhotonNetwork.player.NickName != nickName)
            character.TakeDamage(damage, attackType);
    }
}
