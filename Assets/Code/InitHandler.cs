﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine.Windows.Speech;

public class InitHandler : MonoBehaviour {



    [SerializeField]
    private Material _buildingMaterial;

    [SerializeField] private TextAsset _geojson;
    [SerializeField]
    private GameObject _3dText;

    public delegate void DoVoiceCommand(VoiceCommand c);

    public class VoiceCommand
    {
        public string Command { get; set; }
        public DoVoiceCommand Action { get; set; }
    }

    private List<VoiceCommand> voiceCommands; 


    [System.Serializable]
    public class Geometry
    {
        public string type { get; set; }
        //      public double[] coordinates { get; set; }
        public JSONObject coordinates { get; set; }
        public List<Vector3> vectors { get;set; }
        // List<List<List<List<double>>>> 
    }

    [System.Serializable]
    public class Feature
    {
        public string type { get; set; }
        public Dictionary<string,object> properties { get; set; }
        public Geometry geometry { get; set; }
    }

    public class GeoJson
    {
        public string type { get; set; }      
        public List<Feature> features { get; set; }
        public Vector3 center { get; set; }

    }

    private GeoJson geoJson = new GeoJson();

    private const int EarthRadius = 6378137;

    private const double OriginShift = 2 * Math.PI * EarthRadius / 2;

    public static Vector2d LatLonToMeters(Vector2d v)
    {
        return LatLonToMeters(v.x, v.y);
    }

    //Converts given lat/lon in WGS84 Datum to XY in Spherical Mercator EPSG:900913
    public static Vector2d LatLonToMeters(double lat, double lon)
    {
        var p = new Vector2d();
        p.x = (lon * OriginShift / 180);
        p.y = (Math.Log(Math.Tan((90 + lat) * Math.PI / 360)) / (Math.PI / 180));
        p.y = (p.y * OriginShift / 180);
        return new Vector2d(p.x, p.y);
    }

    private GeoJson loadGeoJson(string text) 
        {
        JSONObject geojson = new JSONObject(text);
        geoJson.features = new List<Feature>();
        var features = geojson["features"];
        for (var fid = 0; fid < features.Count; fid++)
        {
            JSONObject feature = features[fid];
            var f = new Feature();
            f.geometry = new Geometry();
            f.geometry.type = feature["geometry"]["type"].ToString().Replace("\"", "");
            f.geometry.coordinates = feature["geometry"]["coordinates"];

            switch (f.geometry.type)
            {
                case "MultiPolygon":
                    f.geometry.vectors = parsePolygon(f.geometry.coordinates.list[0]);
                    break;
                case "Polygon":
                    f.geometry.vectors = parsePolygon(f.geometry.coordinates);
                    break;
            }
            //switch (f.geometry.type)
            //{
            //    case "MultiPolygon":

            //        break;
            //}
            f.properties = new Dictionary<string, object>();
            foreach (var s in feature["properties"].keys)
            {
                f.properties[s] = feature["properties"][s];
            }
            geoJson.features.Add(f);
        }
        geoJson.center = geoJson.features[0].geometry.vectors[0];

        return geoJson;
    }


    void InitVoiceCommands()
    {

        voiceCommands = new List<VoiceCommand>();

        var go = gameObject.transform;
        foreach (Transform c in go)
        {
            var vcHide = new VoiceCommand();
            vcHide.Command = "Hide " + c.name;
            vcHide.Action = (VoiceCommand command) =>
            {
                c.gameObject.SetActive(false);                
            };
            voiceCommands.Add(vcHide);

            var vcShow = new VoiceCommand();
            vcShow.Command = "Show " + c.name;
            vcShow.Action = (VoiceCommand command) =>
            {
                c.gameObject.SetActive(true);
            };
            voiceCommands.Add(vcShow);

        }
        
        KeywordRecognizer recognizer = new KeywordRecognizer(voiceCommands.Select(k => k.Command).ToArray());
        recognizer.OnPhraseRecognized += Recognizer_OnPhraseRecognized;
        recognizer.Start();
        
    }

    private void Recognizer_OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        foreach (var vc in voiceCommands)
        {
            if (vc.Command == args.text)
            {
                vc.Action(vc);
            }
        }        
    }

    void AddBuildings()
    {
        string encodedString = _geojson.text; // "{\"field1\": 0.5,\"field2\": \"sampletext\",\"field3\": [1,2,3]}";
        var geoJson = loadGeoJson(encodedString);

        setText(geoJson.features.Count + " features");

        var main = new GameObject("Buildings");
        main.transform.SetParent(this.gameObject.transform, false);
        var scale = 0.001f;
        this.gameObject.transform.position = new Vector3(-geoJson.center.x * scale, -0.5f, -geoJson.center.z * scale);
        //      main.transform.Rotate(new Vector3(0.9f, 0, 0));
        this.gameObject.transform.localScale = new Vector3(scale, scale, scale);

        var i = 0;
        foreach (var f in geoJson.features)
        {
            i += 1;
            List<Vector3> contour = f.geometry.vectors;

            var verts = new List<Vector3>();
            var indices = new List<int>();
            var height = float.Parse(f.properties["gemiddelde_hoogte"].ToString());
            MeshHelpers.CreateMesh(contour, height, verts, indices);
            MeshHelpers.CreateGameObject("a" + i, verts, indices, main, _buildingMaterial);

        }
    }


    // Use this for initialization
    void Start()
    {
        Debug.Log("Hoi");
        AddBuildings();
        

        InitVoiceCommands();


    }

    List<Vector3> parseMultiPolygon(JSONObject coords)
    {


        List<Vector3> result = new List<Vector3> { };
        if (coords == null) return result;
        result.AddRange(parsePolygon(coords.list[0]));

        return result;
    }

    List<Vector3> parsePolygon(JSONObject coords)
    {
        List<Vector3> result = new List<Vector3> { };
        if (coords == null) return result;
        var points = coords.list[0];
        foreach (var p in points.list.Take(points.list.Count-1))
        {
            var lat = float.Parse(p.list[1].ToString());
            var lon = float.Parse(p.list[0].ToString());
            var mp = LatLonToMeters(new Vector2d(lat, lon));
            result.Add(new Vector3((float)mp.x, 0, (float)mp.y));
        }

        return result;
    }

    void setText(string text)
    {
        _3dText.GetComponent<TextMesh>().text = text;

    }

    // Update is called once per frame
    void Update () {
	
	}
}


