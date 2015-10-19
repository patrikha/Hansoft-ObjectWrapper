using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HPMSdk;

namespace Hansoft.ObjectWrapper
{
    /// <summary>
    /// Represents a scheduled task in the Schedule view of Hansoft.
    /// </summary>
    public class ScheduledTask : Task
    {
        internal static ScheduledTask GetScheduledTask(HPMUniqueID uniqueID, HPMUniqueID uniqueTaskID)
        {
            return new ScheduledTask(uniqueID, uniqueTaskID);
        }

        private ScheduledTask(HPMUniqueID uniqueID, HPMUniqueID uniqueTaskID)
            : base(uniqueID, uniqueTaskID)
        {
        }

        /// <summary>
        /// The project view that this scheduled task belongs to.
        /// </summary>
        public override ProjectView ProjectView
        {
            get
            {
                return Project.Schedule;
            }
        }


        public Tuple<DateTime, DateTime> TimeZone
        {
            get
            {
                HPMTaskTimeZones tzData = Session.TaskGetTimeZones(UniqueTaskID);
                return new Tuple<DateTime, DateTime>(HPMUtilities.FromHPMDateTime(tzData.m_Zones[0].m_Start), HPMUtilities.FromHPMDateTime(tzData.m_Zones[0].m_End));
            }
            set
            {
                if (!TimeZone.Equals(value))
                {
                    HPMTaskTimeZonesZone tz = new HPMTaskTimeZonesZone();
                    tz.m_Start = HPMUtilities.HPMDateTime(value.Item1);
                    tz.m_End = HPMUtilities.HPMDateTime(value.Item2);
                    HPMTaskTimeZones tzData = new HPMTaskTimeZones();
                    tzData.m_Zones = new HPMTaskTimeZonesZone[] { tz };
                    Session.TaskSetTimeZones(UniqueTaskID, tzData, true);
                }
            }
        }

        public DateTime Start
        {
            get { return TimeZone.Item1; }
            set
            {
                var tz = TimeZone;
                if (!tz.Item1.Equals(value))
                    TimeZone = new Tuple<DateTime, DateTime>(value, tz.Item2);
            }
        }

        public DateTime Finish
        {
            get { return TimeZone.Item2; }
            set
            {
                var tz = TimeZone;
                if (!tz.Item2.Equals(value))
                    TimeZone = new Tuple<DateTime, DateTime>(tz.Item1, value);
            }
        }

        /// <summary>
        /// The Duaration of the task
        /// </summary>
        public int Duration
        {
            get
            {
                return Session.TaskGetDuration(UniqueTaskID);
            }
        }

        /// <summary>
        /// The assignment percentage of a particular user that is assigned to this scheduled task.
        /// </summary>
        /// <param name="user">The name of the user.</param>
        /// <returns>The assignment percentage.</returns>
        public int GetAssignmentPercentage(User user)
        {
            return TaskHelper.GetAssignmentPercentage(this, user);
        }

        /// <summary>
        /// True if this scheduled task is assigned, False otherwise.
        /// </summary>
        public bool IsAssigned
        {
            get
            {
                return TaskHelper.IsAssigned(this);
            }
        }

        /// <summary>
        /// Not applicable for scheduled tasks
        /// </summary>
        public override HansoftEnumValue Priority
        {
            get
            {
                return new HansoftEnumValue(MainProjectID, EHPMProjectDefaultColumn.SprintPriority, EHPMTaskAgilePriorityCategory.None, (int)EHPMTaskAgilePriorityCategory.None);
            }
            set
            {
            }
        }
    }
}
