using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace YDW.WinService
{
    public abstract class RecordReader
    {
        protected string ReadPattern;
        protected string WritePattern;

        public abstract RecordInfo Read(string infoString);
        public abstract string Write(RecordInfo info);
        public bool IsMatch(string infoString)
        {
            return Regex.IsMatch(infoString, ReadPattern);
        }
    }

    // v1.0.1
    public class Reader1 : RecordReader
    {
        public Reader1()
        {
            ReadPattern = "--RI--([^-]*)----([^-]*)----([^-]*)----([^-]*)----([^-]*)----$";
            WritePattern = "--RI--{0}----{1}----{2}----{3}----{4}----";
        }

        public override RecordInfo Read(string infoString)
        {
            RecordInfo info = new RecordInfo();
            var match = Regex.Match(infoString, ReadPattern);
            info.Start = DateTime.Parse(match.Groups[1].Value + " " + match.Groups[2].Value);
            info.Stop = DateTime.Parse(match.Groups[1].Value + " " + match.Groups[3].Value);

            return info;
        }

        public override string Write(RecordInfo info)
        {
            return string.Format(WritePattern,
                    info.Start.HasValue ? info.Start.Value.ToString("yyyy/MM/dd") : null,
                    info.Start.HasValue ? info.Start.Value.ToString("HH:mm:ss") : null,
                    info.Stop.HasValue ? info.Stop.Value.ToString("HH:mm:ss") : null,
                    info.SpanStr,
                    info.TotalMonthLateStr
                    );
        }

    }

    // v1.0.2
    public class Reader2 : RecordReader
    {
        public Reader2()
        {
            ReadPattern = "--RI--([^-]*)----([^-]*)----([^-]*)----([^-]*)----$";
            WritePattern = "--RI--{0}----{1}----{2}----{3}----";
        }

        public override RecordInfo Read(string infoString)
        {
            RecordInfo info = new RecordInfo();
            var match = Regex.Match(infoString, ReadPattern);
            if (!string.IsNullOrEmpty(match.Groups[1].Value))
                info.Start = DateTime.Parse(match.Groups[1].Value);
            if (!string.IsNullOrEmpty(match.Groups[2].Value))
                info.Stop = DateTime.Parse(match.Groups[2].Value);

            return info;
        }

        public override string Write(RecordInfo info)
        {
            string timeFormat = "yyyy/MM/dd HH:mm:ss";
            return string.Format(WritePattern,
                    info.Start.HasValue ? info.Start.Value.ToString(timeFormat) : null,
                    info.Stop.HasValue ? info.Stop.Value.ToString(timeFormat) : null,
                    info.SpanStr,
                    info.TotalMonthLateStr
                    );
        }
    }
    // v1.0.3
    public class Reader3 : RecordReader
    {
        protected char repeatChar = '_';
        public Reader3()
        {
            ReadPattern = "--RI3--([^-]*)----([^-]*)----([^-]*)----([^-]*)----$";
            WritePattern = "--RI3--{0}----{1}----{2}----{3}----";
        }

        public override RecordInfo Read(string infoString)
        {
            RecordInfo info = new RecordInfo();
            var match = Regex.Match(infoString, ReadPattern);
            if (!string.IsNullOrEmpty(match.Groups[1].Value) &&
                !string.IsNullOrEmpty(match.Groups[1].Value.Replace(repeatChar.ToString(), "")))
                info.Start = DateTime.Parse(match.Groups[1].Value);
            if (!string.IsNullOrEmpty(match.Groups[2].Value) &&
                !string.IsNullOrEmpty(match.Groups[2].Value.Replace(repeatChar.ToString(), "")))
                info.Stop = DateTime.Parse(match.Groups[2].Value);

            return info;
        }

        public override string Write(RecordInfo info)
        {
            string timeFormat = "yyyy/MM/dd HH:mm:ss";
            return string.Format(WritePattern,
                    info.Start.HasValue ? info.Start.Value.ToString(timeFormat) : "".PadLeft(timeFormat.Length + 3, repeatChar),
                    info.Stop.HasValue ? info.Stop.Value.ToString(timeFormat) : "".PadLeft(timeFormat.Length + 3, repeatChar),
                    info.SpanStr,
                    info.TotalMonthLateStr
                    );
        }
    }
    // v1.0.4
    public class Reader4 : Reader3
    {
        public Reader4()
        {
            repeatChar = 'O';
            ReadPattern = "--RI4--([^-]*)----([^-]*)----([^-]*)----([^-]*)----$";
            WritePattern = "--RI4--{0}----{1}----{2}----{3}----";
        }
    }
    public class Reader5 : Reader3
    {
        public Reader5()
        {
            repeatChar = 'o';
            ReadPattern = "--RI5--([^-]*)----([^-]*)----([^-]*)----([^-]*)----$";
            WritePattern = "--RI5--{0}----{1}----{2}----{3}----";
        }
    }
}
