using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Units.Ranged {
public class Archer : BattleUnit {
	public int MaxAmmoCount = 5;
	public int AmmoCount = 5;

	void Awake()
	{
		E_IfAttacking += IfWeAttacking;
	}

	public override bool GetIfMayAttack ()
	{
		if (AmmoCount <= 0)
			return false;
		return true;
	}

	void IfWeAttacking()
	{
		AmmoCount--;
		if (AmmoCount <= 0)
			m_OnAmmoEmpty.Invoke ();
	}

	public void FillAmmoCount(){
		AmmoCount = MaxAmmoCount;
		m_OnAmmoFilled.Invoke ();
	}


	[Serializable]
	public class OnAmmoEmpty: UnityEvent
	{}

	[FormerlySerializedAs("OnAmmoEmpty"), SerializeField]
	private Archer.OnAmmoEmpty m_OnAmmoEmpty = new Archer.OnAmmoEmpty ();

	public Archer.OnAmmoEmpty OnAmmoIsEmpty
	{
		get 
		{
			return this.m_OnAmmoEmpty; 
		} 
		set 
		{
			this.m_OnAmmoEmpty = value; 
		}
	}
	[Serializable]
	public class OnAmmoFilled: UnityEvent
	{}

	[FormerlySerializedAs("OnAmmoFilled"), SerializeField]
	private Archer.OnAmmoFilled m_OnAmmoFilled = new Archer.OnAmmoFilled ();

	public Archer.OnAmmoFilled OnAmmoFill
	{
		get 
		{
			return this.m_OnAmmoFilled; 
		} 
		set 
		{
			this.m_OnAmmoFilled = value; 
		}
	}

}
}