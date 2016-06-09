using UnityEngine;
using System.Collections;
using UnityEngine.VR.WSA.WebCam;
using System.Linq;

// Takes a picture upon selection and changes the texture to it.
public class HoloCapture : MonoBehaviour {
    PhotoCapture photoCaptureObject = null;
    public Material captureMaterial;
    public Material cubeMaterial;
    // Use this for initialization
    void Start ()
    {
        Debug.Log("Start called");
        captureMaterial.mainTexture = Texture2D.whiteTexture;
        cubeMaterial.color = new Color(0f, 0f, 1f);
    }
	
	// Update is called once per frame
	void Update () {
	
	}
    
    void OnTakePicture()
    {
        Debug.Log("OnTakePicture called");
        PhotoCapture.CreateAsync(true, OnPhotoCaptureCreated);
    }

    void OnPhotoCaptureCreated(PhotoCapture captureObject)
    {
        Debug.Log("OnPhotoCaptureCreated called");
        captureMaterial.mainTexture = Texture2D.blackTexture;
        cubeMaterial.color = new Color(1f, 1f, 0f);

        photoCaptureObject = captureObject;

        Resolution cameraResolution = PhotoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).First();

        CameraParameters c = new CameraParameters();
        c.hologramOpacity = 0.5f;
        c.cameraResolutionWidth = cameraResolution.width;
        c.cameraResolutionHeight = cameraResolution.height;
        c.pixelFormat = CapturePixelFormat.BGRA32;

        captureObject.StartPhotoModeAsync(c, false, OnPhotoModeStarted);
    }

    void OnStoppedPhotoMode(PhotoCapture.PhotoCaptureResult result)
    {
        Debug.Log("OnStoppedPhotoMode called");
        photoCaptureObject.Dispose();
        photoCaptureObject = null;
    }

    private void OnPhotoModeStarted(PhotoCapture.PhotoCaptureResult result)
    {
        Debug.Log("OnPhotoModeStarted called");
        if (result.success)
        {
            photoCaptureObject.TakePhotoAsync(OnCapturedPhotoToMemory);
        }
        else
        {
            Debug.LogError("Unable to start photo mode!");
            photoCaptureObject = null;
        }
    }

    void OnCapturedPhotoToMemory(PhotoCapture.PhotoCaptureResult result, PhotoCaptureFrame photoCaptureFrame)
    {
        Debug.Log("OnCapturedPhotoToMemory called");
        if (result.success)
        {
            // Create our Texture2D for use and set the correct resolution
            Resolution cameraResolution = PhotoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).First();
            Texture2D targetTexture = new Texture2D(cameraResolution.width, cameraResolution.height);
            // Copy the raw image data into our target texture
            photoCaptureFrame.UploadImageDataToTexture(targetTexture);
            // Do as we wish with the texture such as apply it to a material, etc.
            captureMaterial.mainTexture = targetTexture;
        }
        else
        {
            cubeMaterial.color = new Color(1f, 0f, 0f);
        }
        // Clean up
        photoCaptureObject.StopPhotoModeAsync(OnStoppedPhotoMode);
    }

}
