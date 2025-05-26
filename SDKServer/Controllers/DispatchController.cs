using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Proto;
using ProtoBuf;
using System.Net;
using System.Text.Json;

namespace SDKServer.Controllers
{
    [ApiController]
    [Route("/")]
    public class DispatchController : ControllerBase
    {
        private readonly ILogger<DispatchController> _log;
        private readonly SDKSettings _settings;

        public DispatchController(ILogger<DispatchController> log, IOptions<SDKSettings> settings)
        {
            _log = log;
            _settings = settings.Value;
        }

        [HttpGet("query_dispatch")]
        public IActionResult QueryDispatch()
        {
            GlobalDispatchData dispatchRegion = new GlobalDispatchData()
            {
                Retcode = 0,
            };

            foreach (var region in _settings.DispatchRegionList)
            {
                var regionEntry = new Proto.ServerData()
                {
                    Name = region.Name,
                    Title = region.Tiele,
                    EnvType = region.Env,
                    DispatchUrl = region.DispatchUrl,
                };

                dispatchRegion.ServerLists.Add(regionEntry);
            }

            MemoryStream stream = new MemoryStream();
            Serializer.Serialize(stream, dispatchRegion);
            string rsp = Convert.ToBase64String(stream.ToArray());
            stream.Close();

            _log.LogInformation("客户端请求: query_dispatch");
            return Ok(rsp);
        }

        [HttpGet("query_gateway")]
        public IActionResult QueryGateway()
        {
            try
            {

                var version = Request.Query["version"].ToString();

                var document = JsonDocument.Parse(System.IO.File.ReadAllText("./hotfix.json"));

                string assetBundleUrl = "";
                string exResourceUrl = "";
                string luaUrl = "";
                string ifixUrl = "";

                if (document.RootElement.TryGetProperty(version, out JsonElement versionInfo))
                {
                    assetBundleUrl = versionInfo.GetProperty("assetBundleUrl").GetString();
                    exResourceUrl = versionInfo.GetProperty("exResourceUrl").GetString();
                    luaUrl = versionInfo.GetProperty("luaUrl").GetString();
                    ifixUrl = versionInfo.GetProperty("ifixUrl").GetString();
                }

                Gateserver gateserver = new Gateserver()
                {
                    Ip = _settings.GateServer.Ip,
                    Port = _settings.GateServer.Port,
                    RegionName = _settings.GateServer.RegionName,
                    AssetBundleUrl = assetBundleUrl,
                    ExResourceUrl = exResourceUrl,
                    LuaUrl = luaUrl,
                    IfixUrl = ifixUrl,
                    //IfixVersion = "8123076",
                    EnableVersionUpdate = true,
                    EnableWatermark = true,
                    EnableAndroidMiddlePackage = true,
                    EnableUploadBattleLog = true,
                    EnableSaveReplayFile = true,
                    EventTrackingOpen = true,
                    NetworkDiagnostic = true,
                    MtpSwitch = true,
                    UseNewNetworking = true,
                    EnableDesignDataVersionUpdate = true,
                    ForbidRecharge = true,
                    CloseRedeemCode = true,
                    //Unk1 = true,
                    //Unk2 = true,
                    //Unk3 = true,
                    //Unk4 = true,
                    //Unk5 = true,
                    //Unk6 = true,
                    //Unk7 = true,
                    //Unk8 = true,
                    //Unk9 = true,
                    //Unk10 = true,
                    //UseTcp = true,
                    Ecbfehfpofj = true,
                };


                MemoryStream stream = new MemoryStream();
                Serializer.Serialize(stream, gateserver);
                string rsp = Convert.ToBase64String(stream.ToArray());
                stream.Close();

                _log.LogInformation("客户端请求: query_gateway");
                return Ok(rsp);
            }
            catch (Exception ex)
            {
                _log.LogError("处理 query_gateway 错误：" + ex);
                return StatusCode((int)HttpStatusCode.InternalServerError, "处理 query_gateway 错误");
            }
        }
    }
}