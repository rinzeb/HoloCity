using UnityEngine;
using System.Collections;
using UnityEngine.VR.WSA.Input;

public class GazeHandler : MonoBehaviour {


    private GameObject _hitObject;

	// Use this for initialization
	void Start () {
        GestureRecognizer recognizer = new GestureRecognizer();
        recognizer.SetRecognizableGestures(GestureSettings.Tap);
        recognizer.TappedEvent += Recognizer_TappedEvent;
        recognizer.StartCapturingGestures();

        //SpatialInteractionManager manager = new SpatialInteractionManager();
        //manager.InteractionDetected += Manager_InteractionDetected;


    }


    private void Recognizer_TappedEvent(InteractionSourceKind source, int tapCount, Ray headRay)
    {
        if (_hitObject == null) return;
        _hitObject.SendMessage("Tap");

//        throw new System.NotImplementedException();
    }

    // Update is called once per frame
    void Update () {
        Ray cameraRay = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit hitInfo;
        if (Physics.Raycast(cameraRay, out hitInfo))
        {
            if (_hitObject == hitInfo.transform.gameObject)
            {
                return;                
            }
            _hitObject = hitInfo.transform.gameObject;
            _hitObject.SendMessage("Gaze",true);
            var myRenderer = _hitObject.GetComponent<MeshRenderer>();            
        }
        else
        {
            if (_hitObject == null) return;
            _hitObject.SendMessage("Gaze", false);
            _hitObject = null;
            


        }
    }
}

