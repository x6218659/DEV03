using System;
using ETModel;

namespace ETHotfix
{
	[MessageHandler(AppType.Location)]
	public class ObjectGetRequestHandler : AMRpcHandler<ObjectGetRequest, ObjectGetResponse>
	{
		protected override async void Run(Session session, ObjectGetRequest message, Action<ObjectGetResponse> reply)
		{
			ObjectGetResponse response = new ObjectGetResponse();
            try
            {
                //Log.Info(message.Key.ToString());

                ////if (message.Key==0) {
                ////    session.Dispose();
                ////    return;
                ////}

				long instanceId = await Game.Scene.GetComponent<LocationComponent>().GetAsync(message.Key);

                if (instanceId == 0)
                {
                    session.Dispose();

                    //response.Error = ErrorCode.ERR_ActorLocationNotFound;
                    return;
                }
             
                    response.InstanceId = instanceId;
                    reply(response);
                
			
			}
			catch (Exception e)
			{
				ReplyError(response, e, reply);
			}
		}
	}
}