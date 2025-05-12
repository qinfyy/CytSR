using GameServer.Network;
using GameServer.Resources;
using Proto;

namespace GameServer.Handlers
{
    [PacketHandler(CmdId.CmdPlayerLoginCsReq)]
    internal class HandlerPlayerLoginCsReq : IPacketHandler
    {
        public void Handle(GameSession session, NetPacket packet)
        {
            session._Player.OnPlayerLogin();

            PlayerLoginScRsp rsp = new PlayerLoginScRsp()
            {
                Retcode = 0,
                //LoginRandom = req.LoginRandom,
                ServerTimestampMs = (ulong)DateTimeOffset.Now.ToUnixTimeSeconds() * 1000,
                Stamina = 240,
                BasicInfo = session._Player.GetPlayerBasicInfoProto(),
            };

            var cpgdsr = new ContentPackageGetDataScRsp()
            {
                Retcode = 0,
                Data = new ContentPackageData()
            };

            foreach (var item in GameData.ContentPackageConfigData)
            {
                var cpi = new ContentPackageInfo
                {
                    ContentId = item.Key,
                    Status = ContentPackageStatus.ContentPackageStatusFinished
                };
                cpgdsr.Data.ContentPackageLists.Add(cpi);
            }

            session.Send(NetPacket.Create(CmdId.CmdPlayerLoginScRsp, rsp));
            session.Send(NetPacket.Create(CmdId.CmdContentPackageGetDataScRsp, cpgdsr));
        }
    }
}
