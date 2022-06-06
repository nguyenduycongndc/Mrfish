using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.IO;
using Data.Business;
using log4net;
using Quartz;
using Quartz.Impl;
using Data.Utils;

namespace APIProject.Job
{
    public class Jobclass : IJob
    {
        /// <summary>
        /// Hàm thực hiện trong tiến trình
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        /// 

        //Tiến trình đăng bài viết của các bài viết đang chờ
        public async Task CheckTaskService()
        {
            System.Diagnostics.Debug.WriteLine("job1");
            ILog log = log4net.LogManager.GetLogger(typeof(Jobclass));
            OrderBusiness orderBus = new OrderBusiness();
            orderBus.RemoveOrderNotPaidProcedure();
            log.Info("Schuder Task  start ");
        }
        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                await Task.WhenAll(CheckTaskService());
            }
            catch
            {
                return;
            }
        }
    }
}