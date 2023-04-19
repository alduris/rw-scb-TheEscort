using System;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;


namespace TheEscort{
    public partial class Escort{
        public bool Speedster;
        public int SpeSpeedin;
        public bool SpeDashNCrash;
        public Queue<SpeedTrail> SpeTrail;
        public int SpeTrailTick;
        public bool SpeSecretSpeed;
        public int SpeExtraSpe;
        public Color SpeColor;
        public int SpeBonk;

        public void EscortSS(){
            this.Speedster = false;
            this.SpeSpeedin = 0;
            this.SpeExtraSpe = 0;
            this.SpeDashNCrash = false;
            this.SpeSecretSpeed = false;
            if (this.SpeTrail == null){
                this.SpeTrail = new Queue<SpeedTrail>();
            }
            this.SpeTrailTick = 0;
            this.SpeColor = new Color(0.76f, 0.78f, 0f);
            this.SpeBonk = 0;
        }

        public class SpeedTrail : ISingleCameraDrawable{
            private Vector2 pos;
            private RoomCamera camera;
            public PlayerGraphics playerGraphics;
            private int lifeTime;
            private int maxLife;
            //private RoomCamera.SpriteLeaser pTrail;
            private FSprite[] pSprite;
            private bool[] wasVisible;
            private float[] preAlpha;
            //private Color[] preColor;
            //private float[] preScale;
            private float[] preScaleX;
            private float[] preScaleY;
            private Vector2[] prePos;
            private Color color1;
            private Color color2;
            private Vector2 cPos;
            private Vector2 offset;
            public SpeedTrail(PlayerGraphics pg, RoomCamera.SpriteLeaser s, Color color, Color bonusColor, int life=20){
                this.lifeTime = life;
                this.maxLife = life;
                this.playerGraphics = pg;
                this.color1 = color;
                this.color2 = bonusColor;
                this.camera = pg.owner.room.game.cameras[0];
                this.pSprite = new FSprite[s.sprites.Length];
                this.wasVisible = new bool[s.sprites.Length];
                this.preAlpha = new float[s.sprites.Length];
                //this.preColor = new Color[s.sprites.Length];
                this.preScaleX = new float[s.sprites.Length];
                this.preScaleY = new float[s.sprites.Length];
                this.prePos = new Vector2[s.sprites.Length];
                for (int i = 0; i < s.sprites.Length; i++){
                    //this.pSprite[i] = Clone(s.sprites[i]);
                    this.pSprite[i] = new FSprite(s.sprites[i].element);
                    this.pSprite[i].SetPosition(s.sprites[i].GetPosition());
                    this.pSprite[i].scaleX = s.sprites[i].scaleX;
                    this.pSprite[i].scaleY = s.sprites[i].scaleY;
                    this.preScaleX[i] = s.sprites[i].scaleX;
                    this.preScaleY[i] = s.sprites[i].scaleY;
                    this.pSprite[i].rotation = s.sprites[i].rotation;
                    this.pSprite[i].shader = camera.game.rainWorld.Shaders["Basic"];
                    /*
                    this.wasVisible[i] = s.sprites[i].isVisible;
                    */
                    if (s.sprites[i].element == Futile.atlasManager.GetElementWithName("Futile_White") || s.sprites[i].element == Futile.atlasManager.GetElementWithName("pixel")){
                        this.wasVisible[i] = false;
                    }
                    else {
                        this.wasVisible[i] = s.sprites[i].isVisible;
                    }
                    //this.preScale[i] = s.sprites[i].scale;
                    this.prePos[i] = s.sprites[i].GetPosition();
                    this.pos = s.sprites[1].GetPosition();
                    this.preAlpha[i] = s.sprites[i].alpha;
                    //this.preColor[i] = color;
                };
                this.cPos = this.camera.pos;
                this.offset = new Vector2();
                this.camera.AddSingleCameraDrawable(this);
                foreach(FSprite f in pSprite){
                    camera.ReturnFContainer("Background").AddChild(f);
                }
            }
            /*
            public FSprite Clone(FSprite f){
                using (MemoryStream stream = new MemoryStream()){
                    XmlSerializer spriter = new XmlSerializer(f.GetType());
                    spriter.Serialize(stream, f);
                    stream.Seek(0, SeekOrigin.Begin);
                    return (FSprite) spriter.Deserialize(stream);
                }
            }*/

            public void Update(){
                if (camera.AboutToSwitchRoom){
                    this.KillTrail();
                    return;
                }
                for(int j = 0; j < pSprite.Length; j++){
                    if (lifeTime > maxLife - (int)(maxLife / 10)){
                        pSprite[j].isVisible = false;
                    }
                    else{
                        pSprite[j].isVisible = wasVisible[j];
                        //pSprite[j].color = Color.Lerp(Color.black, preColor[j], Mathf.InverseLerp(0, maxLife - (int)(maxLife / 10), lifeTime));
                        pSprite[j].color = Color.Lerp(color2, color1, Mathf.InverseLerp(0, maxLife - (int)(maxLife / 10), lifeTime));
                        pSprite[j].alpha = preAlpha[j];
                        //pSprite[j]._concatenatedAlpha = Mathf.InverseLerp(0, 40, lifeTime);
                        pSprite[j].scaleX = preScaleX[j] * Mathf.InverseLerp(0, maxLife, lifeTime);
                        pSprite[j].scaleY = preScaleY[j] * Mathf.InverseLerp(0, maxLife - (int)(maxLife / 10), lifeTime);
                        pSprite[j].SetPosition(new Vector2(
                            Mathf.Lerp(pos.x, prePos[j].x, Mathf.InverseLerp(0, maxLife + (int)(maxLife / 10), lifeTime)),
                            Mathf.Lerp(pos.y, prePos[j].y, Mathf.InverseLerp(0, maxLife + (int)(maxLife / 10), lifeTime))
                            ) - this.offset);
                    }
                }
                if (lifeTime > 0){
                    lifeTime--;
                }
            }

            public void KillTrail(){
                foreach(FSprite f in pSprite){
                    camera.ReturnFContainer("Background").RemoveChild(f);
                }
                this.pSprite = null;
                this.wasVisible = null;
                this.preAlpha = null;
                //this.preColor = null;
                this.camera.RemoveSingleCameraDrawable(this);
            }

            public void Draw(RoomCamera rCam, float timeStacker, Vector2 camPos){
                this.offset = new Vector2(camPos.x - this.cPos.x, camPos.y - this.cPos.y);
            }
        }
        public void Escat_addTrail(PlayerGraphics pg, RoomCamera.SpriteLeaser s, int life, int trailCount=10){
            if (this.SpeTrail.Count >= trailCount){
                SpeedTrail trail = this.SpeTrail.Dequeue();
                trail.KillTrail();
            }
            this.SpeTrail.Enqueue(new SpeedTrail(pg, s, (this.SpeSecretSpeed? Color.white : this.hypeColor), (this.SpeSecretSpeed? this.hypeColor : Color.black), life));
        }

        public void Escat_showTrail(RoomCamera rCam, float timeStacker, Vector2 camPos){
            foreach(SpeedTrail trail in this.SpeTrail){
                if (trail.playerGraphics != null && trail.playerGraphics.owner != null){
                    trail.Update();
                } else {
                    trail.KillTrail();
                }
            }
        }

    
    }
}