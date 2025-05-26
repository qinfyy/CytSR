using GameServer.Network;
using Proto;

namespace GameServer.Handlers
{
    [PacketHandler(CmdId.CmdGetBasicInfoCsReq)]
    internal class HandlerGetBasicInfoCsReq : IPacketHandler
    {
        public void Handle(GameSession session, NetPacket packet)
        {
            var rsp = new GetBasicInfoScRsp()
            {
                CurDay = 1,
                //IsGenderSet = true,
                Gender = session._Player.Data.AvatarCompData.TrailblazerGender,
                PlayerSettingInfo = new Djbllokkand(),
                WeekCocoonFinishedCount = 0,
                NextRecoverTime = 2281337
            };

            session.Send(NetPacket.Create(CmdId.CmdGetBasicInfoScRsp, rsp));
        }
    }
}
