using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Data.SqlClient;
using System.Configuration;
using AttributeRouting.Web.Http;

//using AttributeRoutingSample.Models;

namespace MvcApplication13.Controllers
{
    public class ParentController : ApiController
    {

        public static string connectionString = ConfigurationManager.ConnectionStrings["LocalSqlServer"].ConnectionString;
        public SqlCommand myCommand = default(SqlCommand);
        public SqlConnection conn = new SqlConnection(connectionString);
        public SqlDataReader reader = default(SqlDataReader);
        public auth access = new auth();
        SqlCommand nestedCommand = default(SqlCommand);
        SqlCommand myCommand2 = default(SqlCommand);
        SqlConnection nestedConn = new SqlConnection(connectionString);
        SqlDataReader nestedReader = default(SqlDataReader);

        [GET("session/{id}/{siteid}/parent/devicelist")]
        public List<string> GetDeviceIdsInGroup(string group_siteid)
        {
            List<string> deviceIds = new List<string>();
            string siteIds = "";
            nestedCommand = new SqlCommand("select siteid_device from oline_device_group_bindings where siteid_group =" + group_siteid + ";", nestedConn);
            try
            {
                nestedConn.Open();
                nestedReader = nestedCommand.ExecuteReader();

                if ((nestedReader.HasRows))
                {
                    while (nestedReader.Read())
                    {
                        siteIds += "'" + nestedReader[0] + "',";
                    }
                }
                nestedReader.Close();
                nestedConn.Close();
            }
            catch (Exception ex)
            {
            }

            if (siteIds.Length > 0)
            {
                siteIds = siteIds.Remove(siteIds.Length - 1, 1);
            }

            nestedCommand = new SqlCommand("select deviceid from oline_devices where siteid in (" + siteIds + ");", nestedConn);
            try
            {
                nestedConn.Open();
                nestedReader = nestedCommand.ExecuteReader();
                if ((nestedReader.HasRows))
                {
                    while (nestedReader.Read())
                    {
                        deviceIds.Add(nestedReader[0].ToString());
                    }
                }
                nestedReader.Close();
                nestedConn.Close();
            }
            catch (Exception ex)
            {
            }
            return deviceIds;
        }

        public string GenerateGroupQuery(List<string> deviceList, string prefix, string suffix)
        {
            string output = "select datediff(second, 25567,SWITCHOFFSET(TODATETIMEOFFSET(epoch, '+00:00'), '+08:00')),ec from (select t.dateinfo, sum(t.entrycount) from (";
            dynamic numDevices = deviceList.Count;
            dynamic index = 0;
            foreach (var deviceid in deviceList)
            {
                output += prefix + deviceid + suffix;
                index += 1;
                if (index < deviceList.Count)
                {
                    output += " union all ";
                }
            }
            output += ")t group by t.dateinfo)as n(epoch,ec)";
            return output;
        }

        public string GenerateGroupQueryExit(List<string> deviceList, string prefix, string suffix)
        {
            string output = "select datediff(second, 25567,SWITCHOFFSET(TODATETIMEOFFSET(epoch, '+00:00'), '+08:00')),ec from (select t.dateinfo, sum(t.exitcount) from (";
            dynamic numDevices = deviceList.Count;
            dynamic index = 0;
            foreach (var deviceid in deviceList)
            {
                output += prefix + deviceid + suffix;
                index += 1;
                if (index < deviceList.Count)
                {
                    output += " union all ";
                }
            }
            output += ")t group by t.dateinfo)as n(epoch,ec)";
            return output;
        }

        public string GenerateGroupQueryCapacity1(List<string> deviceList, string prefix, string suffix, string maxval, DateTime startdt, DateTime enddt)
        {
            string output = "select datain,capacity = Case when capacity < 0 then 0 else capacity end from (select datediff(second, 25567, SWITCHOFFSET(TODATETIMEOFFSET(t1.Dateinfo, '+00:00'), '+08:00')),(SUM((convert(decimal,t2.entrycount))-(convert(decimal,t2.exitcount)))/" + maxval + ")*100 as CAPACITY from (select epoch, ec,ec1 from (select * from (select mi, sum(val) as val,sum(val1) as val1 from";
            dynamic numDevices = deviceList.Count;
            dynamic index = 0;
            var closepar = ")";
            foreach (var deviceid in deviceList)
            {
                output += prefix + deviceid + suffix;
                index += 1;
                if (index < deviceList.Count)
                {
                    output += " union all ";
                    closepar += ")";
                }
            }
            output += ""+closepar+" as w(mi,val,val1) group by mi) as a(mi,val,val1) where mi>= '" + startdt.ToString("yyyy-MM-dd HH:mm:ss") + "' And mi<='" + enddt.ToString("yyyy-MM-dd HH:mm:ss") + "')as n(epoch,ec,ec1)) as t1(Dateinfo, entrycount, exitcount),";
            //where dateinfo>=  And dateinfo<='" + enddt.ToString("yyyy-MM-dd HH:mm:ss")
            return output;
        }

        public string GenerateGroupQueryCapacity2(List<string> deviceList, string prefix, string suffix, string maxval, DateTime startdt, DateTime enddt)
        {
            string output = "(select epoch, ec,ec1 from (select * from (select mi, sum(val) as val,sum(val1) as val1 from ";
            dynamic numDevices = deviceList.Count;
            dynamic index = 0;
            var closepar = ")";
            foreach (var deviceid in deviceList)
            {
                output += prefix + deviceid + suffix;
                index += 1;
                if (index < deviceList.Count)
                {
                    output += " union all ";
                    closepar += ")";
                }
            }
            output += "" + closepar + " as w(mi,val,val1) group by mi) as a(mi,val,val1) where mi>= '" + startdt.ToString("yyyy-MM-dd HH:mm:ss") + "' And mi<='" + enddt.ToString("yyyy-MM-dd HH:mm:ss") + "')as n(epoch,ec,ec1)) as t2(Dateinfo, entrycount, exitcount) where t1.Dateinfo >=t2.Dateinfo group by t1.Dateinfo, t1.exitcount) as T1(datain,CAPACITY) order by t1.datain";
            return output;
        }

        public string GenerateGroupQueryweeklyCapacitymin1(List<string> deviceList, string prefix, string suffix, string maxval, DateTime startdt, DateTime enddt)
        {
            string output = "select top 1 datain,capacity = Case when capacity < 0 then 0 else capacity end from (select datediff(second, 25567, SWITCHOFFSET(TODATETIMEOFFSET (t1.Dateinfo, '+00:00'), '+08:00')),(SUM((convert(decimal,t2.entrycount))-(convert(decimal,t2.exitcount)))/" + maxval + ")*100 as CAPACITY from (select epoch, ec,ec1 from (select * from (select mi, sum(val) as val,sum(val1) as val1 from";
            dynamic numDevices = deviceList.Count;
            dynamic index = 0;
            foreach (var deviceid in deviceList)
            {
                output += prefix + deviceid + suffix;
                index += 1;
                if (index < deviceList.Count)
                {
                    output += " union all ";
                }
            }
            output += ")) as w(mi,val,val1) group by mi) as a(mi,val,val1) where mi>= '" + startdt.ToString("yyyy-MM-dd HH:mm:ss") + "' And mi<='" + enddt.ToString("yyyy-MM-dd HH:mm:ss") + "')as n(epoch,ec,ec1)) as t1(Dateinfo, entrycount, exitcount),";
            //where dateinfo>=  And dateinfo<='" + enddt.ToString("yyyy-MM-dd HH:mm:ss")
            return output;
        }

        public string GenerateGroupQueryweeklyCapacitymin2(List<string> deviceList, string prefix, string suffix, string maxval, DateTime startdt, DateTime enddt)
        {
            string output = "(select epoch, ec,ec1 from (select * from (select mi, sum(val) as val,sum(val1) as val1 from ";
            dynamic numDevices = deviceList.Count;
            dynamic index = 0;
            foreach (var deviceid in deviceList)
            {
                output += prefix + deviceid + suffix;
                index += 1;
                if (index < deviceList.Count)
                {
                    output += " union all ";
                }
            }
            output += ")) as w(mi,val,val1) group by mi) as a(mi,val,val1) where mi>= '" + startdt.ToString("yyyy-MM-dd HH:mm:ss") + "' And mi<='" + enddt.ToString("yyyy-MM-dd HH:mm:ss") + "')as n(epoch,ec,ec1)) as t2(Dateinfo, entrycount, exitcount) where t1.Dateinfo >=t2.Dateinfo group by t1.Dateinfo, t1.exitcount) as T1(datain,CAPACITY) order by capacity";
            return output;
        }

        public string GenerateGroupQueryweeklyCapacitymax1(List<string> deviceList, string prefix, string suffix, string maxval, DateTime startdt, DateTime enddt)
        {
            string output = "select top 1 datain,capacity = Case when capacity < 0 then 0 else capacity end from (select datediff(second, 25567, SWITCHOFFSET(TODATETIMEOFFSET (t1.Dateinfo, '+00:00'), '+08:00')),(SUM((convert(decimal,t2.entrycount))-(convert(decimal,t2.exitcount)))/" + maxval + ")*100 as CAPACITY from (select epoch, ec,ec1 from (select * from (select mi, sum(val) as val,sum(val1) as val1 from";
            dynamic numDevices = deviceList.Count;
            dynamic index = 0;
            foreach (var deviceid in deviceList)
            {
                output += prefix + deviceid + suffix;
                index += 1;
                if (index < deviceList.Count)
                {
                    output += " union all ";
                }
            }
            output += ")) as w(mi,val,val1) group by mi) as a(mi,val,val1) where mi>= '" + startdt.ToString("yyyy-MM-dd HH:mm:ss") + "' And mi<='" + enddt.ToString("yyyy-MM-dd HH:mm:ss") + "')as n(epoch,ec,ec1)) as t1(Dateinfo, entrycount, exitcount),";
            //where dateinfo>=  And dateinfo<='" + enddt.ToString("yyyy-MM-dd HH:mm:ss")
            return output;
        }

        public string GenerateGroupQueryweeklyCapacitymax2(List<string> deviceList, string prefix, string suffix, string maxval, DateTime startdt, DateTime enddt)
        {
            string output = "(select epoch, ec,ec1 from (select * from (select mi, sum(val) as val,sum(val1) as val1 from ";
            dynamic numDevices = deviceList.Count;
            dynamic index = 0;
            foreach (var deviceid in deviceList)
            {
                output += prefix + deviceid + suffix;
                index += 1;
                if (index < deviceList.Count)
                {
                    output += " union all ";
                }
            }
            output += ")) as w(mi,val,val1) group by mi) as a(mi,val,val1) where mi>= '" + startdt.ToString("yyyy-MM-dd HH:mm:ss") + "' And mi<='" + enddt.ToString("yyyy-MM-dd HH:mm:ss") + "')as n(epoch,ec,ec1)) as t2(Dateinfo, entrycount, exitcount) where t1.Dateinfo >=t2.Dateinfo group by t1.Dateinfo, t1.exitcount) as T1(datain,CAPACITY) order by capacity desc";
            return output;
        }

        public string GenerateGroupQueryweeklyCapacityAvg1(List<string> deviceList, string prefix, string suffix, string maxval, DateTime startdt, DateTime enddt)
        {
            string output = "select avg(t1.cap) from (select datain,capacity = Case when capacity < 0 then 0 else capacity end from (select datediff(second, 25567, SWITCHOFFSET(TODATETIMEOFFSET (t1.Dateinfo, '+00:00'), '+08:00')),(SUM((convert(decimal,t2.entrycount))-(convert(decimal,t2.exitcount)))/" + maxval + ")*100 as CAPACITY from (select epoch, ec,ec1 from (select * from (select mi, sum(val) as val,sum(val1) as val1 from";
            dynamic numDevices = deviceList.Count;
            dynamic index = 0;
            foreach (var deviceid in deviceList)
            {
                output += prefix + deviceid + suffix;
                index += 1;
                if (index < deviceList.Count)
                {
                    output += " union all ";
                }
            }
            output += ")) as w(mi,val,val1) group by mi) as a(mi,val,val1) where mi>= '" + startdt.ToString("yyyy-MM-dd HH:mm:ss") + "' And mi<='" + enddt.ToString("yyyy-MM-dd HH:mm:ss") + "')as n(epoch,ec,ec1)) as t1(Dateinfo, entrycount, exitcount),";
            //where dateinfo>=  And dateinfo<='" + enddt.ToString("yyyy-MM-dd HH:mm:ss")
            return output;
        }

        public string GenerateGroupQueryweeklyCapacityAvg2(List<string> deviceList, string prefix, string suffix, string maxval, DateTime startdt, DateTime enddt)
        {
            string output = "(select epoch, ec,ec1 from (select * from (select mi, sum(val) as val,sum(val1) as val1 from ";
            dynamic numDevices = deviceList.Count;
            dynamic index = 0;
            foreach (var deviceid in deviceList)
            {
                output += prefix + deviceid + suffix;
                index += 1;
                if (index < deviceList.Count)
                {
                    output += " union all ";
                }
            }
            output += ")) as w(mi,val,val1) group by mi) as a(mi,val,val1) where mi>= '" + startdt.ToString("yyyy-MM-dd HH:mm:ss") + "' And mi<='" + enddt.ToString("yyyy-MM-dd HH:mm:ss") + "')as n(epoch,ec,ec1)) as t2(Dateinfo, entrycount, exitcount) where t1.Dateinfo >=t2.Dateinfo group by t1.Dateinfo, t1.exitcount)as T1(datain,capacity))as t1(dat,cap)";
            return output;
        }

        public List<string> GetDeviceIdsFromGroupId(string groupid)
        {
            List<string> deviceIds = new List<string>();
            groupid = "";
            nestedCommand = new SqlCommand("select deviceid, isgroup, siteid from oline_devices where groupid=" + groupid + "", nestedConn);
            try
            {
                nestedConn.Open();
                nestedReader = nestedCommand.ExecuteReader();
                if ((nestedReader.HasRows))
                {
                    while (nestedReader.Read())
                    {
                        deviceIds.Add(nestedReader[0].ToString());
                    }
                }
                nestedReader.Close();
                nestedConn.Close();
            }
            catch (Exception ex)
            {
            }
            return deviceIds;
        }

        // GET api/<controller>/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        public void Post([FromBody]string value)
        {
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}