using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// StepCheck-Klasse.
/// Realisierung der zeitlichen Kontrolle, wann der Proband/ die Probandin auf die Planke tritt. (Beispiel für zukünftige Datenauswertungen)
/// 
/// author: Robin Staab
/// date: 31.05.2022 09:00
/// copyright: §69a Absatz 3 Satz 1 UrhG
/// last editor: Robin Staab
/// 
/// </summary>
/// 
public class StepCheck : MonoBehaviour
{
    bool stepped;

    /// <summary>
    /// Start
    /// </summary>
    private void Start()
    {
        stepped = false;
    }

    /// <summary>
    /// Update
    /// </summary>
    private void Update()
    {
        if(!stepped && this.transform.localPosition.x > 1.05f)
        {
            stepped = true;
            Logger.Log("Player stepped on the plank.");
        }
    }
}
