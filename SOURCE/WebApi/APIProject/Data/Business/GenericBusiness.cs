﻿using Data.DB;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Business
{
    public class GenericBusiness
    {
        public MrFishEntities context;
        public MrFishEntities cnn;
        public ILog log = log4net.LogManager.GetLogger(typeof(GenericBusiness));
        public ResponseBusiness rpBus = new ResponseBusiness();
        public GenericBusiness(MrFishEntities context = null)
        {
            if (context == null)
            {
                this.context = new MrFishEntities();
            }
            cnn = this.context;
            //this.context.Configuration.AutoDetectChangesEnabled = false;
            //this.context.Configuration.ValidateOnSaveEnabled = false;
            //this.context.Configuration.LazyLoadingEnabled = false;
        }
    }
}
