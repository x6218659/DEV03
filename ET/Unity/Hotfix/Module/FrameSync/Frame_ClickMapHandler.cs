using ETModel;
using UnityEngine;

namespace ETHotfix
{
	[MessageHandler]
	public class Frame_ClickMapHandler : AMHandler<Frame_ClickMap>
	{
		protected override void Run(ETModel.Session session, Frame_ClickMap message)
		{
			Unit unit = ETModel.Game.Scene.GetComponent<UnitComponent>().Get(message.Id);

   //         PlayerComponent.Instance.MyPlayer.UnitId = unit.Id;
   //         ETModel.Game.Scene.GetComponent<CameraComponent>().Unit = unit;
            if (ETModel.Game.Scene.GetComponent<CameraComponent>().Unit == null) {
                Log.Info("这Camera.Unit是空");
            }
        
            MoveComponent moveComponent = unit.GetComponent<MoveComponent>();
			Vector3 dest = new Vector3(message.X / 1000f, 0, message.Z / 1000f);
			moveComponent.MoveToDest(dest, 1);
			moveComponent.Turn2D(dest - unit.Position);
          // GameObject.CreatePrimitive(PrimitiveType.Cube).transform.position= dest;
		}
	}

    [MessageHandler]
    public class Frame_ClickMapDoSomeHandler : AMHandler<Frame_ClickMapToDo>
    {
        protected override void Run(ETModel.Session session, Frame_ClickMapToDo message)
        {
            Debug.Log("自己的帧同步消息");
        }
    }
}
