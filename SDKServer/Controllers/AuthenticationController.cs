using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SDKServer.Models;

namespace SDKServer.Controllers
{
    [ApiController]
    [Route("/")]
    public class AuthenticationController : ControllerBase
    {
        private readonly SDKDbContext _db;
        private readonly SDKSettings _settings;

        private static readonly object _generateUidLock = new object();

        public AuthenticationController(SDKDbContext db, IOptions<SDKSettings> settings)
        {
            _db = db;
            _settings = settings.Value;
        }

        [HttpPost("{game_biz}/mdk/shield/api/login")]
        public async Task<JsonResult> LoginWithPassword([FromBody] LoginRequest req)
        {
            if (req == null)
            {
                return new JsonResult(new LoginV2Respond
                {
                    retcode = -202,
                    message = "登录数据无效"
                });
            }

            var user = await _db.Users.FirstOrDefaultAsync(u => u.username == req.Account);

            if (user == null)
            {
                if (!_settings.AutoRegister)
                {
                    return new JsonResult(new LoginV2Respond
                    {
                        retcode = -201,
                        message = "用户不存在"
                    });
                }

                if (!Regex.IsMatch(req.Account, @"^[A-Za-z0-9_]*$"))
                {
                    return new JsonResult(new LoginV2Respond
                    {
                        retcode = -201,
                        message = "用户名格式无效；应由 [A-Za-z0-9_] 组成"
                    });
                }

                var newUid = GenerateNewUid(_db);
                user = new User
                {
                    uid = newUid,
                    username = req.Account,
                    password = req.Password,
                    gameUid = 0,
                    SessionToken = Utils.GenerateToken(newUid),
                };
                _db.Users.Add(user);
                await _db.SaveChangesAsync();
            }
            else
            {
                user.SessionToken = Utils.GenerateToken(user.uid);
                await _db.SaveChangesAsync();
            }

            return new JsonResult(new LoginRespond
            {
                retcode = 0,
                message = "OK",
                data = new LoginRespond.VerifyData
                {
                    account = new LoginRespond.VerifyAccountData
                    {
                        name = user.username,
                        email = user.username + "@cytsr.com",
                        country = "CN",
                        is_email_verify = "1",
                        token = user.SessionToken,
                        uid = user.uid,
                        area_code = "CN"
                    },
                    device_grant_required = false,
                    realperson_required = false,
                    safe_mobile_required = false,
                    realname_operation = "Cyt"
                }
            });
        }

        [HttpPost("{game_biz}/combo/granter/login/v2/login")]
        public async Task<JsonResult> LoginV2([FromBody] LoginV2Request req)
        {
            if (req == null)
            {
                return new JsonResult(new LoginV2Respond
                {
                    retcode = -202,
                    message = "登录数据无效"
                });
            }

            var data = JsonSerializer.Deserialize<LoginV2Request.Data>(req.data);
            if (data == null)
            {
                return new JsonResult(new LoginV2Respond
                {
                    retcode = -202,
                    message = "登录数据无效"
                });
            }

            var user = await _db.Users.FirstOrDefaultAsync(u => u.SessionToken == data.token);

            if (user == null)
            {
                return new JsonResult(new LoginV2Respond
                {
                    retcode = -201,
                    message = "游戏帐号缓存信息错误"
                });
            }

            user.comboToken = Utils.GenerateToken(user.uid);
            await _db.SaveChangesAsync();

            return new JsonResult(new LoginV2Respond
            {
                retcode = 0,
                message = "OK",
                data = new LoginV2Respond.Data
                {
                    account_type = 1,
                    combo_token = user.comboToken,
                    combo_id = null,
                    open_id = user.uid,
                    heartbeat = false,
                    data = "{\"guest\":false}",
                    fatigue_remind = null
                }
            });
        }

        [HttpPost("{game_biz}/mdk/shield/api/verify")]
        public async Task<JsonResult> LoginWithSessionToken([FromBody] TokenLoginRequst req)
        {
            if (req == null)
            {
                return new JsonResult(new LoginV2Respond
                {
                    retcode = -202,
                    message = "登录数据无效"
                });
            }

            var user = await _db.Users.FirstOrDefaultAsync(u => u.SessionToken == req.token);

            if (user == null)
            {
                return new JsonResult(new LoginRespond
                {
                    retcode = -201,
                    message = "游戏账号缓存信息错误"
                });
            }

            return new JsonResult(new LoginRespond
            {
                retcode = 0,
                message = "OK",
                data = new LoginRespond.VerifyData
                {
                    account = new LoginRespond.VerifyAccountData
                    {
                        email = user.username,
                        country = "CN",
                        is_email_verify = "1",
                        token = user.SessionToken,
                        uid = user.uid
                    }
                }
            });
        }

        [HttpPost("/account/risky/api/check")]
        public ContentResult HandleRiskyApiCheckPost()
        {
            var rsp = "{\"retcode\":0,\"message\":\"OK\",\"data\":{\"id\":\"none\",\"action\":\"ACTION_NONE\",\"geetest\":null}}";
            return new ContentResult()
            {
                ContentType = "application/json",
                Content = rsp,
            };
        }

        public static string GenerateNewUid(SDKDbContext _db)
        {
            lock (_generateUidLock)
            {
                var lastUser = _db.Users.OrderByDescending(u => u.uid).FirstOrDefault();
                if (lastUser == null)
                    return "1";

                return (int.Parse(lastUser.uid) + 1).ToString();
            }
        }
    }
}