using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AttendanceRRHH.DAL;
using AttendanceRRHH.Models;

namespace AttendanceRRHH.BLL
{
    public class Utils
    {
        private static Utils instance;
        private Utils()
        {

        }

        public static Utils Instance
        {
            get
            {
                if(instance == null)
                {
                    instance = new Utils();
                }

                return instance;
            }
        }

        public IList<ShiftTime> CreateShiftTime(int shiftId, int number)
        {
            IList<ShiftTime> list = new List<ShiftTime>();

            for(int i = 1; i <= number; i++)
            {
                ShiftTime time = new ShiftTime()
                {
                    ShiftId = shiftId,
                    DayNumber = i,
                    StartTime = TimeSpan.Zero,
                    EndTime = TimeSpan.Zero,
                    IsLaborDay = true,
                    HasLunchTime = false,
                    LunchStartTime = TimeSpan.Zero,
                    LunchEndTime = TimeSpan.Zero,
                    InsertedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    IsActive = true                    
                };

                list.Add(time);
            }
            return list;
        }
    }
}