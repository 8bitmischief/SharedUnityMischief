using System;
using UnityEngine;

namespace SharedUnityMischief {
	[Serializable]
	public class Curve {
		private static int NUM_REVERSE_LOOKUP_SAMPLES = 1001;

		[SerializeField] private AnimationCurve _animationCurve;

		private AnimationCurve reverseLookupAnimationCurve;

		public AnimationCurve animationCurve {
			get => _animationCurve;
			private set => _animationCurve = value;
		}

		public Curve (AnimationCurve animationCurve) {
			this.animationCurve = animationCurve;
		}

		public Curve (params Keyframe[] keys) {
			animationCurve = new AnimationCurve(keys);
		}

		public float Evaluate (float x) => animationCurve.Evaluate(x);

		public float EvaluateReverseLookup (float value) {
			if (reverseLookupAnimationCurve == null) {
				Keyframe[] reverseLookupValues = new Keyframe[NUM_REVERSE_LOOKUP_SAMPLES];
				for (int i = 0; i < NUM_REVERSE_LOOKUP_SAMPLES; i++) {
					float x = ((float) i) / ((float) (NUM_REVERSE_LOOKUP_SAMPLES - 1));
					float y = Evaluate(x);
					reverseLookupValues[i] = new Keyframe(y, x);
				}
				reverseLookupAnimationCurve = new AnimationCurve(reverseLookupValues);
			}
			return reverseLookupAnimationCurve.Evaluate(value);
		}

		public static Curve Constant (float value = 1f) => Constant(0f, 1f, value);
		public static Curve Constant (float xStart, float xEnd, float value) => new Curve(AnimationCurve.Constant(xStart, xEnd, value));
		public static Curve EaseInOut () => EaseInOut(0f, 1f);
		public static Curve EaseInOut (float yStart, float yEnd) => EaseInOut(0f, yStart, 1f, yEnd);
		public static Curve EaseInOut (float xStart, float yStart, float xEnd, float yEnd) => new Curve(AnimationCurve.EaseInOut(xStart, yStart, xEnd, yEnd));
		public static Curve Linear () => Linear(0f, 1f);
		public static Curve Linear (float yStart, float yEnd) => Linear(0f, yStart, 1f, yEnd);
		public static Curve Linear (float xStart, float yStart, float xEnd, float yEnd) => new Curve(AnimationCurve.Linear(xStart, yStart, xEnd, yEnd));
	}
}