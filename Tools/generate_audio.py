"""Deterministic original audio synthesis for LRH. Requires numpy and ffmpeg."""
from pathlib import Path
import numpy as np
import subprocess, wave
RATE=22050
OUT=Path(__file__).resolve().parents[1]/'Assets/_LRH/Resources/Audio'
OUT.mkdir(parents=True,exist_ok=True)
rng=np.random.default_rng(1879)

def noise(n, smooth=1):
    raw=rng.normal(0,1,n)
    if smooth>1: raw=np.convolve(raw,np.ones(smooth)/smooth,mode='same')
    return raw

def write(name,data,loop=False):
    data=np.asarray(data)
    # Leave headroom; avoid zero-length and non-finite artifacts.
    data=np.nan_to_num(data)
    data=np.tanh(data)
    peak=np.max(np.abs(data))
    if peak>0:data=data*(.72/peak)
    if not loop:
        n=min(int(RATE*.012),len(data)//8)
        fade=np.linspace(0,1,n)
        if data.ndim==2:fade=fade[:,None]
        data[:n]*=fade;data[-n:]*=fade[::-1]
    path=OUT/(name+'.wav')
    with wave.open(str(path),'wb') as f:
        f.setnchannels(1 if data.ndim==1 else data.shape[1]);f.setsampwidth(2);f.setframerate(RATE)
        f.writeframes((data*32767).astype('<i2').tobytes())
    if loop:
        subprocess.run(['ffmpeg','-hide_banner','-loglevel','error','-y','-i',str(path),'-c:a','libvorbis','-q:a','5',str(OUT/(name+'.ogg'))],check=True)
        path.unlink()

def effect(kind):
    duration={'cannon':1.35,'impact':.7,'explosion':2.1,'splash':1.1,'ui_click':.13}[kind]
    t=np.arange(int(RATE*duration))/RATE
    if kind=='cannon':
        phase=2*np.pi*(75*t-18*t*t)
        return 1.1*np.sin(phase)*np.exp(-t*5)+noise(len(t),24)*3*np.exp(-t*4)+noise(len(t),2)*.4*np.exp(-t*70)
    if kind=='impact':
        return noise(len(t),3)*.4*np.exp(-t*16)+sum(np.sin(2*np.pi*f*t)*np.exp(-t*d)*a for f,d,a in [(310,12,.4),(837,9,.2),(1423,16,.13)])
    if kind=='explosion':
        return noise(len(t),40)*5*np.exp(-t*2.8)+np.sin(2*np.pi*(56*t-7*t*t))*.7*np.exp(-t*3)+noise(len(t))*np.exp(-t*30)*.2
    if kind=='splash':
        return noise(len(t),6)*2*np.exp(-t*4)*(1-np.exp(-t*55))+np.sin(2*np.pi*(600*t-170*t*t))*.12*np.exp(-t*8)
    return (np.sin(2*np.pi*880*t)+.3*np.sin(2*np.pi*1320*t))*np.exp(-t*45)

for name in ['cannon','impact','explosion','splash','ui_click']:write(name,effect(name))

def instrument(midi,duration,style='horn'):
    t=np.arange(max(1,int(duration*RATE)))/RATE
    f=440*2**((midi-69)/12)
    attack=.18 if style=='pad' else .035
    env=np.minimum(t/attack,1)*np.minimum((duration-t)/(.5 if style=='pad' else .18),1)
    env=np.maximum(env,0)
    if style=='pad':
        signal=sum(np.sin(2*np.pi*f*ratio*t+np.sin(2*np.pi*.4*t)*.06)*amp for ratio,amp in [(1,.45),(1.003,.3),(2,.13),(3,.04)])
    elif style=='pluck':
        signal=(np.sin(2*np.pi*f*t)+.25*np.sin(2*np.pi*2*f*t))*np.exp(-t*4)
    else:
        signal=sum(np.sin(2*np.pi*f*k*t+np.sin(2*np.pi*4.5*t)*.018)*a for k,a in [(1,.65),(2,.23),(3,.1),(4,.035)])
    return signal*env

def add(buffer,signal,start,gain=1,pan=0):
    # Wrap tails into beginning: reverbs and sustained voices preserve the loop boundary.
    pos=int(start*RATE)%len(buffer)
    stereo=np.column_stack((signal*np.sqrt((1-pan)/2),signal*np.sqrt((1+pan)/2)))*gain
    for offset in range(0,len(stereo),len(buffer)):
        section=stereo[offset:offset+len(buffer)]
        count=min(len(section),len(buffer)-pos)
        buffer[pos:pos+count]+=section[:count]
        if count<len(section):buffer[:len(section)-count]+=section[count:]

def theme(name,combat=False):
    beat=60/(92 if combat else 72)
    bar=beat*4
    chords=[[50,57,62,65],[46,53,58,62],[41,53,57,60],[48,55,60,64],
            [43,55,58,62],[46,53,58,62],[45,57,61,64],[50,57,62,65]]*2
    n=int(16*bar*RATE)
    music=np.zeros((n,2),dtype=np.float64)
    melody=[[74,77],[76,72],[69,72],[74,76],[77,79],[77,74],[73,69],[74,69]]*2
    for i,chord in enumerate(chords):
        start=i*bar
        for k,note in enumerate(chord):
            add(music,instrument(note,bar+.35,'pad'),start,.105,(-.55+k*.35))
        add(music,instrument(chord[0]-12,bar*.94,'horn'),start,.16)
        for j,note in enumerate(melody[i]):
            add(music,instrument(note,beat*1.7,'horn'),start+j*2*beat,.14 if combat else .12,.16)
        for step in range(8):
            add(music,instrument(chord[step%4]+12,beat*.7,'pluck'),start+step*beat/2,.10 if combat else .05,-.3)
        for beat_index in ([0,1,2,3] if combat else [0,2]):
            t=np.arange(int(.36*RATE))/RATE
            drum=np.sin(2*np.pi*(78*t-55*t*t))*np.exp(-t*13)+noise(len(t),18)*.3*np.exp(-t*20)
            add(music,drum,start+beat_index*beat,.12 if combat else .065,-.1)
    dry=music.copy()
    for delay,gain in [(.19,.14),(.37,.10),(.59,.07)]:
        music+=np.roll(dry,int(delay*RATE),axis=0)[:,::-1]*gain
    # A short periodic crossfade removes any sample discontinuity at export boundary.
    edge=int(.035*RATE)
    bridge=np.linspace(music[-edge],music[edge],edge*2)
    music[-edge:]=bridge[:edge];music[:edge]=bridge[edge:]
    write(name,music,True)

theme('menu_theme');theme('battle_theme',True)
for name,notes in [('victory',[62,65,69,74]),('defeat',[62,58,57,50])]:
    buf=np.zeros((RATE*4,2))
    for i,note in enumerate(notes):add(buf,instrument(note,1.2,'horn'),i*.6,.35)
    write(name,buf)
# Ocean ambience: filtered periodic noise with slow swells, no recorded material.
n=RATE*12
freq=np.fft.rfftfreq(n,1/RATE)
phase=rng.uniform(0,2*np.pi,len(freq))
spectrum=np.exp(1j*phase)/np.maximum(freq,70)**.65
spectrum[freq<90]=0;spectrum[freq>4500]=0
sea=np.fft.irfft(spectrum,n);sea/=max(np.max(np.abs(sea)),1e-8)
t=np.arange(n)/RATE;sea*=.5+.25*np.sin(2*np.pi*t/6)+.15*np.sin(2*np.pi*t/4)
write('ocean_ambience',np.column_stack((sea,np.roll(sea,RATE//3))),True)
print('Generated',len(list(OUT.glob('*.wav'))),'effects and',len(list(OUT.glob('*.ogg'))),'loops')
