using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
using UnityEngine.Serialization;
namespace Units{
	public class Unit : MonoBehaviour {
		
		public enum AnimStates
		{
			Attack,
			Die,
			Move
		}

		public int MaxUnitHP = 100;
		public int UnitHP = 100;
		public int UnitArmor = 0;
		public int UnitDamage = 10;
		public int UnitPersistentDamage = 10;
		public int UnitCommands = 0;
		[Range(0.1f, 2f)]
		public float AttackSpeed = 1;
		[Range(0.1f, 3f)]
		public float WalkSpeed = 0.2f;

		public Animator Animator;
		public GameObject TakeDamageEffect;
		public UnitTriggers UnitTriggers;

		public Dictionary<AnimStates, string> Animations;

		public bool Invulnerable;

		public MovePoint TargetMovePoint;

		void Awake(){
			if (Animator == null)
				Animator = GetComponent<Animator> ();
			
			if (UnitTriggers == null)
				UnitTriggers = GetComponent<UnitTriggers> ();
			
			if (TakeDamageEffect == null)
				Debug.Log ("Take Damage Effects = null", this);
		}
			
		public virtual bool GetIfMayAttack(){
			return true;
		}

		public virtual void Damage(Unit causer){
			if (Invulnerable)
				return;

			if (this.UnitArmor >= causer.UnitDamage) {
				this.UnitHP = this.UnitHP - UnitPersistentDamage;
			} else {
				this.UnitHP = this.UnitHP - ((causer.UnitDamage - this.UnitArmor) + UnitPersistentDamage);
			}

			if (CheckDeadState ()) {
				DeathReaction ();
			} else {
				DamageReaction ();
			}
		}
		public virtual void Death()
		{
			
		}
		public virtual void OnDamage (){
		}
		public virtual void OnDeath (){
		}

		private bool ifDeath;
		void DeathReaction(){
			if(ifDeath)
				return;
			
			ifDeath = true;
			this.m_OnDeath.Invoke ();
			OnDeath ();
			Death ();

		}

			
		void DamageReaction(){
			this.m_OnDamage.Invoke ();
			if(TakeDamageEffect != null)
			GameObject.Instantiate (TakeDamageEffect, this.transform.position, Quaternion.identity);//тут вообще pull из объектного пула
			OnDamage ();
		}
			
		bool CheckDeadState()
		{
			if (this.UnitHP <= 0)
				return true;
			return false;
		}

		[Serializable]
		public class DamageEvent: UnityEvent
		{}

		[FormerlySerializedAs("OnDamage"), SerializeField]
		private Unit.DamageEvent m_OnDamage = new Unit.DamageEvent ();

		public Unit.DamageEvent OnRecieveDamage
		{
			get 
			{
				return this.m_OnDamage; 
			} 
			set 
			{
				this.m_OnDamage = value; 
			}
		}

		[Serializable]
		public class HealEvent: UnityEvent
		{}

		[FormerlySerializedAs("OnHeal"), SerializeField]
		private Unit.HealEvent m_OnHeal = new Unit.HealEvent ();

		public Unit.HealEvent OnRecieveHeal
		{
			get 
			{
				return this.m_OnHeal; 
			} 
			set 
			{
				this.m_OnHeal = value; 
			}
		}

		[Serializable]
		public class DeathEvent: UnityEvent
		{}

		[FormerlySerializedAs("OnDeath"), SerializeField]
		private Unit.DeathEvent m_OnDeath = new Unit.DeathEvent ();

		public Unit.DeathEvent OnRecieveDeath
		{
			get 
			{
				return this.m_OnDeath; 
			} 
			set 
			{
				this.m_OnDeath = value; 
			}
		}
	}
}