using SoundSystemScripts;
using UnityEngine;
using UnityEngine.UI;

namespace Editor.Scripts.MainMenuScripts.UI
{
    public class PlaySoundButtonUI : ButtonBehaviourUI
    {
        [SerializeField] private TypeOfSFXByItsNature _soundtrack;

           private void Awake()
       {
           _button = GetComponent<Button>();
       }

           private void Start()
           {
               _button.onClick.AddListener(PlayClickSoundtrack);
               _button.onClick.AddListener(PlayClickSoundtrack);
           }

           private void PlayClickSoundtrack()
           {
               SoundtrackPlayerWrapper.PlayUISoundtrack(_soundtrack);
           }
           
           

           private void OnDestroy()
           {
               _button.onClick.RemoveListener(PlayClickSoundtrack);
           }
    }
    
    
}