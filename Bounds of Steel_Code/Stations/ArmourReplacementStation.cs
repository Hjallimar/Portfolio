using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmourReplacementStation : StationBase
{
    [SerializeField] private float timeBeforeReplace = 10.0f;

    private PlayerHealthSystem playerHealthSystem = null;
    private float timeSpentReplacing = 0.0f;


    new protected void Awake()
    {
        base.Awake();
        stationActivated += ResetTimer;
    }

    // Start is called before the first frame update
    new protected void Start()
    {
        base.Start();
        playerHealthSystem = GameController.Player.GetComponentInChildren<PlayerHealthSystem>();
    }

    // Update is called once per frame
    new protected void Update()
    {
        base.Update();

        if (stationActive)
        {
            timeSpentReplacing += Time.deltaTime;

            if(timeSpentReplacing >= timeBeforeReplace && playerHealthSystem != null)
            {
                playerHealthSystem.ReplaceWeakestArmourPlate();
                ResetTimer();
            }
        }
    }

    private void ResetTimer()
    {
        timeSpentReplacing = 0.0f;
    }



}
