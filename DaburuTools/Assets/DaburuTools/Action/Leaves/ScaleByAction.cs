﻿using UnityEngine;
using System.Collections.Generic;

namespace DaburuTools
{
	namespace Action
	{
		public class ScaleByAction : Action
		{
			Transform mTransform;
			Vector3 mvecAccumulatedScale;
			Vector3 mvecDesiredScaleDelta;
			Vector3 mvecDeltaPerSecond;
			float mfActionDuration;
			float mfElaspedDuration;

			public ScaleByAction(Transform _transform)
			{
				mTransform = _transform;
				SetupScaleToAction();
			}

			public ScaleByAction(Transform _transform, Vector3 _desiredDelta, float _actionDuration)
			{
				mTransform = _transform;
				SetupScaleToAction();
				SetScaleToAction(_desiredDelta, _actionDuration);
			}

			public void SetScaleToAction(Vector3 _desiredDelta, float _actionDuration)
			{
				mvecDesiredScaleDelta = _desiredDelta - Vector3.one;
				mfActionDuration = _actionDuration;
				mvecDeltaPerSecond = mvecDesiredScaleDelta / mfActionDuration;	// Cache so don't need to calcualte every RunAction.
			}

			private void SetupScaleToAction()
			{
				mvecAccumulatedScale = Vector3.one;
				mfElaspedDuration = 0f;
			}

			private Vector3 CalcInverseAccumulatedScale()
			{
				Vector3 inverseAccumulatedScale = mTransform.localScale;
				inverseAccumulatedScale.x /= mvecAccumulatedScale.x;
				inverseAccumulatedScale.y /= mvecAccumulatedScale.y;
				inverseAccumulatedScale.z /= mvecAccumulatedScale.z;

				return inverseAccumulatedScale;
			}

			public override void RunAction()
			{
				base.RunAction();

				mfElaspedDuration += Time.deltaTime;

				Vector3 delta = mvecDeltaPerSecond * Time.deltaTime;
				mTransform.localScale = Vector3.Scale(CalcInverseAccumulatedScale(), mvecAccumulatedScale + delta);
				mvecAccumulatedScale += delta;


				// Remove self after action is finished.
				if (mfElaspedDuration > mfActionDuration)
				{
					Vector3 finalScaleVec = CalcInverseAccumulatedScale();
					finalScaleVec = Vector3.Scale(finalScaleVec, mvecDesiredScaleDelta + Vector3.one);
					mTransform.localScale = finalScaleVec;	// Force it to be the exact scale that it wants.

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