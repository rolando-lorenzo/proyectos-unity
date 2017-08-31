using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using Facebook.Unity;

public class FacebookManager : MonoBehaviour {

	private static FacebookManager _instance;
	private string urlImg;
	int score = 10000;

	private Rect windowRect = new Rect ((Screen.width - 300)/2, (Screen.height - 200)/2, 300, 200);
	private bool dialogShow = false;
	private string dialogMensage;
	private bool dialogBtnOk = false;

	public static FacebookManager Instance
	{
		get {
			if (_instance == null) {
				GameObject fbm = new GameObject ("FBManager");
				fbm.AddComponent<FacebookManager> ();
			}

			return _instance;
		}
	}


	public bool IsLoggedIn { get; set; }
	public string ProfileName { get; set; }
	public Sprite ProfilePic { get; set; }
	public string AppLinkURL { get; set; }
	public Image FotoPerfil { get; set;}

	void Awake()
	{
		DontDestroyOnLoad (this.gameObject);
		_instance = this;

	}

	public void InitFB()
	{
		if (!FB.IsInitialized) {
			FB.Init (this.SetInit, this.OnHideUnity);
		} else {
			IsLoggedIn = FB.IsLoggedIn;
		}
	}

	void SetInit()
	{

		if (FB.IsLoggedIn) {
			Debug.Log ("Se ha logueado a facebook correctamente");
			ObtenerPerfil ();
		} else {
			Debug.Log ("Ocurrio un error al intentar loguearse");
		}

		IsLoggedIn = FB.IsLoggedIn;

	}

	void OnHideUnity(bool isGameShown)
	{

		if (!isGameShown) {
			Time.timeScale = 0;
		} else {
			Time.timeScale = 1;
		}

	}

	public void ObtenerPerfil()
	{
		FB.API ("/me?fields=first_name", HttpMethod.GET, MostrarUsuarioLogueado);
		FB.API ("/me/picture?redirect=false", HttpMethod.GET, MostrarImgPerfil);
		FB.GetAppLink (DealWithAppLink);
	}

	private void MostrarUsuarioLogueado(IResult result)
	{
		if (String.IsNullOrEmpty(result.Error)) {
			ProfileName = "" + result.ResultDictionary ["first_name"];
		} else {
			Debug.Log (result.Error);
		}
	}

	private void MostrarImgPerfil(IGraphResult result)
	{
		Dictionary<string,object> dic = result.ResultDictionary ["data"] as Dictionary<string,object>;
		foreach (string key in dic.Keys) {
			Debug.Log(key + " : " + dic[key].ToString());
			if (key == "url") {
				urlImg = dic [key].ToString ();
				StartCoroutine ("ObtenerImgPerfil");
			}
		}

	}


	IEnumerator ObtenerImgPerfil()
	{

		WWW www = new WWW(urlImg);
		yield return www;
		ProfilePic = Sprite.Create (www.texture, new Rect (0, 0, 50, 50), new Vector2 ());

	}

	void DealWithAppLink(IAppLinkResult result)
	{
		
		if (!String.IsNullOrEmpty (result.Url)) {
			AppLinkURL = "" + result.Url + "";
			Debug.Log (AppLinkURL);
		}else {
			AppLinkURL = "http://google.com";
		}
	}


	public void Compartir()
	{
		FB.FeedShare (
			string.Empty,
			new Uri(AppLinkURL),
			"Hola este es el titulo",
			"Subtitutlo",
			"Revisa esto...",
			new Uri("http://iluminart.com.mx/img/portafolio-img/pic3.jpg"),
			string.Empty,
			CompartirCallback
		);
	}

	private void CompartirCallback(IResult result)
	{
		if (result.Cancelled) {
			Debug.Log ("Compartir cancelada");
		} else if (!string.IsNullOrEmpty (result.Error)) {
			Debug.Log ("Error al Compartir!");
		} else if (!string.IsNullOrEmpty (result.RawResult)) {
			Debug.Log ("Compartir Correcto!!!");
		}
	}

	public void InvitarAmigos()
	{
		FB.Mobile.AppInvite (
			new Uri(AppLinkURL),
			new Uri("http://iluminart.com.mx/img/portafolio-img/pic3.jpg"),
			InvitarAmigosCallback
		);
	}

	void InvitarAmigosCallback(IResult result)
	{
		if (result.Cancelled) {
			Debug.Log ("Invitacion cancelada");
		} else if (!string.IsNullOrEmpty (result.Error)) {
			Debug.Log ("Error al invitar!");
		} else if (!string.IsNullOrEmpty (result.RawResult)) {
			Debug.Log ("Exito al invitar");
		}
	}

	public void CompartirConAppUsuarios()
	{

		FB.AppRequest (
			"Texto a compartir...",
			null,
			new List<object> (){ "app_users" },
			null,
			null,
			null,
			null,
			CompartirConAppUsuariosCallback
		);

	}

	private void CompartirConAppUsuariosCallback(IAppRequestResult result)
	{
		if (result.Cancelled) {
			Debug.Log ("Desafio cancelado");
		} else if (!string.IsNullOrEmpty (result.Error)) {
			Debug.Log ("Error en el desafio!");
		} else if (!string.IsNullOrEmpty (result.RawResult)) {
			Debug.Log ("Exito en el desafio");
		}
	}

	public void QueryScores()
	{
		var scoreData = 
			new Dictionary<string, string>() {{"score", score.ToString()}};
		FB.API ("/me/scores", HttpMethod.POST, QueryScoresCallback, scoreData);
		Debug.Log (scoreData);
	}

	private void QueryScoresCallback(IGraphResult result)
	{
		Debug.Log (result);

	}

	private IEnumerator TakeScreenshot() 
	{
		yield return new WaitForEndOfFrame();

		var width = Screen.width;
		var height = Screen.height;
		var tex = new Texture2D(width, height, TextureFormat.RGB24, false);
		// Read screen contents into the texture
		tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
		tex.Apply();
		byte[] screenshot = tex.EncodeToPNG();

		var wwwForm = new WWWForm();
		wwwForm.AddBinaryData("image", screenshot, "Screenshot.png");

		FB.API("me/photos", HttpMethod.POST, QueryScoresCallback, wwwForm);
	}


	void OnGUI () 
	{
		if(dialogShow)
			windowRect = GUI.Window (0, windowRect, DialogWindow, dialogMensage);
	}

	void DialogWindow (int windowID)
	{
		if(GUI.Button(new Rect(5,100, windowRect.width - 10, 20), "Aceptar"))
		{
			dialogShow = false;
		}
	}

	public void MostrarDialogo(string mensaje, bool btnOk)
	{
		dialogMensage = mensaje;
		dialogBtnOk = btnOk;
		dialogShow = true;
	}
}
