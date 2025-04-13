using UnityEngine;
using UnityEngine.SceneManagement;

public class sceneDispatch : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void loadTitle(){
        SceneManager.LoadScene("title", LoadSceneMode.Single);
    }
    public void loadGame(){
        SceneManager.LoadScene("game", LoadSceneMode.Single);
    }
    public void loadGameOver(){
        SceneManager.LoadScene("gameOver", LoadSceneMode.Single);
    }
    public void loadWin(){
        SceneManager.LoadScene("victory", LoadSceneMode.Single);
    }
    public void quit(){
        Application.Quit();
    }
}
