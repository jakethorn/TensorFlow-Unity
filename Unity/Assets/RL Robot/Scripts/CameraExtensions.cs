using UnityEngine;

namespace Jake.CameraExtensions
{
	using RenderTextureExtensions;
	
	/// <summary>
	/// Texture format.
	/// </summary>
	public enum Format { EXR, JPG, PNG };

	/// <summary>
	/// Camera extension methods.
	/// </summary>
	public static class CameraExtensions
	{
		/// <summary>
		/// Instantly render camera and return contents as a Texture2D.
		/// Remember to Object.Destroy the Texture2D when you're finished with it.
		/// </summary>
		public static Texture2D Screenshot(this Camera camera)
		{
			// swap target texture
			var prevTarget = camera.targetTexture;
			camera.targetTexture = new RenderTexture(camera.pixelWidth, camera.pixelHeight, 32);

			// render to texture2d
			camera.Render();
			var screenshot = camera.targetTexture.ToTexture2D();

			// restore target texture
			Object.Destroy(camera.targetTexture);
			camera.targetTexture = prevTarget;

			return screenshot;
		}

		/// <summary>
		/// Instantly render camera and return contents as a byte array in the specific format (jpg, png or exr).
		/// </summary>
		public static byte[] ScreenshotAsBytes(this Camera camera, Format format)
		{
			var bytes = default(byte[]);
			var screenshot = camera.Screenshot();

			switch (format)
			{
				case Format.EXR:	bytes = screenshot.EncodeToEXR();	break;
				case Format.JPG:	bytes = screenshot.EncodeToJPG();	break;
				case Format.PNG:	bytes = screenshot.EncodeToPNG();	break;
			}

			Object.Destroy(screenshot);
			return bytes;
		}
	}
}
