using UnityEngine;
using UnityEngine.UI;

public class TicTacToeGame : MonoBehaviour
{
    public Sprite crossSprite; // Ristin kuva
    public Sprite circleSprite; // Nollan kuva
    public Text winnerText; // Teksti voittajan näyttämiseksi

    private bool isPlayerTurn = true; // Muuttuja pelaajan vuoron seuraamiseksi
    private bool gameOver = false; // Muuttuja pelin päättymisen seuraamiseksi

    public Button[,] buttons; // Pelilaudan esittäminen

    void Start()
    {
        // Alustetaan napi manuaalisesti
        buttons = new Button[3, 3];

        // Liitetään jokainen nappi pelilaudalle jokaiseen soluun
        buttons[0, 0] = GameObject.Find("Button1").GetComponent<Button>();
        buttons[0, 1] = GameObject.Find("Button2").GetComponent<Button>();
        buttons[0, 2] = GameObject.Find("Button3").GetComponent<Button>();

        buttons[1, 0] = GameObject.Find("Button4").GetComponent<Button>();
        buttons[1, 1] = GameObject.Find("Button5").GetComponent<Button>();
        buttons[1, 2] = GameObject.Find("Button6").GetComponent<Button>();

        buttons[2, 0] = GameObject.Find("Button7").GetComponent<Button>();
        buttons[2, 1] = GameObject.Find("Button8").GetComponent<Button>();
        buttons[2, 2] = GameObject.Find("Button9").GetComponent<Button>();

        // Tapahtumankäsittelijä
        AssignButtonClickHandlers();
    }

    void AssignButtonClickHandlers()
    {
        // Liitetään tapahtumankäsittelijä jokaiselle napille
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                int x = i; // Luodaan väliaikaiset muuttujat sulkujen käytön mahdollistamiseksi
                int y = j;

                buttons[i, j].onClick.RemoveAllListeners(); // Poistetaan kaikki aiemmat kuuntelijat
                buttons[i, j].onClick.AddListener(() => HandleCellClick(x, y));
            }
        }
    }

    public void HandleCellClick(int x, int y)
    {
        Debug.Log($"Solua klikattiin: {x}, {y}");

        // Tarkistetaan, ettei kuvaa muuteta, jos solu on jo varattu tai peli on päättynyt
        if (!gameOver && buttons[x, y].image.sprite == null && buttons[x, y].interactable)
        {
            if (isPlayerTurn)
            {
                // Pelaajan vuoro
                buttons[x, y].image.sprite = circleSprite;
            }
            else
            {
                // Botin vuoro
                buttons[x, y].image.sprite = crossSprite;
            }

            // Muutetaan nappi käyttämättömäksi
            buttons[x, y].interactable = false;

            // Voiton ja tasapelin ehdot
            if (CheckForWin())
            {
                winnerText.text = isPlayerTurn ? "Pelaaja voittaa!" : "Botti voittaa!";
                gameOver = true;
            }
            else if (CheckForDraw())
            {
                winnerText.text = "Tasapeli!";
                gameOver = true;
            }

            isPlayerTurn = !isPlayerTurn;

            // Jos on botin vuoro
            if (!isPlayerTurn && !gameOver)
            {
                BotMove();
            }
        }
    }

    void BotMove()
    {
        if (!gameOver)
        {
            // Kutsutaan Minimax-algoritmia parhaan siirron määrittämiseksi
            int[] bestMove = Minimax();

            // Käsitellään paras siirto
            HandleCellClick(bestMove[0], bestMove[1]);
        }
    }

    int[] Minimax()
    {
        int bestScore = int.MinValue;
        int[] bestMove = new int[2];

        // Käydään läpi kaikki mahdolliset siirrot
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                // Tarkistetaan, että solu on vapaa
                if (buttons[i, j].image.sprite == null)
                {
                    // Tehdään siirto
                    buttons[i, j].image.sprite = crossSprite;

                    // Arvioidaan siirto Minimaxin avulla
                    int score = MinimaxScore(false);

                    // Peruuta siirto
                    buttons[i, j].image.sprite = null;

                    // Jos arvio on parempi kuin nykyinen paras mahdollinen
                    if (score > bestScore)
                    {
                        bestScore = score;
                        bestMove[0] = i;
                        bestMove[1] = j;
                    }
                }
            }
        }

        return bestMove;
    }

    int MinimaxScore(bool isMaximizing)
    {
        if (CheckForWin())
        {
            return isMaximizing ? -1 : 1; // Jos voitto, palautetaan -1 botin siirtoa varten, 1 pelaajan siirtoa varten
        }
        else if (CheckForDraw())
        {
            return 0; // Tasapeli
        }

        // Kutsutaan Minimaxia rekursiivisesti kaikille mahdollisille siirroille
        if (isMaximizing)
        {
            int bestScore = int.MinValue;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (buttons[i, j].image.sprite == null)
                    {
                        buttons[i, j].image.sprite = crossSprite;
                        bestScore = Mathf.Max(bestScore, MinimaxScore(false));
                        buttons[i, j].image.sprite = null;
                    }
                }
            }
            return bestScore;
        }
        else
        {
            int bestScore = int.MaxValue;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (buttons[i, j].image.sprite == null)
                    {
                        buttons[i, j].image.sprite = circleSprite;
                        bestScore = Mathf.Min(bestScore, MinimaxScore(true));
                        buttons[i, j].image.sprite = null;
                    }
                }
            }
            return bestScore;
        }
    }

    bool CheckForWin()
    {
        // Tarkistetaan voitto

        // Tarkistetaan rivit ja sarakkeet
        for (int i = 0; i < 3; i++)
        {
            if (buttons[i, 0].image.sprite == buttons[i, 1].image.sprite && buttons[i, 1].image.sprite == buttons[i, 2].image.sprite && buttons[i, 0].image.sprite != null)
                return true;

            if (buttons[0, i].image.sprite == buttons[1, i].image.sprite && buttons[1, i].image.sprite == buttons[2, i].image.sprite && buttons[0, i].image.sprite != null)
                return true;
        }

        // Tarkistetaan halkaisijat
        if (buttons[0, 0].image.sprite == buttons[1, 1].image.sprite && buttons[1, 1].image.sprite == buttons[2, 2].image.sprite && buttons[0, 0].image.sprite != null)
            return true;

        if (buttons[0, 2].image.sprite == buttons[1, 1].image.sprite && buttons[1, 1].image.sprite == buttons[2, 0].image.sprite && buttons[0, 2].image.sprite != null)
            return true;

        return false;
    }

    bool CheckForDraw()
    {
        // Tarkistetaan tasapeli
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (buttons[i, j].image.sprite == null)
                    return false; // On tyhjiä soluja, peli ei ole päättynyt
            }
        }

        // Kaikki solut ovat varattuja, eikä voittoa ole havaittu, joten kyseessä on tasapeli
        return true;
    }
}
