using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class bossSkill : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(StartGame());        
    }

    public void ChangeBossSkillSprite()
    {
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("Role", out object selfRoleObj))
        {
            if (selfRoleObj is string selfRole)
            {
                gameName = selfRole;
            }
        }

        if(gameName == "PeanutGhost")
        {
            image.sprite = bossSkillImages[0];
        }
        else if (gameName == "PukeGirlGhost")
        {
            image.sprite = bossSkillImages[1];
        }
        else if (gameName == "ProteinGhost")
        {
            image.sprite = bossSkillImages[2];
        }
 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    IEnumerator StartGame()
    {
        while (true)
        {
            if (AllPlayersHaveRoles())
            {
                ChangeBossSkillSprite();                
                yield break;
            }
            yield return null;
        }
    }

    private bool AllPlayersHaveRoles()
    {
        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (!player.CustomProperties.ContainsKey("Role"))
            {
                return false;
            }

            if (!player.CustomProperties.ContainsKey("SpawnIndex"))
            {
                return false;
            }
        }
        return true;
    }

    [SerializeField]
    List<Sprite> bossSkillImages = new List<Sprite>();
    [SerializeField]
    Image image;

    string gameName;
}
