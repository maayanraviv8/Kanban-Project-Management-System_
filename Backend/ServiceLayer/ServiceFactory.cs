using IntroSE.Kanban.Backend.BusinessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using log4net.Config;
using System.Reflection;
using log4net.Repository;
using IntroSE.Kanban.Backend.DataAccessLayer;


namespace IntroSE.Kanban.Backend.ServiceLayer
{
    public class ServiceFactory
    {
        internal ILoggerRepository LogRepository;
        private readonly ILog log;
        public UserService us;
        public BoardService bs;
        public TaskService ts;
        private UserFacade uf;
        internal BoardFacade bf;
        internal TaskFacade tf;
        internal UserController uc;
        internal BoardController bc;
        internal TaskController tc;


        public ServiceFactory()
        {

            LogRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(LogRepository, new System.IO.FileInfo("log4net.config"));

            log = LogManager.GetLogger("SYSTEM LOGS");
            log.Info("ServiceFactory initialized successfully.");

         

            uf = new UserFacade(log);
            us = new UserService(uf,log);
            bf = new BoardFacade(uf,log, tf);
            bs = new BoardService(bf,log);
            tf = new TaskFacade(uf,bf,log);
            ts = new TaskService(tf,log);



        }


    }
}
