using ETModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETHotfix.Handler
{
    [MessageHandler(AppType.Gate)]
    public class GetInfoHandler : AMRpcHandler<UserInfor, UserInfo>
    {
        protected override void Run(Session session, UserInfor message, Action<UserInfo> reply)
        {
            UserInfo response = new UserInfo();
            try {
                response.Coin = 10;
                response.Exp = 100;
                reply(response);
            }
            catch (Exception e) {
                ReplyError(response, e, reply);
            }
        }
    }
}
