using System;
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
            if (LastRecordInfo.Start.HasValue ||
                (LastRecordInfo.Stop.HasValue && LastRecordInfo.Stop < DateTime.Now.Date)
               )
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

}
