using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

namespace BVA.Sample
{
    public class AnimationPanel : MonoBehaviour
    {
        public Transform content;
        List<Button> buttons;
        new Animation animation;
        public List<AnimationClip> clips;
        PlayableGraph playableGraph;

        void Awake()
        {
            //SetAnimationClips(clips);
        }
        public void Set(AssetManager manager,Animation _animation)
        {
            animation = _animation;
            if (manager.assetCache.AnimationCache.Length == 0)
            {
                gameObject.SetActive(false);
                return;
            }
            clips = new List<AnimationClip>();
            foreach (var clip in manager.assetCache.AnimationCache)
            {
                if (clip != null)
                    clips.Add(clip.LoadedAnimationClip);
            }
            SetAnimationClips(clips);
        }
        public void SetAnimationClips(List<AnimationClip> _clips)
        {
            clips = _clips;
            buttons = new List<Button>();
            var element = content.GetChild(0);
            Text text = element.GetChild(0).GetComponent<Text>();
            Button button = element.GetChild(1).GetComponent<Button>();
            buttons.Add(button);
            for (int i = 0; i < clips.Count; i++)
            {
                if (i > 0)
                {
                    element = GameObject.Instantiate(element);
                    element.name = clips[i].name;
                    element.SetParent(content, false);
                    text = element.GetChild(0).GetComponent<Text>();
                    button = element.GetChild(1).GetComponent<Button>();
                    buttons.Add(button);
                }

                text.text = clips[i].name;
            }

            for (int i = 0; i < buttons.Count; i++)
            {
                var clip = clips[i];
                buttons[i].onClick.AddListener(() =>
                {
                    animation.Play(clip.name);
                });
            }
        }
    }
}