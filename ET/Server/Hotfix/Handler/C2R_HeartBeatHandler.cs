using System;
using System.Collections.Generic;
using System.Text;
using ETModel;

namespace ETHotfix
{
    [MessageHandler(AppType.Gate)]
    public class C2R_HeartBeatHandler : AMRpcHandler<C2R_HeartBeat, R2C_HeartBeat>
      // public class C2R_HeartBeatHandler : AMRpcHandler<C2R_HeartBeat, R2C_HeartBeat>
    {
        protected override void Run(Session session, C2R_HeartBeat message, Action<R2C_HeartBeat> reply)
        {
            if (session.GetComponent<HeartBeatComponent>() != null)
            {
                session.GetComponent<HeartBeatComponent>().CurrentTime = TimeHelper.ClientNowSeconds();
            }
            Log.Info("心跳更新ID："+session.Id);
            reply(new R2C_HeartBeat() { });
        }
    }
}
