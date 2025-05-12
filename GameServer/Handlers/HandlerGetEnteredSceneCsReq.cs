using Proto;
using GameServer.Resources;
using GameServer.Resources.Excel;
using GameServer.Network;

namespace GameServer.Handlers
{
    [PacketHandler(CmdId.CmdGetEnteredSceneCsReq)]
    internal class HandlerGetEnteredSceneCsReq : IPacketHandler
    {
        public void Handle(GameSession session, NetPacket packet)
        {
            var rsp = new GetEnteredSceneScRsp()
            {
                Retcode = 0
            };

            foreach (MapEntranceExcel mapEntranceExcel in GameData.MapEntranceData.Values)
            {
                if (mapEntranceExcel.FinishMainMissionIdList.Count != 0 || mapEntranceExcel.FinishMainMissionIdList.Count != 0)
                {
                    var enteredSceneInfo = new EnteredSceneInfo
                    {
                        FloorId = mapEntranceExcel.FloorId,
                        PlaneId = mapEntranceExcel.PlaneId
                    };
                    
                    rsp.EnteredSceneInfoLists.Add(enteredSceneInfo);
                }
            }
            session.Send(NetPacket.Create(CmdId.CmdGetEnteredSceneScRsp, rsp));
        }
    }
}
