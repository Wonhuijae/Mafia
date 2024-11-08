using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class PlayerSelecter : MonoBehaviour
{
    [SerializeField]
    private List<CharacterInfo> infoes;

    public Dictionary<CharacterInfo, GameObject> spawnDict = new();

    public Transform spawnPos;

    private Outline outline;
    private CharacterInfo selectInfo;

    private void Awake()
    {
        foreach(CharacterInfo info in infoes)
        {
            spawnDict.Add(info, Instantiate(info.charModel, spawnPos.position, Quaternion.identity));
            spawnDict[info].SetActive(false);
        }
    }

    public void SelectCharacter(CharacterInfo _characterInfo)
    {
        if(selectInfo != null) spawnDict[selectInfo].SetActive(false);

        selectInfo = _characterInfo;
        spawnDict[_characterInfo].SetActive(true);
    }

    public void SetButton(Outline _outline)
    {
        if (outline != null) outline.enabled = false;
        if (outline == _outline)
        {
            _outline.enabled = true;
            return;
        }

        outline = _outline;
    }
}
