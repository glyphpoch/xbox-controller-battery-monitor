namespace Xbca.Models
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents a gaming controller.
    /// </summary>
    public class Controller
    {
        /// <summary>
        /// Gets or sets the controller id.
        /// </summary>
        public int Id { get; set; } = -1;

        /// <summary>
        /// Gets or sets controllers unique or non roamable id.
        /// </summary>
        public string UniqueId { get; set; }

        /// <summary>
        /// Gets or sets controllers battery type.
        /// </summary>
        public byte BatteryType { get; set; }

        /// <summary>
        /// Gets or sets controller battery value.
        /// </summary>
        public byte BatteryValue { get; set; }

        /// <summary>
        /// Gets or sets a note.
        /// </summary>
        public byte Note { get; set; }

        /// <summary>
        /// Gets or sets the battery status string.
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets the controller name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets battery report data.
        /// </summary>
        public string BatteryReportData { get; set; }

        /// <summary>
        /// Constructs the notification string with controller name, id and battery value.
        /// </summary>
        /// <returns>Notification string.</returns>
        public string GetNotificationString()
        {
            return string.Format("Name: {0}\nId:{1}\nBattery Value:{2}", this.Name, this.Id, Constants.GetEnumDescription((Constants.BatteryLevel)this.BatteryValue));
        }
    }
}
