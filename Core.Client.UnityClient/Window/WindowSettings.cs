using System.Collections.Generic;
using Core.Client.UnityClient.Utils.Serialization;
using UnityEngine;

namespace Core.Client.UnityClient.Window {
	public sealed class WindowSettings : MonoBehaviour {
		public GameObject      Content;
		public Animation       Animation;
		public AnimationClip   ShowAnimation;
		public AnimationClip   HideAnimation;
		public List<StrGoPair> Prefabs;
	}
}