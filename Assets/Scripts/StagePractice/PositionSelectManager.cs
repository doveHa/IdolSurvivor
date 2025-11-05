using UnityEngine;

public class PositionSelectManager : MonoBehaviour
{
    public GameObject PositionPanel;
    public GameObject SongPartPanel;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ShowSongPartPanel();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowPositionPanel()
    {
        PositionPanel.SetActive(true);
        SongPartPanel.SetActive(false);
    }

    public void ShowSongPartPanel()
    {
        PositionPanel.SetActive(false);
        SongPartPanel.SetActive(true);
    }

    public void OnClickPosition()
    {

    }
}
