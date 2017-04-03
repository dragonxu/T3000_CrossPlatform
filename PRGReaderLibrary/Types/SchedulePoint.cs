namespace PRGReaderLibrary
{
    using System;
    using System.Collections.Generic;

    public class SchedulePoint : BasePoint
    {
        public Control Control { get; set; }
        public AutoManual AutoManual { get; set; }
        public Control Override1Control { get; set; }
        public Control Override2Control { get; set; }
        public int Off { get; set; }
        public int Unused { get; set; }
        public T3000Point Override1Point { get; set; }
        public T3000Point Override2Point { get; set; }

        public SchedulePoint(string description = "", string label = "",
            FileVersion version = FileVersion.Current)
            : base(description, label, version)
        { }

        #region Binary data

        /// <summary>
        /// FileVersion.Current - 42 bytes
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="offset"></param>
        /// <param name="version"></param>
        public SchedulePoint(byte[] bytes, int offset = 0,
            FileVersion version = FileVersion.Current)
            : base(bytes, offset, version)
        {
            switch (FileVersion)
            {
                case FileVersion.Current:
                    Control = (Control)bytes.ToByte(30 + offset);
                    AutoManual = (AutoManual)bytes.ToByte(31 + offset);
                    Override1Control = (Control)bytes.ToByte(32 + offset);
                    Override2Control = (Control)bytes.ToByte(33 + offset);
                    Off = bytes.ToByte(34 + offset);
                    Unused = bytes.ToByte(35 + offset);
                    Override1Point = new T3000Point(bytes.ToBytes(36 + offset, 3), 0, FileVersion);
                    Override2Point = new T3000Point(bytes.ToBytes(39 + offset, 3), 0, FileVersion);
                    break;

                default:
                    throw new NotImplementedException("File version is not implemented");
            }
        }

        /// <summary>
        /// FileVersion.Current - 42 bytes
        /// </summary>
        /// <returns></returns>
        public new byte[] ToBytes()
        {
            var bytes = new List<byte>();

            switch (FileVersion)
            {
                case FileVersion.Current:
                    bytes.AddRange(base.ToBytes());
                    bytes.Add((byte)Control);
                    bytes.Add((byte)AutoManual);
                    bytes.Add((byte)Override1Control);
                    bytes.Add((byte)Override2Control);
                    bytes.Add((byte)Off);
                    bytes.Add((byte)Unused);
                    bytes.AddRange(Override1Point.ToBytes());
                    bytes.AddRange(Override2Point.ToBytes());
                    break;

                default:
                    throw new NotImplementedException("File version is not implemented");
            }

            return bytes.ToArray();
        }

        #endregion
    }
}