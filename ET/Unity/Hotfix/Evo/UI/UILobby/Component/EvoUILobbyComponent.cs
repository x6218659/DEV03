using System;
using ETModel;
using ETHotfix;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
   
    [ObjectSystem]
    public class EvoUiLobbyComponentSystem : AwakeSystem<EvoUILobbyComponent>
    {
        public override void Awake(EvoUILobbyComponent self)
        {
            self.Awake();
        }
    }

    [ObjectSystem]
    public class EvoUiLobbyComponentUpdateSystem : LateUpdateSystem<EvoUILobbyComponent>
    {
        public override void LateUpdate(EvoUILobbyComponent self)
        {
            self.LateUpdate();
        }
    }

    public class EvoUILobbyComponent : Component
    {
        private Camera modelCamera;
        private RenderTexture renderTexture;
        private RawImage rawImage;
        private GameObject enterMap;
        public void Awake()
        {
            ReferenceCollector rc = this.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();
            modelCamera = rc.Get<GameObject>("modelCamera").GetComponent<Camera>();
            if (modelCamera == null) {
                Log.Info("modelCamera是空的");
                return;
            }
               
            modelCamera.clearFlags = CameraClearFlags.SolidColor;
            modelCamera.backgroundColor = new Color(0,0,0,0);
            modelCamera.fieldOfView = Camera.main.fieldOfView;
            modelCamera.allowHDR = true;
            modelCamera.allowMSAA = true;
            modelCamera.allowDynamicResolution = true;
            renderTexture = new RenderTexture(1920,1080,24);
            renderTexture.dimension = UnityEngine.Rendering.TextureDimension.Tex2D;
            renderTexture.antiAliasing = 8;
            renderTexture.format = RenderTextureFormat.ARGB64;
            renderTexture.useDynamicScale = true;
            modelCamera.targetTexture = renderTexture;
            rawImage = rc.Get<GameObject>("rawImage").GetComponent<RawImage>();
            rawImage.GetComponent<RectTransform>().anchorMax = Vector2.one;
            rawImage.GetComponent<RectTransform>().anchorMin = Vector2.zero;
            rawImage.GetComponent<RectTransform>().pivot = Vector2.one/2f;
            rawImage.GetComponent<RectTransform>().localScale = Vector3.one;
            rawImage.GetComponent<RectTransform>().localPosition=Vector3.zero;
            rawImage.GetComponent<RectTransform>().anchoredPosition3D= Vector3.zero;
            rawImage.GetComponent<RectTransform>().sizeDelta = Vector2.zero;
            rawImage.texture = renderTexture;
            enterMap = rc.Get<GameObject>("EnterMap");
            enterMap.GetComponent<Button>().onClick.Add(this.EnterMap);
            showtab = modelCamera.transform.parent.Find("ShowTab");
        }
        Transform showtab;
        public void LateUpdate()
        {
            if (Input.touchCount == 1) {
                if (Input.touches[0].phase == TouchPhase.Moved) {
                    showtab.RotateAroundLocal(Vector3.up, -Input.touches[0].deltaPosition.x*Time.deltaTime*5f);
                }
            }
            if (Input.GetMouseButton(0))
            {
                //if (Input.touches[0].phase == TouchPhase.Moved)
                //{
                showtab.RotateAroundLocal(Vector3.up, -Input.GetAxis("Mouse X") * Time.deltaTime * 5f);
                //}
            }
        }

        private async void EnterMap()
        {
            try
            {
                G2C_EnterMap g2CEnterMap = (G2C_EnterMap)await ETModel.SessionComponent.Instance.Session.Call(new C2G_EnterMap());
                Game.Scene.GetComponent<UIComponent>().Remove(UIType.EvoUILobby);

            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }
}
