using System;
using UnityEngine;

namespace LRH.Runtime
{
    public sealed class SoundFx : MonoBehaviour
    {
        private AudioSource effects, music, ocean, ui;
        private AudioClip cannon, impact, explosion, splash, click, victory, defeat, menuTheme, battleTheme;
        private AudioClip desiredMusic;
        private float musicLevel;
        private bool paused;
        private float lastCannon = -10, lastImpact = -10;
        public bool IsBattle { get; private set; }
        public AudioClip CurrentTheme => desiredMusic;

        private void Awake()
        {
            effects=Source("Efectos",.7f,false);ui=Source("Interfaz",.65f,false);
            music=Source("Música",0,true);ocean=Source("Océano",0,true);
            cannon=Clip("cannon");impact=Clip("impact");explosion=Clip("explosion");splash=Clip("splash");
            click=Clip("ui_click");victory=Clip("victory");defeat=Clip("defeat");
            menuTheme=Clip("menu_theme");battleTheme=Clip("battle_theme");
            ocean.clip=Clip("ocean_ambience");ocean.Play();
        }
        private AudioSource Source(string name,float volume,bool loop)
        {
            var source=new GameObject(name).AddComponent<AudioSource>();source.transform.SetParent(transform,false);
            source.playOnAwake=false;source.loop=loop;source.spatialBlend=0;source.volume=volume;
            return source;
        }
        private static AudioClip Clip(string name)
        {
            var clip=Resources.Load<AudioClip>("Audio/"+name);
            if(clip==null)throw new InvalidOperationException("Falta audio: "+name);
            return clip;
        }
        public void Menu()
        {
            IsBattle=false;paused=false;desiredMusic=menuTheme;musicLevel=.30f;
        }
        public void Battle()
        {
            IsBattle=true;paused=false;desiredMusic=battleTheme;musicLevel=.26f;
        }
        public void Pause(bool value) { paused=value; }
        public void Result(bool won)
        {
            IsBattle=false;paused=false;desiredMusic=null;musicLevel=0;
            ui.PlayOneShot(won?victory:defeat,.55f);
        }
        private void Update()
        {
            float step=Time.unscaledDeltaTime*.55f;
            if(music.clip!=desiredMusic)
            {
                music.volume=Mathf.MoveTowards(music.volume,0,step);
                if(music.volume<=.001f)
                {
                    music.Stop();music.clip=desiredMusic;
                    if(desiredMusic!=null)music.Play();
                }
            }
            else music.volume=Mathf.MoveTowards(music.volume,paused?musicLevel*.35f:musicLevel,step);
            ocean.volume=Mathf.MoveTowards(ocean.volume,IsBattle?(paused?.025f:.075f):.025f,step*.3f);
        }
        public void Fire()
        {
            if(Time.unscaledTime-lastCannon<.06f)return;
            lastCannon=Time.unscaledTime;effects.PlayOneShot(cannon,.48f);
        }
        public void Hit()
        {
            if(Time.unscaledTime-lastImpact<.04f)return;
            lastImpact=Time.unscaledTime;effects.PlayOneShot(impact,.35f);
        }
        public void Explosion() { effects.PlayOneShot(explosion,.5f); }
        public void Splash() { effects.PlayOneShot(splash,.32f); }
        public void Click() { ui.PlayOneShot(click,.22f); }
    }
}
