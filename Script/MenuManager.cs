using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public static MenuManager MenuManagerInstance;
    public bool GameState;
    public GameObject [] menuElement=new GameObject[2];

    void Start()
    {
        GameState = false;
        MenuManagerInstance = this;
    }

    
    void Update()
    {
        
    }
    public void StartTheGame()
    {
        GameState = true;
        menuElement[0].SetActive(false);
        GameObject.FindWithTag("particle").GetComponent<ParticleSystem>().Play();

    }

}
