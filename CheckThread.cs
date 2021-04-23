﻿using BotBiliBili.Config;
using BotBiliBili.PicGen;
using BotBiliBili.Utils;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BotBiliBili
{
    class CheckThread
    {
        private static Thread thread;
        private static bool IsRun;
        public static void Start()
        {
            thread = new(Task);
            IsRun = true;
            thread.Start();
        }

        private static void Task()
        {
            while (IsRun)
            {
                try
                {
                    bool save = false;
                    foreach (var item in ConfigUtils.Subscribes)
                    {
                        long group = item.Key;
                        SubscribeObj obj = item.Value;
                        foreach (var item1 in obj.Uids)
                        {
                            try
                            {
                                var obj1 = HttpUtils.GetDynamicUid(item1);
                                Thread.Sleep(ConfigUtils.Config.CheckDelay);
                                if (!IsRun)
                                    return;
                                if (obj1 == null)
                                    continue;
                                var obj2 = obj1["data"]["cards"] as JArray;
                                if (obj2.Count == 0)
                                    continue;
                                var obj3 = obj2[0]["desc"]["dynamic_id"].ToString();

                                if (ConfigUtils.UidLast.Dynamic.ContainsKey(item1))
                                {
                                    if (ConfigUtils.UidLast.Dynamic[item1] == obj3)
                                        continue;
                                    else
                                        ConfigUtils.UidLast.Dynamic[item1] = obj3;
                                    save = true;
                                }
                                else
                                {
                                    ConfigUtils.UidLast.Dynamic.Add(item1, obj3);
                                    save = true;
                                }
                                
                                var data1 = HttpUtils.GetDynamic(obj3);
                                if (data1 == null)
                                {
                                    continue;
                                }
                                string temp1 = DynamicPicGen.Gen(data1);
                                Program.Log($"已生成{temp1}");
                                Program.SendGroupImage(temp1, group);
                            }
                            catch (Exception e)
                            {
                                Program.Error(e);
                            }
                        }
                        foreach (var item1 in obj.Lives)
                        {
                            try
                            {
                                var obj1 = HttpUtils.GetLiveUID(item1);
                                Thread.Sleep(ConfigUtils.Config.CheckDelay);
                                if (!IsRun)
                                    return;
                                if (obj1 == null)
                                    continue;
                                var obj2 = (int)obj1["data"]["roomStatus"];
                                if (obj2 == 0)
                                    continue;
                                bool obj3 = (int)obj1["data"]["liveStatus"] == 1;

                                if (ConfigUtils.UidLast.Live.ContainsKey(item1))
                                {
                                    if (ConfigUtils.UidLast.Live[item1] == obj3)
                                        continue;
                                    else
                                        ConfigUtils.UidLast.Live[item1] = obj3;
                                    save = true;
                                }
                                else
                                {
                                    ConfigUtils.UidLast.Live.Add(item1, obj3);
                                    save = true;
                                }
                                if (!obj3)
                                    continue;

                                var data1 = HttpUtils.GetLive(obj1["data"]["roomid"].ToString());
                                if (data1 == null)
                                {
                                    continue;
                                }
                                string temp1 = LivePicGen.Gen(data1);
                                Program.Log($"已生成{temp1}");
                                Program.SendGroupImage(temp1, group);
                            }
                            catch (Exception e)
                            {
                                Program.Error(e);
                            }
                        }
                    }
                    if (save)
                    {
                        ConfigUtils.SaveTemp();
                    }
                }
                catch (Exception e)
                {
                    Program.Error(e);
                }
            }
        }

        public static void Stop()
        {
            IsRun = false;
        }
    }
}
