using UnityEngine;
using HoloToolkit.Sharing;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.VR.WSA.WebCam;
using HoloToolkit.Unity;

public class PictureManager : Singleton<PictureManager> {

    public DrawCanvas Picture;
    public Material CaptureMaterial;

    // Use this for initialization
    void Start ()
    {
        CustomMessages.Instance.MessageHandlers[CustomMessages.TestMessageID.Image] = this.OnReceiveImage;
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void SetPicture(Texture2D targetTexture)
    {
        Picture.GetComponent<MeshRenderer>().enabled = true;
        CaptureMaterial.mainTexture = targetTexture;
    }

    private void OnReceiveImage(NetworkInMessage msg)
    {
        Debug.Log("Received image.");

        // Eat the ID section
        long userID = msg.ReadInt64();

        var list = new List<byte>();
        while (msg.GetUnreadBitsCount() > 0)
        {
            list.Add(msg.ReadByte());
        }

        // Create our Texture2D for use and set the correct resolution
        Resolution cameraResolution = PhotoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).First();
        Texture2D targetTexture = new Texture2D(cameraResolution.width, cameraResolution.height);
        // Copy the raw image data into our target texture
        targetTexture.LoadRawTextureData(list.ToArray());
        targetTexture.Apply();
        // Do as we wish with the texture such as apply it to a material, etc.
        CaptureMaterial.mainTexture = targetTexture;
    }
}
