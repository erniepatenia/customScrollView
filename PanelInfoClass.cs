using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PanelInfoClass : MonoBehaviour
{
    public PanelInfoData data;
    public List<PanelInfoData> PanelList = new List<PanelInfoData>();
    public GameObject panelTemplate;
    public GameObject ScrollView;

    public PanelInfoClass(int newValue)
    {
        data = new PanelInfoData(newValue);
    }

    public int Value
    {
        get { return data.value; }
        set { data.value = value; }
    }
}