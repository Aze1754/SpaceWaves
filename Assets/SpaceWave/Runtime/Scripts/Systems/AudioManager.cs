using UnityEngine;
public class AudioManager : MonoBehaviour {
  [SerializeField] AudioSource music, sfx;
  void Start(){ if(!music) music=gameObject.AddComponent<AudioSource>(); if(!sfx) sfx=gameObject.AddComponent<AudioSource>();
    music.loop=true; music.clip=CreateLoop(); music.volume=0.35f; music.Play(); }
  public void PlayClick()=>sfx.PlayOneShot(CreateClick(0.1f));
  public void PlayCoin()=>sfx.PlayOneShot(CreateClick(0.2f));
  public void PlayHit()=>sfx.PlayOneShot(CreateClick(0.05f));
  AudioClip CreateLoop(){ int sr=48000, samples=sr*2; float[] d=new float[samples]; float f1=220f,f2=277.18f,f3=329.63f;
    for(int i=0;i<samples;i++){ float t=i/(float)sr; float env=0.5f*(1f-Mathf.Cos(2f*Mathf.PI*t/2f));
      d[i]=0.2f*env*(Mathf.Sin(2*Mathf.PI*f1*t)+0.6f*Mathf.Sin(2*Mathf.PI*f2*t)+0.4f*Mathf.Sin(2*Mathf.PI*f3*t)); }
    var clip=AudioClip.Create("Loop",samples,1,sr,false); clip.SetData(d,0); return clip; }
  AudioClip CreateClick(float dur){ int sr=48000, samples=Mathf.CeilToInt(sr*dur); float[] d=new float[samples];
    for(int i=0;i<samples;i++){ float t=i/(float)sr; float env=Mathf.Exp(-t*30f); d[i]=env*(Random.value*2f-1f)*0.4f; }
    var clip=AudioClip.Create("Click",samples,1,sr,false); clip.SetData(d,0); return clip; }
}
