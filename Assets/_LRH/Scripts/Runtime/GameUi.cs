using System;
using System.Collections.Generic;
using LRH.Core;
using UnityEngine;
using UnityEngine.EventSystems;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem.UI;
#endif
using UnityEngine.UI;

namespace LRH.Runtime
{
    public sealed class GameUi : MonoBehaviour
    {
        private static readonly Color Navy = new Color(.015f, .03f, .052f, .985f);
        private static readonly Color Gold = new Color(.86f, .67f, .34f);
        private static readonly Color Cream = new Color(.92f, .94f, .91f);
        private static readonly Color Sea = new Color(.035f, .22f, .29f);
        private static readonly Color Signal = new Color(.82f, .25f, .22f);
        private static readonly Color PixelShadow = new Color(.003f, .008f, .014f, .9f);
        private static readonly Color Ink = new Color(.018f, .055f, .072f, .98f);
        private GameRoot root;
        private RectTransform canvasRect;
        private Font font;
        private Font titleFont;
        private Text healthLabel;
        private Text convoyLabel;
        private Text reportLabel;
        private Health playerHealth;
        private Health convoyHealth;
        private RectTransform minimapRect;
        private Text navigationArrow;
        private Text navigationLabel;
        private Image playerFill, convoyFill;
        private Text objectiveList, objectiveCount;
        private int hudMission;
        private IReadOnlyList<ShipActor> hudEnemies;
        private readonly List<string> objectiveNames=new List<string>();
        private readonly List<MapMarker> mapMarkers = new List<MapMarker>();
        private readonly Dictionary<Health, Action<int, int>> enemyObservers = new Dictionary<Health, Action<int, int>>();

        private sealed class MapMarker
        {
            public RectTransform Rect;
            public RectTransform Halo;
            public ShipActor Actor;
            public float Phase;
        }

        public void Initialize(GameRoot director)
        {
            root = director;
            font = Resources.Load<Font>("Fonts/Interface");
            if (font == null) font = Resources.Load<Font>("Fonts/Pixel");
            if (font == null) font = Font.CreateDynamicFontFromOSFont("Arial", 20);
            titleFont=font;
            var canvas = gameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.worldCamera=null;
            canvas.pixelPerfect=false;
            canvas.sortingOrder = 100;
            var scaler = gameObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
            scaler.matchWidthOrHeight = .5f;
            gameObject.AddComponent<GraphicRaycaster>();
            canvasRect = GetComponent<RectTransform>();
            var events = new GameObject("UI Event System");
            events.transform.SetParent(transform, false);
            events.AddComponent<EventSystem>();
#if ENABLE_INPUT_SYSTEM
            events.AddComponent<InputSystemUIInputModule>().AssignDefaultActions();
#else
            events.AddComponent<StandaloneInputModule>();
#endif
        }

        private void Clear()
        {
            if (playerHealth != null) playerHealth.Changed -= OnPlayerChanged;
            if (convoyHealth != null) convoyHealth.Changed -= OnConvoyChanged;
            foreach (var pair in enemyObservers) pair.Key.Changed -= pair.Value;
            enemyObservers.Clear();
            playerHealth = convoyHealth = null;
            healthLabel = convoyLabel = reportLabel = null;
            minimapRect = null;
            navigationArrow = navigationLabel = null;
            mapMarkers.Clear();
            playerFill = convoyFill = null;
            objectiveList = objectiveCount = null;
            hudEnemies = null;
            objectiveNames.Clear();
            // EventSystem survives screen changes; all other screen children are temporary.
            for (int i = canvasRect.childCount - 1; i >= 0; --i)
                if (canvasRect.GetChild(i).name != "UI Event System")
                {
                    canvasRect.GetChild(i).gameObject.SetActive(false);
                    Destroy(canvasRect.GetChild(i).gameObject);
                }
        }

        public void ShowMenu(Progress progress)
        {
            Clear();
            var background = Panel("Carta del Pacífico", Navy);
            var menuArt = Image(background, "Fondo de menú ilustrado", UiVisuals.Asset("UI/Brand/menu_background"));
            Stretch(menuArt.rectTransform);
            menuArt.color = Color.white;
            menuArt.preserveAspect = false;
            menuArt.raycastTarget = false;
            var title=Text(background,"LA RESURRECCIÓN\nDEL HUÁSCAR",52,new Vector2(-460,-190),new Vector2(760,200),Cream);
            title.font=titleFont;
            Text(background,"CAMPAÑA NAVAL · HISTORIA ALTERNATIVA",21,new Vector2(-460,-320),new Vector2(760,36),Gold);
            Button(background, "01 · Ecos del Pacífico", new Vector2(-460, -430), () => root.PreviewMission(1));
            Button(background, progress.CanPlay(2) ? "02 · Convoy bajo fuego" : "02 · Convoy bajo fuego (bloqueada)",
                new Vector2(-460, -535), () => root.PreviewMission(2), progress.CanPlay(2));
            Button(background, progress.CanPlay(3) ? "03 · Silencio en la costa" : "03 · Silencio en la costa (bloqueada)",
                new Vector2(-460, -640), () => root.PreviewMission(3), progress.CanPlay(3));
            Button(background, "Salir del juego", new Vector2(-625, -745), Quit, true, new Vector2(350, 76));
            Button(background, "Créditos", new Vector2(-235, -745), ShowCredits, true, new Vector2(270, 72));
            VolumeControl(background,progress.MasterVolume);
            Footer(background, "Obra de ficción inspirada en un contexto histórico. Estos eventos no sucedieron en la realidad.");
        }

        public void ShowIntro(int number)
        {
            Clear();
            var panel = Panel("Carta de misión", Navy);
            Text(panel, "MISIÓN 0" + number, 38, new Vector2(-380, -142), new Vector2(700, 70), Gold);
            string title = number == 1 ? "ECOS DEL PACÍFICO" : number == 2 ? "CONVOY BAJO FUEGO" : "SILENCIO EN LA COSTA";
            string story = number == 1
                ? "En una historia alternativa, una patrullera amenaza la ruta de suministros.\nIntercepta la nave antes de que cierre el paso."
                : number == 2
                    ? "Un convoy aliado cruza aguas hostiles.\nDefiéndelo hasta que alcance la salida oriental."
                    : "Dos baterías costeras impiden navegar.\nInutilízalas sin perder al Huáscar.";
            Text(panel, title, 67, new Vector2(-380, -252), new Vector2(780, 116), Cream, FontStyle.Bold);
            Text(panel, story, 31, new Vector2(-380, -415), new Vector2(760, 175), Cream);
            ChartCard(panel, new Vector2(560, -365), new Vector2(700, 420), "LITORAL DEL PACÍFICO  /  CARTA DE OPERACIONES");
            Button(panel, "Iniciar misión", new Vector2(-380, -650), () => root.StartMission(number), true, new Vector2(650, 82));
            Button(panel, "Volver al menú", new Vector2(-380, -765), root.ReturnMenu, true, new Vector2(650, 82));
            Footer(panel, "Esta obra es ficción; no representa sucesos históricos reales.");
        }

        public void ShowCredits()
        {
            Clear();
            var panel=Panel("Créditos",Navy);
            var card=HudCard("Equipo de desarrollo",(canvasRect.rect.width-1000)/2,150,1000,670);
            HudText(card,"LA RESURRECCIÓN DEL HUÁSCAR",34,40,32,920,60,Gold,true).alignment=TextAnchor.MiddleCenter;
            HudText(card,"TECNOLOGÍA DE OBJETOS · UNSA 2026-B",24,40,106,920,44,Cream).alignment=TextAnchor.MiddleCenter;
            HudText(card,"DESARROLLADORES",22,40,192,920,36,Gold,true).alignment=TextAnchor.MiddleCenter;
            string[] developers={"MauroSullcaMamani","SrEstrada","ChristianAlexYana","frix241"};
            for(int i=0;i<developers.Length;++i)
                HudText(card,developers[i],30,40,250+i*64,920,48,Cream,true).alignment=TextAnchor.MiddleCenter;
            Button(card,"Volver al menú",new Vector2(0,-580),root.ReturnMenu,true,new Vector2(600,64));
            Footer(panel,"Una historia alternativa de ficción.");
        }

        private void VolumeControl(RectTransform parent,float value)
        {
            var cardImage=Image(parent,"Ajuste de volumen",UiTheme.Surface); cardImage.type=UnityEngine.UI.Image.Type.Sliced;
            var card=cardImage.rectTransform;card.anchorMin=card.anchorMax=new Vector2(.5f,1);
            card.anchoredPosition=new Vector2(-460,-866);card.sizeDelta=new Vector2(760,132);
            HudText(card,"VOLUMEN GENERAL",23,24,22,480,32,Gold,true);
            var percentage=HudText(card,"",24,594,22,140,32,Cream,true);percentage.alignment=TextAnchor.MiddleRight;
            var go=new GameObject("Volumen",typeof(RectTransform),typeof(Slider));go.transform.SetParent(card,false);
            var rect=(RectTransform)go.transform;SetTopLeft(rect,24,74,712,32);
            var track=Image(rect,"Pista de volumen",UiTheme.Track);track.type=UnityEngine.UI.Image.Type.Sliced;Stretch(track.rectTransform);track.raycastTarget=true;
            var fill=Image(rect,"Nivel de volumen",UiTheme.Gold);fill.type=UnityEngine.UI.Image.Type.Sliced;
            Stretch(fill.rectTransform);fill.rectTransform.offsetMin=new Vector2(4,5);fill.rectTransform.offsetMax=new Vector2(-4,-5);
            var handleArea=new GameObject("Área del control",typeof(RectTransform));handleArea.transform.SetParent(rect,false);Stretch((RectTransform)handleArea.transform);
            ((RectTransform)handleArea.transform).offsetMin=new Vector2(12,0);((RectTransform)handleArea.transform).offsetMax=new Vector2(-12,0);
            var handle=Image(handleArea.transform,"Control de volumen",UiTheme.Knob);handle.type=UnityEngine.UI.Image.Type.Sliced;
            handle.rectTransform.sizeDelta=new Vector2(24,36);handle.raycastTarget=true;
            var slider=go.GetComponent<Slider>();slider.fillRect=fill.rectTransform;slider.handleRect=handle.rectTransform;
            slider.targetGraphic=handle;slider.minValue=0;slider.maxValue=1;slider.SetValueWithoutNotify(value);
            percentage.text=Mathf.RoundToInt(value*100)+" %";
            slider.onValueChanged.AddListener(v=>{percentage.text=Mathf.RoundToInt(v*100)+" %";root.SetVolume(v);});
        }

        public void ShowHud(int number, ShipActor playerActor, Health player, ShipActor convoyActor,
            Health convoy, IReadOnlyList<ShipActor> enemies)
        {
            Clear();
            Canvas.ForceUpdateCanvases();
            hudMission=number; hudEnemies=enemies;
            foreach(var enemy in enemies)objectiveNames.Add(enemy==null?"objetivo":ActorName(enemy));
            float vitalsHeight=convoy == null ? 180 : 280;
            var vitals=HudCard("Estado de la flota",24,24,480,vitalsHeight);
            HudText(vitals,"FLOTA ALIADA",26,28,22,424,38,Gold,true);
            playerFill=HealthBar(vitals,"Huáscar",player,28,70,424,UiTheme.Health,out healthLabel);
            playerHealth=player; playerHealth.Changed+=OnPlayerChanged;
            OnPlayerChanged(player.Current,player.Max);
            if(convoy != null)
            {
                convoyFill=HealthBar(vitals,"Convoy",convoy,28,170,424,UiTheme.Gold,out convoyLabel);
                convoyHealth=convoy; convoyHealth.Changed+=OnConvoyChanged;
                OnConvoyChanged(convoy.Current,convoy.Max);
            }
            float objectiveY=24+vitalsHeight+20;
            var objectives=HudCard("Objetivos de misión",24,objectiveY,480,254);
            HudText(objectives,"OBJETIVOS",26,28,22,424,38,Gold,true);
            objectiveCount=HudText(objectives,"",24,28,68,424,34,Cream);
            objectiveList=HudText(objectives,"",25,28,116,424,112,Cream);
            objectiveList.alignment=TextAnchor.UpperLeft;
            float contactsY=objectiveY+274;
            var contacts=HudCard("Contactos enemigos",24,contactsY,480,80+enemies.Count*96);
            HudText(contacts,"CONTACTOS ENEMIGOS",26,28,22,424,38,Gold,true);
            for(int i=0;i<enemies.Count;++i)
            {
                var actor=enemies[i]; if(actor==null)continue;
                Text label;
                var fill=HealthBar(contacts,ActorName(actor),actor.Health,28,76+i*96,424,UiTheme.Danger,out label);
                string title=ActorName(actor);
                Action<int,int> update=(current,max)=>
                {
                    label.text=title+"   "+current+" / "+max;
                    fill.fillAmount=current/(float)max;
                };
                actor.Health.Changed+=update; enemyObservers.Add(actor.Health,update);
                update(actor.Health.Current,actor.Health.Max);
            }
            var heading=HudCard("Misión actual",(canvasRect.rect.width-660)/2,24,660,104);
            string[] titles={"","ECOS DEL PACÍFICO","CONVOY BAJO FUEGO","SILENCIO EN LA COSTA"};
            HudText(heading,"MISIÓN 0"+number,20,24,12,612,28,Gold,true).alignment=TextAnchor.MiddleCenter;
            HudText(heading,titles[number],25,24,48,612,38,Cream,true).alignment=TextAnchor.MiddleCenter;
            minimapRect=ChartCard(canvasRect,Vector2.zero,new Vector2(320,266),"CARTA TÁCTICA");
            var mapCard=(RectTransform)minimapRect.parent.parent;
            mapCard.anchorMin=mapCard.anchorMax=new Vector2(1,1); mapCard.pivot=new Vector2(1,1);
            mapCard.anchoredPosition=new Vector2(-24,-106);
            AddMapMarker(playerActor,Gold);
            if(convoyActor != null)AddMapMarker(convoyActor,new Color(.45f,.85f,.78f));
            foreach(var enemy in enemies) if(enemy != null)AddMapMarker(enemy,new Color(.95f,.42f,.35f));
            var pause=Button(canvasRect,"II  Pausa",Vector2.zero,root.Pause,true,new Vector2(180,58));
            var pauseRect=(RectTransform)pause.transform;
            pauseRect.anchorMin=pauseRect.anchorMax=new Vector2(1,1); pauseRect.pivot=new Vector2(1,1);
            pauseRect.anchoredPosition=new Vector2(-24,-24);
            navigationArrow=Text(canvasRect,">",36,Vector2.zero,new Vector2(48,48),Gold,FontStyle.Bold);
            navigationLabel=Text(canvasRect,"",20,Vector2.zero,new Vector2(330,38),Cream,FontStyle.Bold);
            var navigationShadow=navigationLabel.gameObject.AddComponent<Shadow>();
            navigationShadow.effectColor=new Color(0,0,0,.85f);navigationShadow.effectDistance=new Vector2(1,-1);
            var footer=HudCard("Controles de navegación",0,0,1180,52);
            footer.anchorMin=footer.anchorMax=new Vector2(.5f,0); footer.pivot=new Vector2(.5f,0);
            footer.anchoredPosition=new Vector2(0,18);
            HudText(footer,"W/S Navegar     A/D Girar     Ratón Apuntar     Clic Disparar     ESC Pausa",20,18,8,1144,36,Cream).alignment=TextAnchor.MiddleCenter;
            UpdateObjectives();
            UpdateNavigation();
        }

        private RectTransform HudCard(string name,float x,float y,float width,float height)
        {
            var image=Image(canvasRect,name,UiTheme.Surface); image.type=UnityEngine.UI.Image.Type.Sliced;
            var rect=image.rectTransform; SetTopLeft(rect,x,y,width,height); return rect;
        }
        private static void SetTopLeft(RectTransform rect,float x,float y,float width,float height)
        {
            rect.anchorMin=rect.anchorMax=new Vector2(0,1); rect.pivot=new Vector2(0,1);
            rect.anchoredPosition=new Vector2(x,-y); rect.sizeDelta=new Vector2(width,height);
        }
        private Text HudText(Transform parent,string value,int size,float x,float y,float width,float height,Color color,bool bold=false)
        {
            var label=Text(parent,value,size,Vector2.zero,new Vector2(width,height),color,bold?FontStyle.Bold:FontStyle.Normal);
            SetTopLeft(label.rectTransform,x,y,width,height); label.alignment=TextAnchor.MiddleLeft; return label;
        }
        private Image HealthBar(RectTransform parent,string title,Health health,float x,float y,float width,Sprite skin,out Text label)
        {
            label=HudText(parent,title,28,x,y,width,38,Cream,true);
            var track=Image(parent,title+" / pista de vida",UiTheme.Track); track.type=UnityEngine.UI.Image.Type.Sliced;
            SetTopLeft(track.rectTransform,x,y+48,width,24);
            var fill=Image(track.rectTransform,title+" / barra de vida",skin);
            Stretch(fill.rectTransform); fill.rectTransform.offsetMin=new Vector2(3,3);fill.rectTransform.offsetMax=new Vector2(-3,-3);
            fill.type=UnityEngine.UI.Image.Type.Filled;fill.fillMethod=UnityEngine.UI.Image.FillMethod.Horizontal;
            fill.fillAmount=health.Current/(float)health.Max;
            return fill;
        }
        private static string ActorName(ShipActor actor) => actor.Id.StartsWith("battery") ? "Batería "+actor.Id.Substring(7)
            : actor.Id.StartsWith("gunboat") ? "Cañonera" : actor.Faction==Faction.Ally ? "Convoy aliado" : "Patrullera";
        private void UpdateObjectives()
        {
            if(objectiveList==null || hudEnemies==null)return;
            int destroyed=0; foreach(var actor in hudEnemies)if(actor==null || !actor.Alive)++destroyed;
            string survival=(root.Player!=null && root.Player.Alive ? "[+] " : "[x] ")+"Mantener el Huáscar operativo";
            if(hudMission==2)
            {
                float route=root.Convoy==null?0:Mathf.InverseLerp(-9,9,root.Convoy.transform.position.x);
                objectiveCount.text="Escolta en curso  ·  "+Mathf.RoundToInt(route*100)+" %";
                objectiveList.text=survival+"\n[ ] Proteger el convoy aliado\n[ ] Llegar al destino oriental";
            }
            else
            {
                objectiveCount.text=destroyed+" / "+hudEnemies.Count+" objetivos neutralizados";
                string rows=survival;
                for(int i=0;i<hudEnemies.Count;++i)
                {
                    var actor=hudEnemies[i];
                    rows+="\n"+(actor==null || !actor.Alive?"[+] ":"[ ] ")+"Neutralizar "+objectiveNames[i].ToLowerInvariant();
                }
                objectiveList.text=rows;
            }
        }

        public void ShowPause()
        {
            Clear();
            var panel = Panel("Pausa", Navy);
            Text(panel, "PARTIDA EN PAUSA", 82, new Vector2(0, -240), new Vector2(1450, 120), Gold, FontStyle.Bold);
            Text(panel, "El mar puede esperar.", 34, new Vector2(0, -360), new Vector2(1050, 64), Cream);
            Button(panel, "Continuar partida", new Vector2(0, -535), root.Resume);
            Button(panel, "Volver al menú", new Vector2(0, -650), root.ReturnMenu);
            Footer(panel, "ESC para continuar");
        }

        public void ShowResult(int number, MissionState result, MissionFailure failure)
        {
            Clear();
            bool victory=result==MissionState.Victory;
            var panel=Panel(victory?"Resultado / victoria":"Resultado / derrota",Navy,
                victory?"Artstyle/victory-background":"Artstyle/defeat-background");
            var card=Image(panel,"Informe de misión",UiTheme.Surface);card.type=UnityEngine.UI.Image.Type.Sliced;
            SetTopLeft(card.rectTransform,100,146,750,736);
            var heading=HudText(card.transform,victory?"VICTORIA":"DERROTA",62,40,30,670,90,victory?Gold:Signal);
            heading.font=titleFont;
            HudText(card.transform,"MISIÓN 0"+number+" · "+(victory?"Objetivo cumplido":"Operación interrumpida"),23,40,134,670,48,Cream,true);
            string message=victory
                ? number==1?"La patrullera ha sido neutralizada. La ruta queda libre para el convoy."
                    :number==2?"El convoy llegó a destino. Es hora de asegurar la costa."
                    :"Las baterías han sido neutralizadas. La campaña ha concluido."
                :failure==MissionFailure.ObjectiveDestroyed?"El convoy fue destruido. Mantente cerca y protege su ruta en el próximo intento."
                    :"El Huáscar ha sido destruido. Reagrupa la flota y vuelve a intentarlo.";
            HudText(card.transform,message,26,40,210,670,126,Cream).alignment=TextAnchor.UpperLeft;
            reportLabel=HudText(card.transform,"Preparando el informe de combate...",21,40,354,670,70,Gold);
            reportLabel.alignment=TextAnchor.UpperLeft;
            if(victory && number<3)
            {
                Button(card.transform,"Continuar campaña",new Vector2(0,-490),()=>root.PreviewMission(number+1),true,new Vector2(650,68));
                Button(card.transform,"Repetir misión",new Vector2(0,-582),root.Retry,true,new Vector2(650,68));
                Button(card.transform,"Volver al menú",new Vector2(0,-674),root.ReturnMenu,true,new Vector2(650,68));
            }
            else
            {
                Button(card.transform,victory?"Repetir misión":"Reintentar misión",new Vector2(0,-510),root.Retry,true,new Vector2(650,72));
                Button(card.transform,"Volver al menú",new Vector2(0,-610),root.ReturnMenu,true,new Vector2(650,72));
            }
            Footer(panel,"Una historia alternativa de ficción · La Resurrección del Huáscar");
        }

        public void UpdateReport(string message)
        {
            if (reportLabel != null) reportLabel.text = message;
        }

        private void OnPlayerChanged(int current, int maximum)
        {
            if(healthLabel!=null)healthLabel.text="Huáscar   "+current+" / "+maximum;
            if(playerFill!=null){playerFill.fillAmount=current/(float)maximum;playerFill.sprite=current<maximum*.3f?UiTheme.Danger:UiTheme.Health;}
        }
        private void OnConvoyChanged(int current, int maximum)
        {
            if(convoyLabel!=null)convoyLabel.text="Convoy   "+current+" / "+maximum;
            if(convoyFill!=null)convoyFill.fillAmount=current/(float)maximum;
        }

        private void Update()
        {
            UpdateObjectives();
            UpdateNavigation();
            if (minimapRect == null) return;
            for (int i = mapMarkers.Count - 1; i >= 0; --i)
            {
                var marker = mapMarkers[i];
                if (marker.Actor == null || !marker.Actor.Alive)
                {
                    Destroy(marker.Rect.gameObject);
                    Destroy(marker.Halo.gameObject);
                    mapMarkers.RemoveAt(i);
                    continue;
                }
                Vector3 position = marker.Actor.transform.position;
                float x = Mathf.InverseLerp(GameRoot.WorldMinX, GameRoot.WorldMaxX, position.x);
                float y = Mathf.InverseLerp(GameRoot.WorldMinY, GameRoot.WorldMaxY, position.y);
                marker.Rect.anchorMin = marker.Rect.anchorMax = new Vector2(x, y);
                marker.Rect.anchoredPosition = Vector2.zero;
                marker.Halo.anchorMin = marker.Halo.anchorMax = new Vector2(x, y);
                marker.Halo.anchoredPosition = Vector2.zero;
                float pulse = 1f + Mathf.Sin(Time.unscaledTime * 3.2f + marker.Phase) * .16f;
                marker.Halo.localScale = Vector3.one * pulse;
            }
        }

        private void UpdateNavigation()
        {
            if (navigationArrow == null || navigationLabel == null) return;
            var target = root.NavigationTarget;
            var player = root.Player;
            bool visible = target != null && player != null && target.Alive;
            navigationArrow.gameObject.SetActive(visible);
            navigationLabel.gameObject.SetActive(visible);
            if (!visible) return;
            var viewport = root.GameCamera.WorldToViewportPoint(target.transform.position);
            bool offscreen = viewport.x < .08f || viewport.x > .92f || viewport.y < .12f || viewport.y > .78f;
            Vector2 direction = new Vector2(viewport.x - .5f, viewport.y - .5f);
            Vector2 point = new Vector2(Mathf.Clamp(viewport.x, .12f, .88f), Mathf.Clamp(viewport.y, .16f, .74f));
            point.x=Mathf.Max(point.x,710f/canvasRect.rect.width);
            if (point.x > .76f && point.y > .60f) point.y = .60f;
            Vector2 local = new Vector2((point.x - .5f) * canvasRect.rect.width,
                (point.y - 1) * canvasRect.rect.height);
            navigationArrow.rectTransform.anchoredPosition = local + (offscreen ? Vector2.zero : new Vector2(0, 65));
            navigationArrow.rectTransform.localRotation = Quaternion.Euler(0, 0,
                offscreen ? Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg : -90);
            navigationLabel.rectTransform.anchoredPosition = navigationArrow.rectTransform.anchoredPosition + new Vector2(0, -44);
            float distance = Vector2.Distance(player.transform.position, target.transform.position);
            navigationLabel.text = (target.Faction == Faction.Ally ? "CONVOY ALIADO" : ActorName(target).ToUpperInvariant()) + "  ·  " + distance.ToString("0.0") + " u";
        }

        private void AddMapMarker(ShipActor actor, Color color)
        {
            var halo = Image(minimapRect, actor.Id + " / halo", UiTheme.Knob);
            halo.color = new Color(color.r, color.g, color.b, .22f);
            halo.raycastTarget = false;
            halo.rectTransform.sizeDelta = new Vector2(30, 30);
            var marker = Image(minimapRect, actor.Id + " / marcador", Visuals.White);
            marker.color = color;
            marker.raycastTarget = false;
            marker.rectTransform.sizeDelta = new Vector2(14, 14);
            marker.rectTransform.localRotation = Quaternion.Euler(0, 0, 45);
            mapMarkers.Add(new MapMarker { Rect = marker.rectTransform, Halo = halo.rectTransform, Actor = actor, Phase = mapMarkers.Count * .8f });
        }

        private RectTransform ChartCard(Transform parent, Vector2 position, Vector2 dimensions, string title)
        {
            var card = Image(parent, title, UiTheme.Surface);
            card.type = UnityEngine.UI.Image.Type.Sliced;
            card.rectTransform.anchorMin = card.rectTransform.anchorMax = new Vector2(.5f, 1);
            card.rectTransform.anchoredPosition = position;
            card.rectTransform.sizeDelta = dimensions;
            card.color = new Color(.82f, .9f, .87f, .96f);
            var mask = Image(card.rectTransform, "Ventana redondeada de carta", UiTheme.Surface);
            mask.type = UnityEngine.UI.Image.Type.Sliced;
            mask.rectTransform.anchorMin = Vector2.zero;
            mask.rectTransform.anchorMax = Vector2.one;
            mask.rectTransform.offsetMin = new Vector2(16, 16);
            mask.rectTransform.offsetMax = new Vector2(-16, -46);
            mask.gameObject.AddComponent<Mask>().showMaskGraphic = false;
            var chart = Image(mask.rectTransform, "Mapa táctico", UiVisuals.Asset("UI/Panels/tactical_map"));
            Stretch(chart.rectTransform);
            chart.color = Color.white;
            Text(card.rectTransform, title, 18, new Vector2(0, -32),
                new Vector2(dimensions.x - 32, 32), Cream, FontStyle.Bold);
            return chart.rectTransform;
        }

        private void AddAtmosphere(RectTransform parent)
        {
            var wash = Image(parent, "Veladura de tinta", null);
            Stretch(wash.rectTransform);
            wash.color = new Color(.02f, .06f, .08f, .18f);
            wash.raycastTarget = false;
            var glint = Image(parent, "Brillo ornamental", null);
            glint.rectTransform.anchorMin = new Vector2(0, 1);
            glint.rectTransform.anchorMax = new Vector2(1, 1);
            glint.rectTransform.pivot = new Vector2(.5f, 1);
            glint.rectTransform.sizeDelta = new Vector2(0, 2);
            glint.color = new Color(.92f, .78f, .48f, .5f);
            glint.raycastTarget = false;
        }

        private void PixelFrame(RectTransform parent, Color accent, int thickness)
        {
            var top = Image(parent, "Marco pixel superior", null);
            top.rectTransform.anchorMin = new Vector2(0, 1);
            top.rectTransform.anchorMax = new Vector2(1, 1);
            top.rectTransform.pivot = new Vector2(.5f, 1);
            top.rectTransform.sizeDelta = new Vector2(-thickness * 2, thickness);
            top.rectTransform.anchoredPosition = Vector2.zero;
            top.color = accent;
            var bottom = Image(parent, "Marco pixel inferior", null);
            bottom.rectTransform.anchorMin = new Vector2(0, 0);
            bottom.rectTransform.anchorMax = new Vector2(1, 0);
            bottom.rectTransform.pivot = new Vector2(.5f, 0);
            bottom.rectTransform.sizeDelta = new Vector2(-thickness * 2, thickness);
            bottom.rectTransform.anchoredPosition = Vector2.zero;
            bottom.color = PixelShadow;
            var left = Image(parent, "Marco pixel izquierdo", null);
            left.rectTransform.anchorMin = new Vector2(0, 0);
            left.rectTransform.anchorMax = new Vector2(0, 1);
            left.rectTransform.pivot = new Vector2(0, .5f);
            left.rectTransform.sizeDelta = new Vector2(thickness, -thickness * 2);
            left.rectTransform.anchoredPosition = Vector2.zero;
            left.color = accent;
            var right = Image(parent, "Marco pixel derecho", null);
            right.rectTransform.anchorMin = new Vector2(1, 0);
            right.rectTransform.anchorMax = new Vector2(1, 1);
            right.rectTransform.pivot = new Vector2(1, .5f);
            right.rectTransform.sizeDelta = new Vector2(thickness, -thickness * 2);
            right.rectTransform.anchoredPosition = Vector2.zero;
            right.color = PixelShadow;
            var corner = Image(parent, "Remate ornamental", null);
            corner.rectTransform.anchorMin = corner.rectTransform.anchorMax = new Vector2(0, 1);
            corner.rectTransform.sizeDelta = new Vector2(18, 18);
            corner.rectTransform.anchoredPosition = new Vector2(thickness, -thickness);
            corner.color = accent;
        }

        private RectTransform Panel(string name, Color color, string backgroundPath=null)
        {
            var image = Image(canvasRect, name, UiVisuals.Asset(backgroundPath??"UI/Brand/menu_background"));
            Stretch(image.rectTransform);
            image.color = Color.white;
            AddAtmosphere(image.rectTransform);
            return image.rectTransform;
        }

        private RectTransform Bar(string name, Color color, float height, bool top)
        {
            var image = Image(canvasRect, name, UiVisuals.Asset("UI/Hud/top_hud", new Vector4(160, 160, 160, 60)));
            var rect = image.rectTransform;
            rect.anchorMin = new Vector2(0, top ? 1 : 0);
            rect.anchorMax = new Vector2(1, top ? 1 : 0);
            rect.pivot = new Vector2(.5f, top ? 1 : 0);
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = new Vector2(0, height);
            image.type = UnityEngine.UI.Image.Type.Sliced;
            image.color = Color.white;

            return rect;
        }

        private Text Text(Transform parent, string value, int size, Vector2 position, Vector2 dimensions,
            Color color, FontStyle style = FontStyle.Normal)
        {
            var go = new GameObject("Texto: " + value, typeof(RectTransform), typeof(Text));
            go.transform.SetParent(parent, false);
            var rect = (RectTransform)go.transform;
            rect.anchorMin = rect.anchorMax = new Vector2(.5f, 1);
            rect.anchoredPosition = position;
            rect.sizeDelta = dimensions;
            var text = go.GetComponent<Text>();
            text.text = value;
            text.font = font;
            text.fontSize = size;
            text.fontStyle = style;
            text.alignment = TextAnchor.MiddleCenter;
            text.color = color;
            text.raycastTarget = false;
            text.horizontalOverflow=HorizontalWrapMode.Wrap;
            text.verticalOverflow=VerticalWrapMode.Truncate;
            text.resizeTextForBestFit = true;
            text.resizeTextMinSize = Mathf.Min(18, size);
            text.resizeTextMaxSize = size;
            return text;
        }

        private Button Button(Transform parent, string title, Vector2 position, UnityEngine.Events.UnityAction action,
            bool enabled = true, Vector2? dimensions = null)
        {
            var image = Image(parent, title, ButtonSprite(title, enabled));
            var rect = image.rectTransform;
            rect.anchorMin = rect.anchorMax = new Vector2(.5f, 1);
            rect.anchoredPosition = position;
            rect.sizeDelta = dimensions ?? new Vector2(760, 82);
            image.type = UnityEngine.UI.Image.Type.Sliced;
            image.color = Color.white;
            image.raycastTarget = true;
            var button = image.gameObject.AddComponent<Button>();
            button.targetGraphic = image;
            button.interactable = enabled;
            button.onClick.AddListener(root.Sounds.Click);
            button.onClick.AddListener(action);
            var colors = button.colors;
            colors.normalColor = enabled ? new Color(.06f, .25f, .31f, .98f) : new Color(.12f, .15f, .17f, .8f);
            colors.highlightedColor = new Color(.12f, .39f, .45f, 1f);
            colors.pressedColor = new Color(.82f, .55f, .22f, 1f);
            colors.selectedColor = colors.highlightedColor;
            colors.disabledColor = new Color(.12f, .15f, .17f, .65f);
            colors.colorMultiplier = 1f;
            colors.fadeDuration = .12f;
            button.colors = colors;
            var state = new SpriteState
            {
                highlightedSprite = ButtonSprite(title, enabled, "hover"),
                pressedSprite = ButtonSprite(title, enabled, "pressed"),
                disabledSprite = ButtonSprite(title, false, "disabled")
            };
            button.spriteState = state;
            button.transition = Selectable.Transition.SpriteSwap;

            var label = Text(rect, title, 30, new Vector2(0, -rect.sizeDelta.y / 2), rect.sizeDelta-new Vector2(64,12), enabled ? Cream : Color.gray, FontStyle.Bold);
            label.rectTransform.anchorMin = label.rectTransform.anchorMax = new Vector2(.5f, 1);
            return button;
        }

        private static Sprite ButtonSprite(string title, bool enabled, string state = "normal")
        {
            if(!enabled || state=="disabled")return UiTheme.Disabled;
            return state=="hover" ? UiTheme.Hover : state=="pressed" ? UiTheme.Pressed : UiTheme.Normal;
        }

        private static Image Image(Transform parent, string name, Sprite sprite)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Image));
            go.transform.SetParent(parent, false);
            var image = go.GetComponent<Image>();
            image.sprite = sprite;
            image.preserveAspect = false;
            image.raycastTarget = false;
            return image;
        }

        private static void Stretch(RectTransform rect)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = rect.offsetMax = Vector2.zero;
        }

        private void AccentRail(RectTransform parent, Color color)
        {
            var rail = Image(parent, "Acento de superficie", null);
            rail.rectTransform.anchorMin = new Vector2(.08f, 1);
            rail.rectTransform.anchorMax = new Vector2(.92f, 1);
            rail.rectTransform.pivot = new Vector2(.5f, 1);
            rail.rectTransform.anchoredPosition = new Vector2(0, -2);
            rail.rectTransform.sizeDelta = new Vector2(0, 2);
            rail.color = color;
            rail.raycastTarget = false;
        }

        private void Ornament(RectTransform parent)
        {
            var image = Image(parent, "Línea dorada", null);
            image.rectTransform.anchorMin = image.rectTransform.anchorMax = new Vector2(.5f, 1);
            image.rectTransform.anchoredPosition = new Vector2(0, -399);
            image.rectTransform.sizeDelta = new Vector2(740, 4);
            image.color = Gold;
        }

        private void Footer(RectTransform parent, string message)
        {
            var label = Text(parent, message, 22, new Vector2(0, -1025), new Vector2(1750, 62), Cream);
            label.rectTransform.anchorMin = label.rectTransform.anchorMax = new Vector2(.5f, 0);
            label.rectTransform.anchoredPosition = new Vector2(0, 32);
        }

        private static void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        private void OnDestroy() { Clear(); }
    }
}
