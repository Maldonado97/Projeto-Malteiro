using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject headingIndicator;
    private TextMeshProUGUI headingIndicatorText;
    // Start is called before the first frame update
    void Start()
    {
        headingIndicatorText = headingIndicator.GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        headingIndicatorText.text = "Heading:" + Mathf.RoundToInt(ShipControl.instance.shipHeading);
    }
}
