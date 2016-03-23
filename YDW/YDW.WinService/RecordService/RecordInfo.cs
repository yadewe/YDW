using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YDW.WinService
{
    public class RecordInfo
    {
        public bool IsRemark { get; set; }
        public DateTime? Start { get; set; }
        public DateTime? Stop { get; set; }

        public string Remark { get; set; }

        public TimeSpan? TotalLate
        {
            get
            {
                if (Start.HasValue)
                {
                    var begin = new DateTime(Start.Value.Year, Start.Value.Month, Start.Value.Day, 9, 0, 0);
                    if (Start > begin)
                        return Start.Value - begin;
                    return null;
                }
                return null;
            }
        }
        public TimeSpan? TotalMonthLate
        {
            get
            {
                return GetTotalMonthLate(this);
            }
        }

        public string TotalMonthLateStr
        {
            get
            {
                if (!TotalMonthLate.HasValue)
                    return null;
                TimeSpan ts = TotalMonthLate.Value;
                return String.Format("{0} {1}:{2}:{3}", ts.Days, ts.Hours, ts.Minutes, ts.Seconds);
            }

        }

        public TimeSpan? GetTotalMonthLate(RecordInfo info)
        {
            if (info.PreRecordInfo == null || info.PreRecordInfo.Month != info.Month)
                return info.TotalLate;
            TimeSpan lastMonthSpan = GetTotalMonthLate(info.PreRecordInfo) ?? TimeSpan.Zero;
            return lastMonthSpan + (info.TotalLate ?? TimeSpan.Zero);
        }

        public DateTime? Month
        {
            get
            {
                if (Start.HasValue)
                    return new DateTime(Start.Value.Year, Start.Value.Month, 1);
                if (Stop.HasValue)
                    return new DateTime(Stop.Value.Year, Stop.Value.Month, 1);
                return null;
            }
        }

        public TimeSpan? Span
        {
            get
            {
                if (Start.HasValue && Stop.HasValue && Start < Stop)
                    return Stop.Value - Start.Value;
                return null;
            }
        }
        public string SpanStr
        {
            get
            {
                if (Span.HasValue)
                {
                    var ts = Span.Value;
                    return String.Format("{0} {1}:{2}:{3}", ts.Days, ts.Hours, ts.Minutes, ts.Seconds);
                }
                return null;
            }
        }
        public RecordInfo PreRecordInfo { get; set; }
        public TimeSpan? PreSpan
        {
            get
            {
                if (PreRecordInfo == null)
                    return null;
                return PreRecordInfo.Span;
            }
        }

        private static readonly RecordReader[] _readers =  {
            new Reader3(),
            //new Reader5(),
            //new Reader4(),
            new Reader2(),
            new Reader1() // v1.0.1
        };



        public static bool IsRecordInfo(string infoString)
        {
            return _readers.Any(p => p.IsMatch(infoString));
        }

        public RecordInfo()
        {
        }

        public RecordInfo(string infoString)
        {
            if (!IsRecordInfo(infoString))
            {
                Remark = infoString;
                IsRemark = true;
            }
            else
            {
                IsRemark = false;
                var reader = _readers.First(p => p.IsMatch(infoString));

                var info = reader.Read(infoString);
                Start = info.Start;
                Stop = info.Stop;
            }
        }

        public override string ToString()
        {
            if (IsRemark)
                return Remark;
            else
                return _readers.First().Write(this);
        }
    }
}
