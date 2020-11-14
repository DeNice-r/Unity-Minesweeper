using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    GameObject[,] cells = new GameObject[9, 9];
    public int mineNum;
    public Text time;
    public Text mines;
    private DateTime gameStarted;
    int unfmines;
    bool playing = true;

    void Start()
    {
        placeMines();
        placeClues();
        fillMap();
        mines.text = mineNum.ToString();
        unfmines = mineNum;
        gameStarted = DateTime.Now;
        InvokeRepeating("Timer", 0f, 0.05f);
    }

    private void Update()
    {
        if (playing && (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)))
        {
            Vector3 mpos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            int x = Mathf.RoundToInt(mpos.x);
            int y = Mathf.RoundToInt(mpos.y);
            if (x >= 0 && y >= 0 && x < 9 && y < 9)
            {
                if (Input.GetMouseButtonDown(0) && !cells[x,y].GetComponent<Tile>().isFlagged)
                {
                    recurUncover(x, y);
                }
                else if (Input.GetMouseButtonDown(1))
                {
                    if (cells[x, y].GetComponent<Tile>().Flag())
                    {
                        if (cells[x, y].GetComponent<Tile>().tileKind == Tile.Kind.Mine)
                            unfmines--;
                        mines.text = (Convert.ToInt32(mines.text) - 1).ToString();
                        if(unfmines == 0)
                        {
                            win();
                        }
                    }
                    else
                    {
                        if (cells[x, y].GetComponent<Tile>().tileKind == Tile.Kind.Mine)
                            unfmines++;
                        mines.text = (Convert.ToInt32(mines.text) + 1).ToString();
                    }
                }
            }
        }
    }

    void fillMap()
    {
        for (int x = 0; x < Math.Sqrt(cells.Length); x++)
            for (int y = 0; y < Math.Sqrt(cells.Length); y++)
            {
                if (cells[x, y] == null)
                {
                    GameObject nclue = Instantiate(Resources.Load("Prefabs/Blank"), new Vector3(x, y, 0), Quaternion.identity) as GameObject;
                    cells[x, y] = nclue;
                }
            }
    }

    void placeMines()
    {
        for (int n = 0; n < mineNum; n++)
        {
            int x = Random.Range(0, 9);
            int y = Random.Range(0, 9);

            if (cells[x, y] == null)
            {
                GameObject nmine = Instantiate(Resources.Load("Prefabs/Mine"), new Vector3(x, y, 0), Quaternion.identity) as GameObject;
                cells[x, y] = nmine;
            }
            else
                n--;
        }
    }

    void placeClues()
    {
        for (int x = 0; x < Math.Sqrt(cells.Length); x++)
            for (int y = 0; y < Math.Sqrt(cells.Length); y++)
            {
                if (cells[x, y] == null)
                {
                    int minenum = 0;
                    if (x > 0 && cells[x - 1, y] != null && cells[x - 1, y].GetComponent<Tile>().tileKind == Tile.Kind.Mine)
                        minenum++;
                    if (y > 0 && cells[x, y - 1] != null && cells[x, y - 1].GetComponent<Tile>().tileKind == Tile.Kind.Mine)
                        minenum++;
                    if (x > 0 && y > 0 && cells[x - 1, y - 1] != null && cells[x - 1, y - 1].GetComponent<Tile>().tileKind == Tile.Kind.Mine)
                        minenum++;
                    if (x < 8 && cells[x + 1, y] != null && cells[x + 1, y].GetComponent<Tile>().tileKind == Tile.Kind.Mine)
                        minenum++;
                    if (y < 8 && cells[x, y + 1] != null && cells[x, y + 1].GetComponent<Tile>().tileKind == Tile.Kind.Mine)
                        minenum++;
                    if (x < 8 && y < 8 && cells[x + 1, y + 1] != null && cells[x + 1, y + 1].GetComponent<Tile>().tileKind == Tile.Kind.Mine)
                        minenum++;
                    if (x < 8 && y > 0 && cells[x + 1, y - 1] != null && cells[x + 1, y - 1].GetComponent<Tile>().tileKind == Tile.Kind.Mine)
                        minenum++;
                    if (x > 0 && y < 8 && cells[x - 1, y + 1] != null && cells[x - 1, y + 1].GetComponent<Tile>().tileKind == Tile.Kind.Mine)
                        minenum++;
                    if (minenum > 0)
                    {
                        GameObject nclue;
                        nclue = Instantiate(Resources.Load("Prefabs/Num " + minenum), new Vector3(x, y, 0), Quaternion.identity) as GameObject;
                        Destroy(cells[x, y]);
                        cells[x, y] = nclue;
                    }
                }
            }
    }

    private void recurUncover(int x, int y)
    {
        GameObject obj = cells[x, y];
        Tile t = obj.GetComponent<Tile>();

        if (t.Uncover())
            lose();

        if (t.tileKind == Tile.Kind.Blank) // TODO: Руструктурировать так, чтобы если первая цифра, а рядом пустое поле, то пустое открывается
        {
            if (x > 0 && cells[x - 1, y].GetComponent<Tile>().isCovered)
                if (cells[x - 1, y].GetComponent<Tile>().tileKind == Tile.Kind.Blank)
                    recurUncover(x - 1, y);
                else
                    cells[x - 1, y].GetComponent<Tile>().Uncover();
            if (y > 0 && cells[x, y - 1].GetComponent<Tile>().isCovered)
                if (cells[x, y - 1].GetComponent<Tile>().tileKind == Tile.Kind.Blank)
                    recurUncover(x, y - 1);
                else
                    cells[x, y - 1].GetComponent<Tile>().Uncover();
            if (x > 0 && y > 0 && cells[x - 1, y - 1].GetComponent<Tile>().isCovered)
                if (cells[x - 1, y - 1].GetComponent<Tile>().tileKind == Tile.Kind.Blank)
                    recurUncover(x - 1, y - 1);
                else
                    cells[x - 1, y - 1].GetComponent<Tile>().Uncover();
            if (x < 8 && cells[x + 1, y].GetComponent<Tile>().isCovered)
                if (cells[x + 1, y].GetComponent<Tile>().tileKind == Tile.Kind.Blank)
                    recurUncover(x + 1, y);
                else
                    cells[x + 1, y].GetComponent<Tile>().Uncover();
            if (y < 8 && cells[x, y + 1].GetComponent<Tile>().isCovered)
                if (cells[x, y + 1].GetComponent<Tile>().tileKind == Tile.Kind.Blank)
                    recurUncover(x, y + 1);
                else
                    cells[x, y + 1].GetComponent<Tile>().Uncover();
            if (x < 8 && y < 8 && cells[x + 1, y + 1].GetComponent<Tile>().isCovered)
                if (cells[x + 1, y + 1].GetComponent<Tile>().tileKind == Tile.Kind.Blank)
                    recurUncover(x + 1, y + 1);
                else
                    cells[x + 1, y + 1].GetComponent<Tile>().Uncover();
            if (x < 8 && y > 0 && cells[x + 1, y - 1].GetComponent<Tile>().isCovered)
                if (cells[x + 1, y - 1].GetComponent<Tile>().tileKind == Tile.Kind.Blank)
                    recurUncover(x + 1, y - 1);
                else
                    cells[x + 1, y - 1].GetComponent<Tile>().Uncover();
            if (x > 0 && y < 8 && cells[x - 1, y + 1].GetComponent<Tile>().isCovered)
                if (cells[x - 1, y + 1].GetComponent<Tile>().tileKind == Tile.Kind.Blank)
                    recurUncover(x - 1, y + 1);
                else
                    cells[x - 1, y+1].GetComponent<Tile>().Uncover();
        }
        else if (t.tileKind == Tile.Kind.Clue)
        {
            if (x > 0 && cells[x - 1, y].GetComponent<Tile>().isCovered)
                if (cells[x - 1, y].GetComponent<Tile>().tileKind == Tile.Kind.Blank)
                    recurUncover(x - 1, y);
            if (y > 0 && cells[x, y - 1].GetComponent<Tile>().isCovered)
                if (cells[x, y - 1].GetComponent<Tile>().tileKind == Tile.Kind.Blank)
                    recurUncover(x, y - 1);
            if (x > 0 && y > 0 && cells[x - 1, y - 1].GetComponent<Tile>().isCovered)
                if (cells[x - 1, y - 1].GetComponent<Tile>().tileKind == Tile.Kind.Blank)
                    recurUncover(x - 1, y - 1);
            if (x < 8 && cells[x + 1, y].GetComponent<Tile>().isCovered)
                if (cells[x + 1, y].GetComponent<Tile>().tileKind == Tile.Kind.Blank)
                    recurUncover(x + 1, y);
            if (y < 8 && cells[x, y + 1].GetComponent<Tile>().isCovered)
                if (cells[x, y + 1].GetComponent<Tile>().tileKind == Tile.Kind.Blank)
                    recurUncover(x, y + 1);
            if (x < 8 && y < 8 && cells[x + 1, y + 1].GetComponent<Tile>().isCovered)
                if (cells[x + 1, y + 1].GetComponent<Tile>().tileKind == Tile.Kind.Blank)
                    recurUncover(x + 1, y + 1);
            if (x < 8 && y > 0 && cells[x + 1, y - 1].GetComponent<Tile>().isCovered)
                if (cells[x + 1, y - 1].GetComponent<Tile>().tileKind == Tile.Kind.Blank)
                    recurUncover(x + 1, y - 1);
            if (x > 0 && y < 8 && cells[x - 1, y + 1].GetComponent<Tile>().isCovered)
                if (cells[x - 1, y + 1].GetComponent<Tile>().tileKind == Tile.Kind.Blank)
                    recurUncover(x - 1, y + 1);
        }
    }

    private void lose()
    {
        playing = false;
        CancelInvoke("Timer");
        foreach(GameObject obj in cells)
        {
            Tile t = obj.GetComponent<Tile>();
            t.Uncover(true);
        }
    }

    private void win()
    {
        playing = false;
        CancelInvoke("Timer");
        foreach (GameObject obj in cells)
        {
            Tile t = obj.GetComponent<Tile>();
            t.Uncover();
        }
        if (PlayerPrefs.GetFloat("record") > float.Parse(time.text))
            PlayerPrefs.SetFloat("record", float.Parse(time.text));
    }

    private void Timer()
    {
        time.text = Math.Round((DateTime.Now - gameStarted).TotalSeconds, 3).ToString();
    }

    public void GoToMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void Restart()
    {
        SceneManager.LoadScene(1);
    }
}
