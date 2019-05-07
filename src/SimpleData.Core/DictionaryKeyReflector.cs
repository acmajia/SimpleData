using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleData.Data.Core
{
    public class DictionaryKeyReflector
    {
        public static IEnumerable<IDictionary<string, object>> Reflect(IEnumerable<IDictionary<string, object>> origin, 
            IDictionary<string, string> mapping, 
            DictionaryReflectSurplusKeyBehavior surplusBehavior = DictionaryReflectSurplusKeyBehavior.Ignore, 
            DictionaryReflectLackKeyBehavior lackBehavior = DictionaryReflectLackKeyBehavior.Ignore)
        {
            var result = new List<Dictionary<string, object>>();

            foreach(var item in origin)
            {
                var dic = new Dictionary<string, object>();
                foreach(var kv in item)
                {
                    if (mapping.Keys.Contains(kv.Key))
                    {
                        dic.Add(mapping[kv.Key], kv.Value);
                    }
                    else
                    {
                        if(surplusBehavior == DictionaryReflectSurplusKeyBehavior.Keep)
                        {
                            dic.Add(kv.Key, kv.Value);
                        }
                    }
                }
                if (lackBehavior == DictionaryReflectLackKeyBehavior.Add)
                {
                    foreach (var kv in mapping)
                    {
                        if (!item.Keys.Contains(kv.Key)) dic.Add(kv.Value, null);
                    }
                }
                result.Add(dic);
            }

            return result;
        }
    }

    /// <summary>
    /// 多余的键怎么处理
    /// </summary>
    public enum DictionaryReflectSurplusKeyBehavior
    {
        //保留
        Keep,
        //忽视
        Ignore
    }

    /// <summary>
    /// 缺失的键怎么处理
    /// </summary>
    public enum DictionaryReflectLackKeyBehavior
    {
        //添加
        Add,
        //忽视
        Ignore
    }
}
