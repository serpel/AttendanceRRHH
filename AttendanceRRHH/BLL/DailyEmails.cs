using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AttendanceRRHH.BLL
{
    public class DailyEmails
    {
        private ApplicationException context;

        public DailyEmails():this(new ApplicationException()) { }

        public DailyEmails(ApplicationException context)
        {
            this.context = context;
        }

        public void SendEmailReporToManager()
        {
            
        }
    }
}