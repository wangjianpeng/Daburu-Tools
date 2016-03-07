﻿using UnityEngine;
using System.Collections.Generic;

namespace DaburuTools
{
	namespace Action
	{
		public class LocalRotateByAction : Action
		{
			Transform mTransform;
			Vector3 mvecAccumulatedDelta;
			Vector3 mvecDesiredTotalDelta;
			Vector3 mvecDeltaPerSecond;
			float mfActionDuration;
			float mfElaspedDuration;

			public LocalRotateByAction(Transform _transform)
			{
				mTransform = _transform;
				SetupMoveToAction();
			}

			public LocalRotateByAction(Transform _transform, Vector3 _desiredDelta, float _actionDuration)
			{
				mTransform = _transform;
				SetupMoveToAction();
				SetMoveToAction(_desiredDelta, _actionDuration);
			}

			public void SetMoveToAction(Vector3 _desiredDelta, float _actionDuration)
			{
				mvecDesiredTotalDelta = _desiredDelta;
				mfActionDuration = _actionDuration;
				mvecDeltaPerSecond = _desiredDelta / mfActionDuration;	// Cache so don't need to calcualte every RunAction.
			}

			private void SetupMoveToAction()
			{
				mvecAccumulatedDelta = Vector3.zero;
				mfElaspedDuration = 0f;
			}

			public override void RunAction()
			{
				base.RunAction();

				// It is less tricky to track the action by elasped time.
				// Otherwise, we need to check the sqrDist of both vec3s
				// for when we need to terminate the action.
				mfElaspedDuration += Time.deltaTime;

				Vector3 delta = mvecDeltaPerSecond * Time.deltaTime;
				mTransform.Rotate(delta, Space.Self);
				mvecAccumulatedDelta += delta;

				// Remove self after action is finished.
				if (mfElaspedDuration > mfActionDuration)
				{
					Vector3 imperfection = mvecDesiredTotalDelta - mvecAccumulatedDelta;
					mTransform.Rotate(imperfection, Space.Self);	// Force to exact delta displacement.

					OnActionEnd();
					mParent.Remove(this);
				}
			}

			public override bool Add(Action _Action)
			{
				return false;
			}
			public override bool Remove(Action _Action)
			{
				return false;
			}
			public override LinkedListNode<Action> GetListHead() { return null; }
		}
	}
}
