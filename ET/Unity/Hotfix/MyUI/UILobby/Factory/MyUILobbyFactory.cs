using System;
using ETModel;
using UnityEngine;

namespace ETHotfix
{
    [UIFactory(UIType.MyUILobby)]
    public class MyUILobbyFactory : IUIFactory
    {
        public UI Create(Scene scene, string type, GameObject gameObject)
        {
	        try
	        {
                Debug.Log("14124");

                //获取资源管理组件 
				ResourcesComponent resourcesComponent = ETModel.Game.Scene.GetComponent<ResourcesComponent>();
                //加载对应的Ab资源包并加载
                resourcesComponent.LoadBundle($"{type}.unity3d");
                //获取对象资源物体
				GameObject bundleGameObject = (GameObject)resourcesComponent.GetAsset($"{type}.unity3d", $"{type}");
                //创建对象UI
                GameObject lobby = UnityEngine.Object.Instantiate(bundleGameObject);
                //设定UI层
				lobby.layer = LayerMask.NameToLayer(LayerNames.UI);
                //调用组件工厂创建UI lobby大厅
				UI ui = ComponentFactory.Create<UI, GameObject>(lobby);

                //给该ui挂载UI大厅组件
				ui.AddComponent<MyUILobbyComponent>();
                //组装完毕返回出去
				return ui;
	        }
	        catch (Exception e)
	        {
				Log.Error(e);
		        return null;
	        }
		}

	    public void Remove(string type)
	    {
			ETModel.Game.Scene.GetComponent<ResourcesComponent>().UnloadBundle($"{type}.unity3d");
		}
    }
}