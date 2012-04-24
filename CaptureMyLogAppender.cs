using System;
using System.Linq;
using CaptureMyLog.Framework;
using log4net.Appender;
using log4net.Core;

namespace CaptureMyLog.log4net
{
    public sealed class CaptureMyLogAppender : AppenderSkeleton
    {
        #region Constructors

        public CaptureMyLogAppender()
        {
            this.ApplicationKey = Guid.NewGuid().ToString();
            this.OwnerKey = Guid.NewGuid().ToString();
            this.DeviceKey = Environment.MachineName;
            this.IsTakingPictureEnabled = false;
        }

        public CaptureMyLogAppender(string applicationKey, string deviceKey, string ownerKey, bool isTakingPictureEnabled)
        {
            if (string.IsNullOrWhiteSpace(applicationKey))
                throw new ArgumentNullException("The parameter applicationKey cannot be null nor empty!");

            if (string.IsNullOrWhiteSpace(deviceKey))
                throw new ArgumentNullException("The parameter deviceKey cannot be null nor empty!");

            if (string.IsNullOrWhiteSpace(ownerKey))
                throw new ArgumentNullException("The parameter ownerKey cannot be null nor empty!");

            this.ApplicationKey = applicationKey;
            this.DeviceKey = deviceKey;
            this.OwnerKey = ownerKey;
            this.IsTakingPictureEnabled = isTakingPictureEnabled;
        }

        #endregion Constructors

        #region Properties

        public string ApplicationKey { get; set; }
        public string OwnerKey { get; set; }
        public string DeviceKey { get; set; }
        public bool IsTakingPictureEnabled { get; set; }

        #endregion Properties

        #region Overriden Members

        protected override void Append(LoggingEvent[] loggingEvents)
        {
            if (loggingEvents.Length == 0)
                return;
            else if (loggingEvents.Length == 1)
                this.Append(loggingEvents[0]);
            else
                loggingEvents.AsParallel().ForAll(loggingEvent => this.Append(loggingEvent));
        }

        protected override void Append(LoggingEvent loggingEvent)
        {
            byte[] picture = null;
            if (this.IsTakingPictureEnabled)
                picture = Helpers.CaptureScreenAsJpeg();

            //LogDataWrapper.LogNewDataAsync(this.ApplicationKey, this.DeviceKey, this.OwnerKey, loggingEvent.RenderedMessage, loggingEvent.Level.DisplayName, picture);
            LogDataWrapper.LogNewData(this.ApplicationKey, this.DeviceKey, this.OwnerKey, loggingEvent.RenderedMessage, loggingEvent.Level.DisplayName, picture);
        }

        protected override bool RequiresLayout
        {
            get
            {
                return false;
            }
        }

        #endregion Overriden Members
    }
}
