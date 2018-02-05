using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Units{
	public class BattleUnit : Unit {
		bool AttackInDelay;

		public delegate void D_IfAttacking();
		public event D_IfAttacking E_IfAttacking;

		void Update()
		{
			if (UnitTriggers.EnemyUnitInRange.Count > 0 & AttackInDelay == false & GetIfMayAttack()) {
				AttackNextUnit ();
			}
		}

		public override void OnDeath ()
		{
			StartCoroutine (IE_Death());
		}
				
		public void Attack(Unit otherUnit){
			bool checkFortress = otherUnit is Fortress;
			if (this is Enemy & checkFortress == false)
				return;

			StopAllCourutines ();

			StartCoroutine (IE_AttakingRoutine ());

			if (otherUnit == null)
				return;
			if(E_IfAttacking != null)
				E_IfAttacking.Invoke ();
			
			otherUnit.Damage (this);
			
		}

		public void StopAllCourutines()
		{
			foreach (var i in Animations.Values) {
				StopCoroutine(i);
			}
		}

		public void AttackNextUnit()
		{
			AttackInDelay = true;
			if (UnitTriggers.EnemyUnitInRange.Count <= 0)

			if (UnitTriggers == null)
				return;
			if (UnitTriggers.EnemyUnitInRange.Count <= 0)
				return;

			Attack (UnitTriggers.EnemyUnitInRange [0]);
		}

		public void MoveToNextMovePoint()
		{
			if (TargetMovePoint == null)
				return;
			MoveTo (TargetMovePoint.gameObject);
		}

		void MoveTo(GameObject position)
		{
			StartCoroutine( IE_MovingRoutine (position));
		}

		protected IEnumerator IE_MovingRoutine(GameObject position)
		{
			if (!Animations.ContainsKey(AnimStates.Move)) {
				Debug.Log ("Animation name null", this);
			}

			string animName = "";
			Animations.TryGetValue (AnimStates.Move, out animName);

			Animator.Play (animName);

			Vector3 FirstMovePos = gameObject.transform.position;
			float startTime = Time.time;

			while (Vector3.Distance(gameObject.transform.position, position.transform.position) > 0.5) {
				gameObject.transform.position = Vector3.Lerp (FirstMovePos, position.transform.position, (WalkSpeed/2)*(Time.time -startTime));
				yield return new WaitForEndOfFrame ();
			}

			TargetMovePoint = TargetMovePoint.NextPoint;

			MoveToNextMovePoint ();
		}
			
		protected IEnumerator IE_AttakingRoutine(){
			if (!Animations.ContainsKey(AnimStates.Attack)) {
				Debug.Log ("Animation name null", this);
				yield break;
			}

			string animName = "";
			Animations.TryGetValue (AnimStates.Attack, out animName);

			if (UnitTriggers.EnemyUnitInRange.Count <= 0)
				yield break;

			var enemy = UnitTriggers.EnemyUnitInRange [0];

			if (enemy.UnitHP <= 0)
				yield break;
			

			Animator.Play (animName);
			yield return new WaitForSeconds (0.01f);

			int animHash = Animator.StringToHash (string.Format("Base Layer.{0}",animName));
			while (Animator.GetCurrentAnimatorStateInfo (0).fullPathHash == animHash) {
				yield return new WaitForEndOfFrame ();
			}

			yield return new WaitForSeconds (AttackSpeed);
			AttackInDelay = false;
		}

		protected IEnumerator IE_Death()
		{
			if (!Animations.ContainsKey(AnimStates.Die)) {
				Debug.Log ("Animation name null", this);
				yield break;
			}

			string animName = "";
			Animations.TryGetValue (AnimStates.Die, out animName);

			Animator.Play (animName);
			yield return new WaitForSeconds (0.01f);
			int animHash = Animator.StringToHash (string.Format("Base Layer.{0}",animName));
			while (Animator.GetCurrentAnimatorStateInfo (0).fullPathHash == animHash) {
				yield return new WaitForEndOfFrame ();
			}

			GameObject.Destroy (this.gameObject);
		}
	}
}
