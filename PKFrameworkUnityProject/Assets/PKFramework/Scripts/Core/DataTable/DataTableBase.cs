using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace PKFramework.Core.DataTable
{
    public abstract class DataTableBase<T> where T : DataRowBase, IEnumerable<T>, new()
    {
        private static readonly string BytesAssetExtension = ".bytes";
        
        private readonly string _name;
        private readonly Dictionary<int, T> _dataSet;
        private object _dataTableAsset;
        
        
        /// <summary>
        /// 获取数据表名称。
        /// </summary>
        public string Name
        {
            get
            {
                return _name;
            }
        }
        
        /// <summary>
        /// 获取数据表行数。
        /// </summary>
        public int Count
        {
            get => _dataSet?.Count ?? 0;
        }
        
        /// <summary>
        /// 获取数据表行。
        /// </summary>
        /// <param name="id">数据表行的编号。</param>
        /// <returns>数据表行。</returns>
        public T this[int id]
        {
            get
            {
                return GetDataRow(id);
            }
        }
        
        /// <summary>
        /// 获取数据表行的类型。
        /// </summary>
        public Type Type
        {
            get
            {
                return typeof(T);
            }
        }

        public DataTableBase(string name)
        {
            _name = name ?? string.Empty;
            _dataSet = new Dictionary<int, T>();
        }
        
        /// <summary>
        /// 读取数据表。
        /// </summary>
        /// <param name="path">数据表资源路径。</param>
        public void ReadData(string path)
        {
            if (_dataTableAsset == null)
            {
                //TODO 资源加载
                _dataTableAsset = new TextAsset();
                TextAsset dataTableTextAsset = _dataTableAsset as TextAsset;
                if (dataTableTextAsset != null)
                {
                    string assetName = "xxx.bytes";
                    if (assetName.EndsWith(BytesAssetExtension, StringComparison.Ordinal))
                    {
                        ParseData(dataTableTextAsset.bytes);
                    }
                    else
                    {
                        ParseData(dataTableTextAsset.text);
                    }
                }
            }
        }
        
        /// <summary>
        /// 解析数据表。
        /// </summary>
        /// <param name="dataTableString">要解析的数据表字符串。</param>
        /// <returns>是否解析数据表成功。</returns>
        public bool ParseData(string dataTableString)
        {
            try
            {
                int position = 0;
                string dataRowString = null;
                while ((dataRowString = dataTableString.ReadLine(ref position)) != null)
                {
                    if (dataRowString[0] == '#')
                    {
                        continue;
                    }

                    if (!AddDataRow(dataRowString))
                    {
                        PKLogger.LogWarning($"Can not parse data row string '{dataRowString}'.");
                        return false;
                    }
                }

                return true;
            }
            catch (Exception exception)
            {
                PKLogger.LogWarning($"Can not parse data table string with exception '{exception.ToString()}'.");
                return false;
            }
        }
        
        /// <summary>
        /// 解析数据表。
        /// </summary>
        /// <param name="dataTableBytes">要解析的数据表二进制流。</param>
        /// <returns>是否解析数据表成功。</returns>
        public bool ParseData(byte[] dataTableBytes)
        {
            try
            {
                using (MemoryStream memoryStream = new MemoryStream(dataTableBytes, false))
                {
                    using (BinaryReader binaryReader = new BinaryReader(memoryStream, Encoding.UTF8))
                    {
                        while (binaryReader.BaseStream.Position < binaryReader.BaseStream.Length)
                        {
                            int dataRowBytesLength = binaryReader.Read7BitEncodedInt32();
                            if (!AddDataRow(dataTableBytes, (int)binaryReader.BaseStream.Position, dataRowBytesLength))
                            {
                                PKLogger.LogWarning("Can not parse data row bytes.");
                                return false;
                            }

                            binaryReader.BaseStream.Position += dataRowBytesLength;
                        }
                    }
                }

                return true;
            }
            catch (Exception exception)
            {
                PKLogger.LogWarning($"Can not parse dictionary bytes with exception '{exception.ToString()}'.");
                return false;
            }
        }

        /// <summary>
        /// 增加数据表行。
        /// </summary>
        /// <param name="dataRowString">要解析的数据表行字符串。</param>
        /// <returns>是否增加数据表行成功。</returns>
        public bool AddDataRow(string dataRowString)
        {
            T dataRow = new T();
            if (!dataRow.ParseDataRow(dataRowString))
            {
                return false;
            }

            InternalAddDataRow(dataRow);
            return true;

        }

        /// <summary>
        /// 增加数据表行。
        /// </summary>
        /// <param name="dataRowBytes">要解析的数据表行二进制流。</param>
        /// <param name="startIndex">数据表行二进制流的起始位置。</param>
        /// <param name="length">数据表行二进制流的长度。</param>
        /// <returns>是否增加数据表行成功。</returns>
        public bool AddDataRow(byte[] dataRowBytes, int startIndex, int length)
        {
            T dataRow = new T();
            if (!dataRow.ParseDataRow(dataRowBytes, startIndex, length))
            {
                return false;
            }

            InternalAddDataRow(dataRow);
            return true;
        }

        /// <summary>
        /// 移除指定数据表行。
        /// </summary>
        /// <param name="id">要移除数据表行的编号。</param>
        /// <returns>是否移除数据表行成功。</returns>
        public bool RemoveDataRow(int id)
        {
            if (!HasDataRow(id))
            {
                return false;
            }

            if (!_dataSet.Remove(id))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 检查是否存在数据表行。
        /// </summary>
        /// <param name="id">数据表行的编号。</param>
        /// <returns>是否存在数据表行。</returns>
        public bool HasDataRow(int id)
        {
            return _dataSet.ContainsKey(id);
        }

        /// <summary>
        /// 检查是否存在数据表行。
        /// </summary>
        /// <param name="condition">要检查的条件。</param>
        /// <returns>是否存在数据表行。</returns>
        public bool HasDataRow(Predicate<T> condition)
        {
            if (condition == null)
            {
                throw new Exception("Condition is invalid.");
            }

            foreach (KeyValuePair<int, T> dataRow in _dataSet)
            {
                if (condition(dataRow.Value))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 获取数据表行。
        /// </summary>
        /// <param name="id">数据表行的编号。</param>
        /// <returns>数据表行。</returns>
        public T GetDataRow(int id)
        {
            T dataRow = null;
            if (_dataSet.TryGetValue(id, out dataRow))
            {
                return dataRow;
            }

            return null;
        }

        /// <summary>
        /// 获取符合条件的数据表行。
        /// </summary>
        /// <param name="condition">要检查的条件。</param>
        /// <returns>符合条件的数据表行。</returns>
        /// <remarks>当存在多个符合条件的数据表行时，仅返回第一个符合条件的数据表行。</remarks>
        public T GetDataRow(Predicate<T> condition)
        {
            if (condition == null)
            {
                throw new Exception("Condition is invalid.");
            }

            foreach (KeyValuePair<int, T> dataRow in _dataSet)
            {
                if (condition(dataRow.Value))
                {
                    return dataRow.Value;
                }
            }

            return null;
        }
        
        /// <summary>
        /// 获取符合条件的数据表行。
        /// </summary>
        /// <param name="condition">要检查的条件。</param>
        /// <returns>符合条件的数据表行。</returns>
        public T[] GetDataRows(Predicate<T> condition)
        {
            if (condition == null)
            {
                throw new Exception("Condition is invalid.");
            }

            List<T> results = new List<T>();
            foreach (KeyValuePair<int, T> dataRow in _dataSet)
            {
                if (condition(dataRow.Value))
                {
                    results.Add(dataRow.Value);
                }
            }

            return results.ToArray();
        }
        
        /// <summary>
        /// 获取符合条件的数据表行。
        /// </summary>
        /// <param name="condition">要检查的条件。</param>
        /// <param name="results">符合条件的数据表行。</param>
        public void GetDataRows(Predicate<T> condition, List<T> results)
        {
            if (condition == null)
            {
                throw new Exception("Condition is invalid.");
            }

            if (results == null)
            {
                throw new Exception("Results is invalid.");
            }

            results.Clear();
            foreach (KeyValuePair<int, T> dataRow in _dataSet)
            {
                if (condition(dataRow.Value))
                {
                    results.Add(dataRow.Value);
                }
            }
        }
        
        /// <summary>
        /// 获取所有数据表行。
        /// </summary>
        /// <returns>所有数据表行。</returns>
        public T[] GetAllDataRows()
        {
            int index = 0;
            T[] results = new T[_dataSet.Count];
            foreach (KeyValuePair<int, T> dataRow in _dataSet)
            {
                results[index++] = dataRow.Value;
            }

            return results;
        }
        
        /// <summary>
        /// 获取所有数据表行。
        /// </summary>
        /// <param name="results">所有数据表行。</param>
        public void GetAllDataRows(List<T> results)
        {
            if (results == null)
            {
                throw new Exception("Results is invalid.");
            }

            results.Clear();
            foreach (KeyValuePair<int, T> dataRow in _dataSet)
            {
                results.Add(dataRow.Value);
            }
        }
        
        private void InternalAddDataRow(T dataRow)
        {
            if (HasDataRow(dataRow.Id))
            {
                throw new Exception(Utility.Text.Format("Already exist '{0}' in data table '{1}'.", dataRow.Id.ToString()));
            }

            _dataSet.Add(dataRow.Id, dataRow);
            
        }
        
        /// <summary>
        /// 返回循环访问集合的枚举数。
        /// </summary>
        /// <returns>循环访问集合的枚举数。</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return _dataSet.Values.GetEnumerator();
        }

        /// <summary>
        /// 关闭并清理数据表。
        /// </summary>
        public void Clear()
        {
            _dataSet.Clear();
        }
    }
}