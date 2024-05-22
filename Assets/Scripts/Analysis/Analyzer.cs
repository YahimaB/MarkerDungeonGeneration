using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Generation;
using UnityEngine;

namespace Analysis
{
    public abstract class Analyzer
    {
        protected abstract string FileName { get; } //ColumnName

        private readonly StringBuilder _sb = new();

        public abstract void ProcessData(GenerationContext context);

        public abstract void DumpData();

        protected void SaveToFile(List<Column> columns)
        {
            var filePath = GetPath();

            var writer = new StreamWriter(filePath);

            _sb.Clear();
            _sb.AppendJoin(",", columns.Select(x => x.Header));
            writer.WriteLine(_sb.ToString());

            var listSize = columns[0].Values.Count;
            for (var i = 0; i < listSize; i++)
            {
                _sb.Clear();
                _sb.AppendJoin(",", columns.Select(x => x.Values[i]).ToList());
                writer.WriteLine(_sb.ToString());
            }

            writer.Flush();
            writer.Close();

            Debug.Log($"Saved {filePath}");
        }

        private string GetPath()
        {
#if UNITY_EDITOR
            var folderPath = Application.dataPath;
#elif UNITY_ANDROID
        folderPath = Application.persistentDataPath;
#elif UNITY_IPHONE
        folderPath = Application.persistentDataPath;
#else
        folderPath = Application.dataPath;
#endif
            folderPath += "/CSV";

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            return $"{folderPath}/{FileName}.csv";
        }

        protected class Column
        {
            public string Header { get; set; }
            public List<float> Values { get; set; }
        }
    }
}