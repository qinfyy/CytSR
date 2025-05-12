using GameServer.Resources;
using Proto;
using GameServer.Network;

namespace GameServer.Handlers
{
    [PacketHandler(CmdId.CmdGetUnlockTeleportCsReq)]
    internal class HandlerGetUnlockTeleportCsReq : IPacketHandler
    {
        public void Handle(GameSession session, NetPacket packet)
        {
            var req = NetPacket.GetProto<GetUnlockTeleportCsReq>(packet);
            var rsp = new GetUnlockTeleportScRsp();

            foreach (var entranceId in req.EntryIdLists)
            {
                GameData.MapEntranceData.TryGetValue(entranceId, out var mec);
                if (mec == null) 
                    continue;

                GameData.GetFloorInfo(mec.PlaneId, mec.FloorId, out var floorInfo);
                if (floorInfo == null) 
                    continue;

                var utl = new List<uint>();
                foreach (var teleport in floorInfo.CachedTeleports)
                    utl.Add(teleport.Value.MappingInfoID);
                rsp.UnlockTeleportLists = utl.ToArray();
            }

            session.Send(NetPacket.Create(CmdId.CmdGetUnlockTeleportScRsp, rsp));
        }
    }
}
