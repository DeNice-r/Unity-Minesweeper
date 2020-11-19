using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public enum Kind {Blank, Mine, Clue}

    [HideInInspector]
    public bool isCovered = true;
    [HideInInspector]
    public bool isFlagged = false;
    public Kind tileKind;
    [HideInInspector]
    public Sprite DefSprite;
    public Sprite CovSprite;
    public Sprite FlagSprite;
    public Sprite XmineSprite;
    public Sprite RedMineSprite;

    private void Start()
    {
        DefSprite = gameObject.GetComponent<SpriteRenderer>().sprite;
        gameObject.GetComponent<SpriteRenderer>().sprite = CovSprite;
    }
    public bool Flag()
    {
        if (isCovered)
        {
            if (isFlagged)
            {
                GetComponent<SpriteRenderer>().sprite = CovSprite;
                isFlagged = false;
            }
            else
            {
                GetComponent<SpriteRenderer>().sprite = FlagSprite;
                isFlagged = true;
            }
        }
        return isFlagged;
    }
    public bool Uncover(bool lose = false)
    {
        if (isCovered && !isFlagged)
        {
            if (tileKind == Kind.Mine)
            {
                if (lose)
                    GetComponent<SpriteRenderer>().sprite = DefSprite;
                else
                    GetComponent<SpriteRenderer>().sprite = RedMineSprite;
                isCovered = false;
                return true;
            }
            else if (!lose)
            {
                GetComponent<SpriteRenderer>().sprite = DefSprite;
            }
        }
        else if (lose && isFlagged)
        {
            if (tileKind != Kind.Mine)
            {
                GetComponent<SpriteRenderer>().sprite = XmineSprite;
            }
        }
        isCovered = false;
        return false;
    }
}
