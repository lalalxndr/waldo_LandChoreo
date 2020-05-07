using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainControl : MonoBehaviour
{
    [SerializeField] GUISkin _skin = null;
    string GreenAreaSizeX = "3";
    string GreenAreaSizeY = "3";
    string multiple = "100";
    int _multiple = 100;

    Vector3 ClickPostion = Vector3.zero;

    public Transform GreenBuildingCenter;

    public int GreenSizeX, GreenSizeY;
    int _GreenSizeX, _GreenSizeY;

    Vector3 greenCenterPos = Vector3.zero;

    int greenArea;

    Grid grid;

    public int PatternSizeX, PatternSizeY;

    void Awake()
    {
        grid = GetComponent<Grid>();
    }

   
    void Start()
    {
        GreenSizeX = int.Parse(GreenAreaSizeX) - 1;
        GreenSizeY = int.Parse(GreenAreaSizeY) - 1;
        _GreenSizeX = GreenSizeX;
        _GreenSizeY = GreenSizeY;
        greenCenterPos = ClickPostion;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            ClickPostion = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            ClickPostion = new Vector3(ClickPostion.x, 0.5f, ClickPostion.z);
        }

        greenArea = _GreenSizeX * _GreenSizeY;
        GreenSizeX = int.Parse(GreenAreaSizeX)-1;
        GreenSizeY = int.Parse(GreenAreaSizeY)-1;
        _multiple = int.Parse(multiple);

        defineGreenArea(_GreenSizeX, _GreenSizeY, greenCenterPos);
    }

    void OnGUI()
    {
        int i = 1;
        int s = 25;
        GUI.skin = _skin;

        if (GUI.Button(new Rect(s, s * i++, 100, 20), "Generate"))
        {
            greenCenterPos = ClickPostion;
            _GreenSizeX = GreenSizeX;
            _GreenSizeY = GreenSizeY;

            grid.nodeStateReset();
            startFilling = false;
        }

        if (GUI.Button(new Rect(s, s * i++, 100, 20), "Filling"))
        {
            startFilling = true;
        }

        GUI.Label(new Rect(s, s * i++, 100, 20), "GreenAreaSizeX");
        GreenAreaSizeX = GUI.TextField(new Rect(s, s * i++, 100, 20), GreenAreaSizeX);
        GUI.Label(new Rect(s, s * i++, 100, 20), "GreenAreaSizeY");
        GreenAreaSizeY = GUI.TextField(new Rect(s, s * i++, 100, 20), GreenAreaSizeY);
        GUI.Label(new Rect(s, s * i++, 100, 20), "Blue/Green");
        multiple = GUI.TextField(new Rect(s, s * i++, 100, 20), multiple);
    }

    bool startFilling = false;

    //Green
    void defineGreenArea(int _GreenSizeX, int _GreenSizeY, Vector3 center)
    {
        Vector3 leftBottom = center - new Vector3(_GreenSizeX / 2, 0, _GreenSizeY / 2);

        Node conner = grid.NodeFromWorldPoint(leftBottom);
        Node cen = grid.NodeFromWorldPoint(center);

        List<Node> GreenArea = grid.GetGreenArea(conner, _GreenSizeX, _GreenSizeY);
        foreach( Node x in GreenArea)
        {
            x.state = 2;
        }

        // Blue 
        List<Node> BlueArea = grid.GetBlueArea(cen, greenArea, _multiple);

        // Red 
        if (startFilling)
        {
            grid.PlacePattern(cen, 7, 9);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(ClickPostion, 0.75f);
    }








}
