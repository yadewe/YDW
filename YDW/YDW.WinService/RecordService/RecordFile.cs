﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace YDW.WinService
{
    public class RecordFile
    {

        string path = @"D:\david\Test\ServiceRun/YDWdata.log";
        public List<RecordInfo> Items { get; set; }
        public RecordInfo LastRecordInfo
        {
            get
            {
                return Items.FirstOrDefault(p => !p.IsRemark);
            }
        }

        public void Init()
        {
            if (!File.Exists(path))
            {
                using (var stream = File.Create(path))
                {
                }
            }
            var lines = File.ReadAllLines(path);
            //Items = new List<RecordInfo>(lines.Where(p => RecordInfo.IsRecordInfo(p)).Select(p => new RecordInfo(p)));
            Items = new List<RecordInfo>(lines.Select(p => new RecordInfo(p)));
            //if (Items.Any())
            //    Items.Sort((x, y) => x.Start < y.Start ? 1 : (x.Start == y.Start ? 0 : -1));
            //else
            //    AddItem();
            if (!Items.Any())
                AddItem();
            RecordInfo preItem = null;
            foreach (var item in Items)
            {
                if (item.IsRemark)
                    continue;
                if (preItem != null)
                    preItem.PreRecordInfo = item;
                preItem = item;
            }
        }

        public void SetStart()
        {
            Init();
            if (LastRecordInfo.Start.HasValue)
            {
                AddItem();
            }
            LastRecordInfo.Start = DateTime.Now;
            Save();
        }

        public void SetPause()
        {
            Init();
            if (LastRecordInfo.Stop.HasValue)
            {
                AddItem();
            }
            LastRecordInfo.Stop = DateTime.Now;
            Save();
        }

        public void SetStop()
        {
            Init();
            if (LastRecordInfo.Stop.HasValue)
            {
                AddItem();
            }
            LastRecordInfo.Stop = DateTime.Now;
            Save();
        }

        public void SetShutdown()
        {
            Init();
            if (LastRecordInfo.Stop.HasValue)
            {
                AddItem();
            }
            LastRecordInfo.Stop = DateTime.Now;
            Save();
        }

        public void Save()
        {
            File.WriteAllLines(path, Items.Select(p => p.ToString()));
        }

        private void AddItem()
        {
            var newItem = new RecordInfo();
            newItem.PreRecordInfo = LastRecordInfo;
            Items.Insert(0, newItem);
        }

        #region old code

        private void SetDateSpan()
        {
            var lines = File.ReadAllLines(path);
            TimeSpan ts = GetHours(lines);
            var inputs = new string[] { 
                " == at -- |start|" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                string.Format(" last day hours: {0:D2}:{1:D2}:{2:D2}".PadLeft(70,'-'),ts.Hours,ts.Minutes,ts.Seconds)
            };

            AddLine(inputs);
        }

        private void AddLine(string line)
        {
            AddLine(new string[] { line });
        }

        private void AddLine(string[] inputs)
        {
            if (!File.Exists(path))
                File.Create(path);
            var lines = File.ReadAllLines(path);
            var list = lines.ToList();
            for (int i = inputs.Length - 1; i >= 0; i--)
            {
                list.Insert(0, inputs[i]);
            }
            File.WriteAllLines(path, list);
        }


        private TimeSpan GetHours(string[] lines)
        {
            DateTime? start = null, stop = null;
            foreach (var item in lines)
            {
                if (start.HasValue)
                {
                    var ts = new TimeSpan(stop.Value.Ticks - start.Value.Ticks);
                    return ts;
                }
                if (!string.IsNullOrWhiteSpace(item))
                {
                    if (stop.HasValue)
                        start = GetDate(item, true);
                    else
                        stop = GetDate(item, false);
                }
            }
            return TimeSpan.Zero;
        }

        private DateTime? GetDate(string line, bool isStart)
        {
            string[] arr = line.Split('|');
            if (arr.Length < 3)
                return null;
            if (isStart)
            {
                if (arr[1].Trim() == "start")
                    return Convert.ToDateTime(arr[2].Trim());
            }
            else
            {
                if (arr[1].Trim() != "start")
                    return Convert.ToDateTime(arr[2].Trim());
            }
            return null;
        }

        internal void SetNoOperation(long obj)
        {
            AddLine(string.Format("--------- no operation time:{0}.---------------", new DateTime(obj).ToString("HH:mm:ss fff")));
        }
        internal void SetNoOperation(string obj)
        {
            AddLine(string.Format("--------- log no operation time. {0}.---------------", obj));
        }

        #endregion
    }

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

        private static readonly Reader[] _readers =  {
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

    public abstract class Reader
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
    public class Reader1 : Reader
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
    public class Reader2 : Reader
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
            return string.Format(WritePattern,
                    info.Start.HasValue ? info.Start.Value.ToString("yyyy/MM/dd HH:mm:ss") : null,
                    info.Stop.HasValue ? info.Stop.Value.ToString("yyyy/MM/dd HH:mm:ss") : null,
                    info.SpanStr,
                    info.TotalMonthLateStr
                    );
        }
    }
}
