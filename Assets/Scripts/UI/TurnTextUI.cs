using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnTextUI : MonoBehaviour
{
    public TurnController turnController;

    private void Start()
    {
        turnController.onTurnEnded += OnTurnEnded;
        GetComponent<Text>().text = "Turn Number: " + turnController.GetCurrentTurnNumber();
    }

    private void OnTurnEnded()
    {
        GetComponent<Text>().text = "Turn Number: " + turnController.GetCurrentTurnNumber();
    }

    private void OnDisable()
    {
        turnController.onTurnEnded -= OnTurnEnded;
    }
}
