using UnityEngine;
using System.Collections;
using UnityEngine.Windows.Speech;
using System;
using UnityEngine.VR.WSA.Input;

public class MovingCityHandler : MonoBehaviour {

	// Use this for initialization
	void Start () {
        KeywordRecognizer KWRecognizer = new KeywordRecognizer(new string[] { "Move here" });
        KWRecognizer.OnPhraseRecognized += onMoveHereRecognized;
        KWRecognizer.Start();

        GestureRecognizer GRecognizer = new GestureRecognizer();

        GRecognizer.SetRecognizableGestures(GestureSettings.Tap);

        GRecognizer.TappedEvent += RecognizerOnDoubleTappedEvent;
        GRecognizer.StartCapturingGestures();

    }

    private void RecognizerOnDoubleTappedEvent(InteractionSourceKind source, int tapCount, Ray headRay)
    {
        //var go = GameObject.Find("City");
        //GameObject go = .gameObject;
        //Vector3 oldPos = go.transform.position;
        //go.transform.position = new Vector3(oldPos.x + 4, oldPos.y, oldPos.z + 4);
        //go.transform.Translate(4, 0, 4, go.transform);
        //transform.position -= new Vector3(0.1f, 0, 0.1f) ;
        Transform trans = this.gameObject.transform;

        var cameraRay = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit hitInfo;

        if (Physics.Raycast(cameraRay, out hitInfo, 31))
        {
            var _hitObject = hitInfo.transform.gameObject;

            if (_hitObject == null || _hitObject.layer != 31)
            {
                return;
            }

            //if (_cursor == null)
            //    return;
            //_cursor.position = hitInfo.point;
            //_cursor.up = hitInfo.normal;

            var pointTo = hitInfo.point;
            //var pointFrom = go.transform.position;
            //var transformVector = Vector3.MoveTowards(pointFrom, pointTo, 1000000);

            trans.position = pointTo;

        }
        else
        {
            //HandleEmptyHit();
        }
    }

    private void onMoveHereRecognized(PhraseRecognizedEventArgs args)
    {
        GameObject go = gameObject;
        Vector3 oldPos = go.transform.position;
        go.transform.position = new Vector3(oldPos.x+4, oldPos.y, oldPos.z+4);

    }

    // Update is called once per frame
    void Update () {
	
	}
}
