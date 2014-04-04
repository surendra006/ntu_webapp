using System;
using System.Collections.Generic;
using AttributeRouting.Web.Http;
using System.Data.SqlClient;
//using AttributeRoutingSample.Models;


namespace MvcApplication13.Controllers
{
    public class EntryController : ParentController
    {
        [GET("session/{id}/{siteid}/entry/{resolution}/{freq}/{startdt}/{enddt}")]
        public string getentry(String id, int siteid, String resolution, int freq, DateTime startdt, DateTime enddt)
        {
            //string query = "select DATEDIFF(SECOND, 25567, SWITCHOFFSET(TODATETIMEOFFSET(dateInfo, '+00:00'), '+08:00')), entryCount from " + tablePrefix + deviceId.Key + tableSuffix + " where dateInfo > CONVERT(datetimeoffset, '" + startdt + "T00:08:00+08:00', 127) and dateInfo <= CONVERT(datetimeoffset, '" + enddt + "T00:08:00+08:00', 127)";
            string uid = "";
            string gid = "";
            int year = 525600 * freq;
            int month = 43200 * freq;
            int fortnight = 20160 * freq;
            int week = 10080 * freq;
            int day = 1440 * freq;
            int hour = 60 * freq;
            int min = freq;
            int count = 0;
            if (id == null)
            {
                //return new Dictionary<string, string>();
                return "";
            }
            if (access.Authenticate(id, ref gid, ref uid) == false)
            {
               // return new Dictionary<string, string>();
                return "";
            }
            
            //Continue For
            string output = "";
            var deviceid = "";
            var isgroup = "";
            startdt = startdt.AddHours(-8);
            enddt = enddt.AddHours(-8);
            
            myCommand = new SqlCommand("select deviceid, isgroup from oline_devices where siteID = " + siteid, conn);
            try
            {
                conn.Open();
                reader = myCommand.ExecuteReader();
                if ((reader.HasRows))
                {
                    while (reader.Read())
                    {
                        deviceid = reader["deviceid"].ToString();
                        isgroup = reader["isgroup"].ToString();
                    }
                }
                reader.Close();
                conn.Close();
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            Console.WriteLine(deviceid + " " + isgroup);
            string query = "";
            Console.WriteLine(startdt.ToString("yyyy-MM-ddTHH:mm:ss"));

            if (isgroup.ToLower() == "true")
            {
                List<string> group_deviceids = GetDeviceIdsInGroup(siteid.ToString());

                if (resolution.Equals("year"))
                {
                    query = GenerateGroupQuery(group_deviceids, "(select * from (select dateinfo, sum(entrycount) as val from (select dateadd(minute, floor(datediff(minute, '20000101', dateinfo)/" + year + ")*" + year + ",'20000101'),entrycount from " + "oline_", "_h" + " )as w(dateinfo,entrycount) group by dateinfo) as a(dateinfo,entrycount) where dateinfo>= '" + startdt.ToString("yyyy-MM-dd HH:mm:ss") + "' And dateinfo<='" + enddt.ToString("yyyy-MM-dd HH:mm:ss") + "')");

                }
                else if (resolution.Equals("month"))
                {
                    query = GenerateGroupQuery(group_deviceids, "(select * from (select dateinfo, sum(entrycount) as val from (select dateadd(minute, floor(datediff(minute, '20000101', dateinfo)/" + month + ")*" + month + ",'20000101'),entrycount from " + "oline_", "_h" + " )as w(dateinfo,entrycount) group by dateinfo) as a(dateinfo,entrycount) where dateinfo>= '" + startdt.ToString("yyyy-MM-dd HH:mm:ss") + "' And dateinfo<='" + enddt.ToString("yyyy-MM-dd HH:mm:ss") + "')");

                }
                else if (resolution.Equals("fortnight"))
                {
                    query = GenerateGroupQuery(group_deviceids, "(select * from (select dateinfo, sum(entrycount) as val from (select dateadd(minute, floor(datediff(minute, '20000101', dateinfo)/" + fortnight + ")*" + fortnight + ",'20000101'),entrycount from " + "oline_", "_h" + " )as w(dateinfo,entrycount) group by dateinfo) as a(dateinfo,entrycount) where dateinfo>= '" + startdt.ToString("yyyy-MM-dd HH:mm:ss") + "' And dateinfo<='" + enddt.ToString("yyyy-MM-dd HH:mm:ss") + "')");

                }
                else if (resolution.Equals("week"))
                {
                    query = GenerateGroupQuery(group_deviceids, "(select * from (select dateinfo, sum(entrycount) as val from (select dateadd(minute, floor(datediff(minute, '20000101', dateinfo)/" + week + ")*" + week + ",'20000101'),entrycount from " + "oline_", "_h" + " )as w(dateinfo,entrycount) group by dateinfo) as a(dateinfo,entrycount) where dateinfo>= '" + startdt.ToString("yyyy-MM-dd HH:mm:ss") + "' And dateinfo<='" + enddt.ToString("yyyy-MM-dd HH:mm:ss") + "')");

                }
                else if (resolution.Equals("day"))
                {
                    query = GenerateGroupQuery(group_deviceids, "(select * from (select dateinfo, sum(entrycount) as val from (select dateadd(minute, floor(datediff(minute, '20000101', dateinfo)/" + day+ ")*" + day + ",'20000101'),entrycount from " + "oline_", "_h" + " )as w(dateinfo,entrycount) group by dateinfo) as a(dateinfo,entrycount) where dateinfo>= '" + startdt.ToString("yyyy-MM-dd HH:mm:ss") + "' And dateinfo<='" + enddt.ToString("yyyy-MM-dd HH:mm:ss") + "')");

                }
                else if (resolution.Equals("hours"))
                {
                    query = GenerateGroupQuery(group_deviceids, "(select * from (select dateinfo, sum(entrycount) as val from (select dateadd(minute, floor(datediff(minute, '20000101', dateinfo)/" + hour + ")*" + hour + ",'20000101'),entrycount from " + "oline_", "_h" + " )as w(dateinfo,entrycount) group by dateinfo) as a(dateinfo,entrycount) where dateinfo>= '" + startdt.ToString("yyyy-MM-dd HH:mm:ss") + "' And dateinfo<='" + enddt.ToString("yyyy-MM-dd HH:mm:ss") + "')");
                }
                else if (resolution.Equals("minutes"))
                {
                    query = GenerateGroupQuery(group_deviceids, "(select * from (select dateinfo, sum(entrycount) as val from (select dateadd(minute, floor(datediff(minute, '20000101', dateinfo)/" + min + ")*" + min + ",'20000101'),entrycount from " + "oline_", "_h" + " )as w(dateinfo,entrycount) group by dateinfo) as a(dateinfo,entrycount) where dateinfo>= '" + startdt.ToString("yyyy-MM-dd HH:mm:ss") + "' And dateinfo<='" + enddt.ToString("yyyy-MM-dd HH:mm:ss") + "')");
                }

            }
            else
            {
                if (resolution.Equals("year"))
                {
                    // query = "select * from (select mi, sum(val) as val from (select dateadd(minute, floor(datediff(minute, '20000101', dateinfo)/" + year + ")*" + year + ",'20000101'),entrycount from " + "oline_" + deviceid + "_h" + " )as w(mi,val) group by mi) as a(mi,val) where mi>= '" + startdt.ToString("yyyy-MM-dd") + "' And mi<='" + enddt.ToString("yyyy-MM-dd") + "' order by mi";
                    query = "select datediff(second, 25567, SWITCHOFFSET(TODATETIMEOFFSET(epoch, '+00:00'), '+08:00')), ec from (select * from (select mi, sum(val) as val from (select dateadd(minute, floor(datediff(minute, '20000101', dateinfo)/" + year + ")*" + year + ",'20000101'),entrycount from " + "oline_" + deviceid + "_h" + " )as w(mi,val) group by mi) as a(mi,val) where mi>= '" + startdt.ToString("yyyy-MM-dd HH:mm:ss") + "' And mi<='" + enddt.ToString("yyyy-MM-dd HH:mm:ss") + "')as n(epoch,ec) order by epoch";
                }
                else if (resolution.Equals("month"))
                {
                    query = "select datediff(second, 25567, SWITCHOFFSET(TODATETIMEOFFSET(epoch, '+00:00'), '+08:00')), ec from (select * from (select mi, sum(val) as val from (select dateadd(minute, floor(datediff(minute, '20000101', dateinfo)/" + month + ")*" + month + ",'20000101'),entrycount from " + "oline_" + deviceid + "_h" + " )as w(mi,val) group by mi) as a(mi,val) where mi>= '" + startdt.ToString("yyyy-MM-dd HH:mm:ss") + "' And mi<='" + enddt.ToString("yyyy-MM-dd HH:mm:ss") + "')as n(epoch,ec)  order by epoch";
                }
                else if (resolution.Equals("fortnight"))
                {
                    query = "select datediff(second, 25567, SWITCHOFFSET(TODATETIMEOFFSET(epoch, '+00:00'), '+08:00')), ec from (select * from (select mi, sum(val) as val from (select dateadd(minute, floor(datediff(minute, '20000101', dateinfo)/" + fortnight + ")*" + fortnight + ",'20000101'),entrycount from " + "oline_" + deviceid + "_h" + " )as w(mi,val) group by mi) as a(mi,val) where mi>= '" + startdt.ToString("yyyy-MM-dd HH:mm:ss") + "' And mi<='" + enddt.ToString("yyyy-MM-dd HH:mm:ss") + "')as n(epoch,ec)  order by epoch";
                }

                else if (resolution.Equals("week"))
                {
                    query = "select datediff(second, 25567, SWITCHOFFSET(TODATETIMEOFFSET(epoch, '+00:00'), '+08:00')), ec from (select * from (select mi, sum(val) as val from (select dateadd(minute, floor(datediff(minute, '20000101', dateinfo)/" + week + ")*" + week + ",'20000101'),entrycount from " + "oline_" + deviceid + "_h" + " )as w(mi,val) group by mi) as a(mi,val) where mi>= '" + startdt.ToString("yyyy-MM-dd HH:mm:ss") + "' And mi<='" + enddt.ToString("yyyy-MM-dd HH:mm:ss") + "')as n(epoch,ec)  order by epoch";
                }

                else if (resolution.Equals("day"))
                {
                    // query = "select * from (select mi, sum(val) as val from (select dateadd(minute, floor(datediff(minute, '20000101', dateinfo)/" + day + ")*" + day + ",'20000101'),entrycount from " + "oline_" + deviceid + "_h" + " )as w(mi,val) group by mi) as a(mi,val) where mi>= '" + startdt.ToString("yyyy-MM-dd") + "' And mi<='" + enddt.ToString("yyyy-MM-dd") + "' order by mi";
                    query = "select datediff(second, 25567, SWITCHOFFSET(TODATETIMEOFFSET(epoch, '+00:00'), '+08:00')), ec from (select * from (select mi, sum(val) as val from (select dateadd(minute, floor(datediff(minute, '20000101', dateinfo)/" + day + ")*" + day + ",'20000101'),entrycount from " + "oline_" + deviceid + "_h" + " )as w(mi,val) group by mi) as a(mi,val) where mi>= '" + startdt.ToString("yyyy-MM-dd HH:mm:ss") + "' And mi<='" + enddt.ToString("yyyy-MM-dd HH:mm:ss") + "')as n(epoch,ec) order by epoch";
                }
                else if (resolution.Equals("hours"))
                {
                    query = "select datediff(second, 25567, SWITCHOFFSET(TODATETIMEOFFSET(epoch, '+00:00'), '+08:00')), ec from (select * from (select mi, sum(val) as val from (select dateadd(minute, floor(datediff(minute, '20000101', dateinfo)/" + hour + ")*" + hour + ",'20000101'),entrycount from " + "oline_" + deviceid + "_h" + " )as w(mi,val) group by mi) as a(mi,val) where mi>= '" + startdt.ToString("yyyy-MM-dd HH:mm:ss") + "' And mi<='" + enddt.ToString("yyyy-MM-dd HH:mm:ss") + "')as n(epoch,ec)  order by epoch";
                }
                else if (resolution.Equals("minutes"))
                {
                    query = "select datediff(second, 25567, SWITCHOFFSET(TODATETIMEOFFSET(epoch, '+00:00'), '+08:00')), ec from (select * from (select mi, sum(val) as val from (select dateadd(minute, floor(datediff(minute, '20000101', dateinfo)/" + min + ")*" + min + ",'20000101'),entrycount from " + "oline_" + deviceid + "_h" + " )as w(mi,val) group by mi) as a(mi,val) where mi>= '" + startdt.ToString("yyyy-MM-dd HH:mm:ss") + "' And mi<='" + enddt.ToString("yyyy-MM-dd HH:mm:ss") + "')as n(epoch,ec) order by epoch";
                }
            }
            myCommand = new SqlCommand(query, conn);
            var resultDict = new Dictionary<string, string>();
            try
            {
                conn.Open();
                reader = myCommand.ExecuteReader();
                if ((reader.HasRows))
                {
                    while (reader.Read())
                    {
                        output += "[" + reader[0].ToString() + "000," + reader[1].ToString() + "],";
                        resultDict.Add(reader[0].ToString() + "000", reader[1].ToString());
                    }
                }
                reader.Close();
                conn.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            output = "[" + output + "]";
            Console.Write(output);
            //Console.Read();
            //return output;
            return output;
        }
    }
}