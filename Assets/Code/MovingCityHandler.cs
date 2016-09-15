using UnityEngine;
using System.Collections;
using UnityEngine.Windows.Speech;
using System;
using UnityEngine.VR.WSA.Input;
using UnityEngine.VR.WSA.Persistence;
using UnityEngine.VR.WSA;

public class MovingCityHandler : MonoBehaviour {
    private WorldAnchorStore _store;

	// Use this for initialization
	void Start () {
        WorldAnchorStore.GetAsync(AsyncStoreReady);

        KeywordRecognizer KWRecognizer = new KeywordRecognizer(new string[] { "Move here" });
        KWRecognizer.OnPhraseRecognized += onMoveHereRecognized;
        KWRecognizer.Start();

        GestureRecognizer GRecognizer = new GestureRecognizer();

        GRecognizer.SetRecognizableGestures(GestureSettings.Tap);

        GRecognizer.TappedEvent += RecognizerOnDoubleTappedEvent;
        GRecognizer.StartCapturingGestures();
        gameObject.BroadcastMessage("YeeHaw0019");

    }

    private void AsyncStoreReady(WorldAnchorStore store)
    {
        _store = store;
    }

    void Place ()
    {
        var worldAnchor = GetComponent<WorldAnchor>();
        if (worldAnchor == null)
        {
            CreateAnchor();
        } else
        {
            StoreAnchor(worldAnchor);
        }
    }

    private void StoreAnchor(WorldAnchor woldAnchor)
    {
        _store.Delete("myID");
        _store.Save("myID", woldAnchor);
    }

    private void CreateAnchor()
    {
        var wa = gameObject.AddComponent<WorldAnchor>();
        _store.Save("myID", wa);
    }

    private void RecognizerOnDoubleTappedEvent(InteractionSourceKind source, int tapCount, Ray headRay)
    {
        
        return;
        //var go = GameObject.Find("City");
        //GameObject go = .gameObject;
        //Vector3 oldPos = go.transform.position;
        //go.transform.position = new Vector3(oldPos.x + 4, oldPos.y, oldPos.z + 4);
        //go.transform.Translate(4, 0, 4, go.transform);
        //transform.position -= new Vector3(0.1f, 0, 0.1f) ;
        Transform trans = this.gameObject.transform;

        var cameraRay = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit hitInfo;
        var layerMask = 1 << 31;

        //Physics.Raycast(facePoints[i],
        //                        raycastDirection,
        //                        out hitInfo,
        //                        maximumPlacementDistance,
        //                        SpatialMappingManager.Instance.LayerMask

        if (Physics.Raycast(cameraRay, out hitInfo, layerMask))
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
