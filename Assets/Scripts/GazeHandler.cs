using UnityEngine;
using System.Collections;
using UnityEngine.VR.WSA.Input;

public class GazeHandler : MonoBehaviour
{
    [SerializeField]
    private TextAsset _myJsonFile;

    private GameObject hitObject;
    [SerializeField]
    private Material hitMaterial;
    private Material oldMaterial;

    [SerializeField]
    private GameObject prefab;

    // Use this for initialization
    void Start()
    {
        //JsonUtility.FromJson<mydata>(_myJsonFile.text

        GestureRecognizer recognizer = new GestureRecognizer();
        recognizer.SetRecognizableGestures(GestureSettings.Tap);
        recognizer.TappedEvent += RecognizerOnTappedEvent;
        recognizer.StartCapturingGestures();

        for (int x = 0; x < 10; x++)
        {
            GameObject.Instantiate(prefab, new Vector3(-1.25F + (0.25F * x), 0.5F, 4.0F), Quaternion.identity);
        }
    }

    // Update is called once per frame
    void Update()
    {
        Ray cameraRay = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit hitInfo;
        if (Physics.Raycast(cameraRay, out hitInfo))
        {
            if (hitObject == hitInfo.transform.gameObject)
            {
                return;
            }
            hitObject = hitInfo.transform.gameObject;
            var myRenderer = hitObject.GetComponent<MeshRenderer>();
            if (myRenderer == null) return;
            oldMaterial = myRenderer.material;
            myRenderer.material = hitMaterial;
        }
        else
        {
            if (hitObject == null)
            {
                return;
            }
            var myRenderer = hitObject.GetComponent<MeshRenderer>();
            if (myRenderer == null) return;
            myRenderer.material = oldMaterial;
            hitObject = null;
        }
    }

    private void RecognizerOnTappedEvent(InteractionSourceKind kind, int tapCount, Ray headRay)
    {
        if (hitObject == null)
        {
            return;
        }
        hitObject.SendMessage("Hit");
    }
}
