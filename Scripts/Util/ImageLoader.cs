using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;


namespace Xsolla {
	public class ImageLoader : MonoBehaviour {
		//This is the method you call if you use the "OnClick" event from a button click.
		//We cannot call the Coroutine function below, because it breaks the rule for
		//the event system. i.e. Must be a return type of void.
		public string imageUrl;
		public static Dictionary<string, Sprite> imageCashe;
		private static ImageLoader instance;

		void Awake()
		{
			if(imageCashe == null)
				imageCashe = new Dictionary<string, Sprite> ();	
		}

		public void Start()
		{
			if (imageUrl != null && !imageUrl.Equals(""))
				UploadImageToCurrentView (imageUrl);
		}

		public void LoadImage (Image imageView, string url) {
			imageView.enabled = false;
			//Call the actual loading method
			StartCoroutine(RealLoadImage(url, imageView));
		}


		public void UploadImageToCurrentView(string imageUrl){
			StartCoroutine(RealLoadImage(imageUrl, GetComponent<Image>()));
		}
		
		//This is where the actual code is executed
		//A URL where the image is stored
		private IEnumerator RealLoadImage (string url, UnityEngine.UI.Image imageView) {
			if (imageCashe.ContainsKey (url)) {
				if (imageView != null) {
					imageView.sprite = imageCashe[url]; 
				}
			} else {
				//Call the WWW class constructor
				WWW imageURLWWW = new WWW (url);  
			
				//Wait for the download
				yield return imageURLWWW;	

				//Simple check to see if there's indeed a texture available
				if(imageURLWWW.error == null){
					if (imageURLWWW.texture != null) {
					
						//Construct a new Sprite
						Sprite sprite = new Sprite ();     
					
						Texture2D image = new Texture2D(imageURLWWW.texture.width, imageURLWWW.texture.height, TextureFormat.RGBA32, true);
						imageURLWWW.LoadImageIntoTexture(image);
						image.SetPixels( image.GetPixels(0,0,image.width,image.height));
						image.Apply();
						image.filterMode = FilterMode.Trilinear;
						sprite = Sprite.Create(image, new Rect (0, 0, image.width, image.height), new Vector2(0,0));

						if(!imageCashe.ContainsKey(url))
						{
							imageCashe.Add (url, sprite);
						}

						//Assign the sprite to the Image Component
						if (imageView != null) 
						{
							imageView.sprite = sprite; 
						}
					}
				}
				else
				{
					Logger.LogError("Error to load img with url - " + url);
					// делаем image прозрачным 
					imageView.color = new Color(255,255,255,0);
				}
				imageURLWWW.Dispose ();
				imageURLWWW = null;
			}
			imageView.enabled = true;
			yield return null;
		}
	}
}
