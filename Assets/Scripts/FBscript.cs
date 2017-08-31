using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Facebook.Unity;

public class FBscript : MonoBehaviour {

	public GameObject DialogLoggedIn;
	public GameObject DialogLoggedOut;
	public GameObject DialogUsername;
	public GameObject DialogProfilePic;

	void Awake()
	{

		FacebookManager.Instance.InitFB ();
		DealWithFBMenus (FB.IsLoggedIn);

	}


	public void FBlogin()
	{

		List<string> permissions = new List<string> ();
		permissions.Add ("public_profile");
		permissions.Add ("user_friends");
		permissions.Add ("email");
		FB.LogInWithReadPermissions (permissions, this.AuthCallBack);
	}

	void AuthCallBack(IResult result)
	{

		if (result.Error != null) {
			Debug.Log (result.Error);
		} else {
			if (FB.IsLoggedIn) {
				FacebookManager.Instance.IsLoggedIn = true;
				FacebookManager.Instance.ObtenerPerfil ();
				Debug.Log ("FB is logged in");
			} else {
				Debug.Log ("FB is not logged in");
			}

			DealWithFBMenus (FB.IsLoggedIn);
		}

	}

	void DealWithFBMenus(bool isLoggedIn)
	{

		if (isLoggedIn) {
			DialogLoggedIn.SetActive (true);
			DialogLoggedOut.SetActive (false);

			if (FacebookManager.Instance.ProfileName != null) {

				Text UserName = DialogUsername.GetComponent<Text> ();
				UserName.text = "Bienvenido, " + FacebookManager.Instance.ProfileName;

			} else {
				StartCoroutine ("WaitForProfileName");
			}


			if (FacebookManager.Instance.ProfilePic != null) {

				Image ProfilePic = DialogProfilePic.GetComponent<Image> ();
				ProfilePic.sprite = FacebookManager.Instance.ProfilePic;

			} else {
				StartCoroutine ("WaitForProfilePic");
			}

		} else {
			DialogLoggedIn.SetActive (false);
			DialogLoggedOut.SetActive (true);


		}

	}


	IEnumerator WaitForProfileName()
	{

		while (FacebookManager.Instance.ProfileName == null) {
			yield return null;
		}

		DealWithFBMenus (FB.IsLoggedIn);

	}

	IEnumerator WaitForProfilePic()
	{

		while (FacebookManager.Instance.ProfilePic == null) {
			yield return null;
		}

		DealWithFBMenus (FB.IsLoggedIn);

	}


	public void Share()
	{
		FacebookManager.Instance.Compartir ();
	}

	public void Invite()
	{
		FacebookManager.Instance.InvitarAmigos ();
	}

	public void ShareWithUsers()
	{
		FacebookManager.Instance.CompartirConAppUsuarios ();
	}

	public void SendQueryScores()
	{
		FacebookManager.Instance.QueryScores ();
	}
}
