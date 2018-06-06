using ETModel;
using ETHotfix;
using UnityEngine;

namespace ETHotfix
{
	[MessageHandler]
	public class Actor_CreateUnitsHandler : AMHandler<Actor_CreateUnits>
	{
		protected override void Run(ETModel.Session session, Actor_CreateUnits message)
		{
			// 加载Unit资源
			ResourcesComponent resourcesComponent = ETModel.Game.Scene.GetComponent<ResourcesComponent>();
			resourcesComponent.LoadBundle($"Unit.unity3d");
			
			UnitComponent unitComponent = ETModel.Game.Scene.GetComponent<UnitComponent>();
            

            foreach (UnitInfo unitInfo in message.Units)
			{

                if (unitComponent.Get(unitInfo.UnitId) != null)
				{
					continue;
				}


				Unit unit = UnitFactory.Create(unitInfo.UnitId);
        
                unit.Position = new Vector3(unitInfo.X / 1000f, 0, unitInfo.Z / 1000f);
				unit.IntPos = new VInt3(unitInfo.X, 0, unitInfo.Z);
                //Log.Info(" PlayerComponent.Instance.MyPlayer.UnitId:  " + PlayerComponent.Instance.MyPlayer.UnitId.ToString());
                //Log.Info("unit.Id :  " + unit.Id);
                //PlayerComponent.Instance.MyPlayer.UnitId = unit.Id;
    //            if (PlayerComponent.Instance.MyPlayer.UnitId == unit.Id)
				//{
				//	ETModel.Game.Scene.GetComponent<CameraComponent>().Unit = unit;

    //                //Log.Info("Unit id:" + unit.Id.ToString());
    //                //unit.GameObject.AddComponent<Rigidbody>();
    //            }
			}
            if (Game.Scene.GetComponent<ClassComponent>()==null) {
                Game.Scene.AddComponent<ClassComponent>();
            }
            if (Game.Scene.GetComponent<OperaComponent>() == null)
            {
                Game.Scene.AddComponent<OperaComponent>();
            }
		}
	}
}
