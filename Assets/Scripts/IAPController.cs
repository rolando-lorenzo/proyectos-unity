using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IAPController : MonoBehaviour {

	void Awake()
	{
		IAPManager.Instance.InitializePurchasing ();
	}

	public void Comprar_200Huellas()
	{
		IAPManager.Instance.Comprar_200Huellas ();		
	}

	public void Comprar_500Huellas()
	{
		IAPManager.Instance.Comprar_500Huellas ();
	}

	public void Comprar_1000Huellas()
	{
		IAPManager.Instance.Comprar_1000Huellas ();
	}
}
	
