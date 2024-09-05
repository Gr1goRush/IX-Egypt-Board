using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

public static class ScenesLoader
{
    public static void LoadMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    public static void LoadGame()
    {
        SceneManager.LoadScene("Game");
    }
}